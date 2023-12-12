using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apolyton.FastJson.Data;
using System.Runtime.Serialization;
using System.Globalization;
using Apolyton.FastJson.Registry;
using System.Collections;

namespace Apolyton.FastJson.Serialization
{
    /// <summary>
    /// Represents a simple deserializer which returns only the data types of this assembly.
    /// </summary>
    internal class JsonValueDeserializer : JsonDeserializer
    {
        private readonly JsonParameters _params;
        private readonly JsonRegistry _registry;
        private readonly JsonTypeDescriptor _typeDescriptor;
        private bool _useTypeExtension;

        /// <summary>
        /// Creates an instance of the deserializer.
        /// </summary>
        public JsonValueDeserializer(JsonParameters parameters)
        {
            _params = parameters;
            _registry = parameters.Registry;
            _typeDescriptor = parameters.Registry.TypeDescriptor;
            _useTypeExtension = parameters.UseTypeExtension;
        }

        /// <summary>
        /// Returns a serialized json value.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        internal IJsonValue Deserialize(ref String jsonString)
        {
            this._jsonOriginal = jsonString.ToCharArray();
            this._index = 0;

            return ParseValue();
        }

        /// <summary>
        /// The public interface to build up a given instance with values from the dictionary. 
        /// </summary>
        /// <remarks>
        /// In contrary to the internal BuildObject method, this flattens the input variables for dictionaries and lists which is the normally expected behavior 
        /// (if a dictionary is provided, and values is dictionary, fill the values of the source dictionary with those from the target).
        /// </remarks>
        /// <param name="instanceToBuild"></param>
        /// <param name="values"></param>
        internal void BuildUp(object instanceToBuild, JsonArray values)
        {
            var info = _registry.GetTypeInfo(instanceToBuild.GetType());
            BuildList(info, instanceToBuild, values);
        }

        /// <summary>
        /// The public interface to build up a given instance with values from the dictionary. 
        /// </summary>
        /// <remarks>
        /// In contrary to the internal BuildObject method, this flattens the input variables for dictionaries and lists which is the normally expected behavior 
        /// (if a dictionary is provided, and values is dictionary, fill the values of the source dictionary with those from the target).
        /// </remarks>
        /// <param name="instanceToBuild"></param>
        /// <param name="values"></param>
        internal void BuildUp(object instanceToBuild, JsonObject values)
        {
            BuildObject(instanceToBuild, values);
        }

        private void SetValue(JsonPropertyInfo info, object instanceToBuild, IJsonValue localValue)
        {
            var targetValue = ChangeType(info, localValue);

            if (info.Setter == null) { throw new SerializationException(String.Format("Cannot set the value for '{0}'. Is the setter public?", info.PropertyOrFieldName)); }

            if (targetValue == null && info.IsValueType)
            {
                info.Setter(instanceToBuild, info.DefaultValue);
            }
            else
            {
                info.Setter(instanceToBuild, targetValue);
            }
        }

        private void BuildObject(object instanceToBuild, JsonObject jsonObject)
        {
            foreach (JsonPropertyInfo info in _registry.GetPropertiesAndFields(instanceToBuild.GetType(), null).Values)
            {
                IJsonValue localValue = null;
                if (!jsonObject.TryGetValue(info.JsonFieldName, out localValue))
                {
                    continue;
                }
                else if (localValue == null)
                {
                    if (info.IsValueType)
                    {
                        info.Setter(instanceToBuild, info.DefaultValue);
                    }
                    else
                    {
                        info.Setter(instanceToBuild, null);
                    }

                    continue;
                }

                if (info.IsDictionary)
                {
                    var dictionary = (IDictionary)_registry.CreateInstanceFast(info.PropertyOrFieldType);

                    if (localValue.Type == JsonType.JsonObject)
                    {
                        BuildDictionary(info, dictionary, (JsonObject)localValue);
                    }
                    else
                    {
                        // The value is represents a list of key-value pairs.
                        BuildDictionary(info, dictionary, (JsonArray)localValue);
                    }

                    info.Setter(instanceToBuild, dictionary);
                }
                else
                {
                    SetValue(info, instanceToBuild, localValue);
                }
            }
        }

        /// <summary>
        /// Builds a dictionary from a given list of key value pairs
        /// </summary>
        /// <param name="info"></param>
        /// <param name="keyValuePairs"></param>
        private void BuildDictionary(JsonPropertyInfo info, IDictionary instanceToBuild, JsonArray keyValuePairs)
        {
            if (info.IsGenericType)
            {
                var keyType = info.GenericTypes[0];
                var valueType = info.GenericTypes[1]; // Dictionary has (almost) always 2 generic types.

                foreach (var keyValuePair in keyValuePairs)
                {
                    var key = ChangeType(keyType, keyValuePair["k"]);
                    var value = ChangeType(valueType, keyValuePair["v"]);

                    instanceToBuild.Add(key, value);
                }
            }
            else
            {
                foreach (var keyValuePair in keyValuePairs)
                {
                    // TODO: check for type extensions, no conversion for now.
                    instanceToBuild.Add(keyValuePair["k"], keyValuePair["v"]);
                }
            }
        }

        private void BuildDictionary(JsonPropertyInfo info, IDictionary instanceToBuild, JsonObject values)
        {
            var keyType = info.GenericTypes[0];
            var valueType = info.GenericTypes[1]; // Dictionary has almost always 2 generic types.

            foreach (var keyValuePair in values)
            {
                if (keyType == typeof(string))
                {
                    instanceToBuild.Add(keyValuePair.Key, ChangeType(valueType, keyValuePair.Value));
                }
                else
                {
                    instanceToBuild.Add(
                        ChangeType(valueType, new JsonPrimitive(keyValuePair.Key, _registry)),
                        ChangeType(valueType, keyValuePair.Value));
                }
            }
        }

        /// <summary>
        /// Populates the given array with converted JsonPrimitive values.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="instanceToBuild"></param>
        /// <param name="values"></param>
        private void BuildGenericArray(JsonPropertyInfo info, Array instanceToBuild, JsonArray values)
        {
            for (int i = 0; i < instanceToBuild.Length; i++)
            {
                var localValue = ChangeType(info.GenericItemType, values[i]);
                instanceToBuild.SetValue(localValue, i);
            }
        }

        /// <summary>
        /// Populates the given array with the JsonPrimitive values (no item type information available).
        /// </summary>
        /// <param name="info"></param>
        /// <param name="instanceToBuild"></param>
        /// <param name="values"></param>
        private void BuildArray(Array instanceToBuild, JsonArray values)
        {
            for (int i = 0; i < instanceToBuild.Length; i++)
            {
                instanceToBuild.SetValue(values[i], i);
            }
        }

        private void BuildList(JsonPropertyInfo info, object instanceToBuild, JsonArray values)
        {
            IList instanceAsList = instanceToBuild as IList;
            if (instanceAsList != null)
            {
                if (info.IsGenericType)
                {
                    foreach (IJsonValue value in values)
                    {
                        instanceAsList.Add(ChangeType(info.GenericItemType, value));
                    }
                }
                else
                {
                    // if the target list is not generic, we don't know which item type we should insert. Take the simplest and add the json value.
                    foreach (IJsonValue value in values)
                    {
                        instanceAsList.Insert(0, value);
                    }
                }

                return;
            }

            // ICollection has no Add method.
            throw new NotSupportedException("Type must implement IList in order to be filled.");
        }

        private Object ChangeType(Type targetType, IJsonValue genericValue)
        {
            if (genericValue == null)
            {
                return null;
            }

            if (genericValue.Type == JsonType.JsonPrimitive)
            {
                return Convert.ChangeType(genericValue, targetType, CultureInfo.InvariantCulture);
            }
            else
            {
                JsonPropertyInfo info = _registry.GetTypeInfo(targetType);
                return ChangeType(info, genericValue);
            }
        }

        private Object ChangeType(JsonPropertyInfo info, IJsonValue genericValue)
        {
            if (genericValue == null)
            {
                return null;
            }

            if (genericValue.Type == JsonType.JsonPrimitive)
            {
                JsonPrimitive jsonPrimitive = genericValue as JsonPrimitive;
                jsonPrimitive.FieldInfo = info;

                return Convert.ChangeType(genericValue, info.PropertyOrFieldType, CultureInfo.InvariantCulture);
            }

            object instanceToBuild = null;

            if (genericValue.Type == JsonType.JsonArray)
            {
                Type targetType = info.PropertyOrFieldType;
                var valueAsArray = (JsonArray)genericValue;

                if (targetType.IsArray)
                {
                    // We can't use FastCreateInstance for arrays since their size must be known at creation time.
                    instanceToBuild = Array.CreateInstance(info.GenericItemType, valueAsArray.Count);
                    BuildGenericArray(info, (Array)instanceToBuild, valueAsArray);
                }
                else if (targetType.IsInterface)
                {
                    if (info.IsGenericType)
                    {
                        // Building into interface can be a mess, esspecially, if the setter is casting on its own. However, in that we 
                        // consider this as an implementation issue on the external class.
                        instanceToBuild = Array.CreateInstance(info.GenericItemType, valueAsArray.Count);
                        BuildGenericArray(info, (Array)instanceToBuild, valueAsArray);
                    }
                    else
                    {
                        instanceToBuild = Array.CreateInstance(typeof(JsonPrimitive), valueAsArray.Count);
                        BuildArray((Array)instanceToBuild, valueAsArray);
                    }
                }
                else if (!info.IsHashSet)
                {
                    instanceToBuild = _registry.CreateInstanceFast(targetType);
                    BuildList(info, instanceToBuild, valueAsArray);
                }
                else
                {
                    var convertedValues = Array.CreateInstance(info.GenericItemType, valueAsArray.Count);
                    BuildGenericArray(info, convertedValues, valueAsArray);

                    instanceToBuild = Activator.CreateInstance(info.PropertyOrFieldType, convertedValues);
                }
            }
            else
            {
                JsonObject valueAsObject = (JsonObject)genericValue;
                Type targetType = null;

                if (_useTypeExtension)
                {
                    String customTypeName = valueAsObject.TryGetTypeName();

                    if (!_typeDescriptor.TryGetType(customTypeName, out targetType))
                    {
                        targetType = info.PropertyOrFieldType;
                    }
                }
                else
                {
                    targetType = info.PropertyOrFieldType;
                }

                instanceToBuild = _registry.CreateInstanceFast(targetType);
                BuildObject(instanceToBuild, valueAsObject);
            }

            return instanceToBuild;
        }

        #region Parsing

        private IJsonValue ParseValue()
        {
            switch (LookAhead())
            {
                case Token.Number:
                    return ParseNumberString();

                case Token.String:
                    return new JsonPrimitive(ParseString(), _registry);

                case Token.Curly_Open:
                    return ParseObject();

                case Token.Squared_Open:
                    return ParseArray();

                case Token.True:
                    ConsumeToken();
                    return new JsonPrimitive(true);

                case Token.False:
                    ConsumeToken();
                    return new JsonPrimitive(false);

                case Token.Null:
                    ConsumeToken();
                    return null;
            }

            throw new SerializationException("Unrecognized token at index" + _index);
        }

        private JsonObject ParseObject()
        {
            JsonObject table = new JsonObject();

            ConsumeToken(); // {

            while (true)
            {
                switch (LookAhead())
                {
                    case Token.Comma:
                        ConsumeToken();
                        break;

                    case Token.Curly_Close:
                        ConsumeToken();
                        return table;

                    default:
                        {
                            // name
                            string name = ParseString();

                            // :
                            if (NextToken() != Token.Colon)
                            {
                                throw new SerializationException("Expected colon at index " + _index);
                            }

                            // value
                            IJsonValue value = ParseValue();

                            table[name] = value;
                        }
                        break;
                }
            }
        }

        private JsonArray ParseArray()
        {
            JsonArray array = new JsonArray();
            ConsumeToken(); // [

            while (true)
            {
                switch (LookAhead())
                {

                    case Token.Comma:
                        ConsumeToken();
                        break;

                    case Token.Squared_Close:
                        ConsumeToken();
                        return array;

                    default:
                        array.Add(ParseValue());
                        break;
                }
            }
        }

        private JsonPrimitive ParseNumberString()
        {
            ConsumeToken();

            // Need to start back one place because the first digit is also a token and would have been consumed
            var startIndex = _index - 1;

            do
            {
                var c = _jsonOriginal[_index];

                if ((c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E')
                {
                    if (++_index == _jsonOriginal.Length)
                    {
                        break;                        //throw new Exception("Unexpected end of string whilst parsing number");
                    }

                    continue;
                }

                break;

            } while (true);

            string s = new string(_jsonOriginal, startIndex, _index - startIndex);

            return new JsonPrimitive(s);
        }

        #endregion
    }
}
