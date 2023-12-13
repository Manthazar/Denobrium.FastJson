using Denobrium.Json.Common;
using Denobrium.Json.Data;
using Denobrium.Json.Registry;
using Denobrium.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Denobrium.Json
{
    /// <summary>
    /// Entry point for json related operations. 
    /// </summary>
    /// <remarks>
    /// http://www.codeproject.com/Articles/159450/fastJSON
    /// The version over there (2.0.9) could not be taken directly, as its serializer is taking all public properties, disregarding any attribute policy. This is
    /// not good for our case, as we want to return (portions of) data objects as well.
    /// </remarks>
    public sealed class Json
    {
        private static Json _current;
        private JsonParameters _defaultParameters;

        private Json()
        {
            _defaultParameters = new JsonParameters();
        }

        /// <summary>
        /// Returns the list of members which are concerded by (de-)serialization according to the current default parameters.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<JsonSerializationInfo> GetSerializationMembers(Type type)
        {
            return _defaultParameters.GetSerializationMembers(type);
        }

        /// <summary>
        /// Returns a json string for the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string ToJson(object obj)
        {
            return ToJson(obj, _defaultParameters);
        }

        /// <summary>
        /// Returns the bytes of the produced json string according to the encoding as defined in the default parameters.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] ToJsonBytes(object obj)
        {
            var jsonString = ToJson(obj, _defaultParameters);

            return _defaultParameters.Encoding.GetBytes(jsonString);
        }

        /// <summary>
        /// Returns a json string for the given object.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Code sugar: inlines with existing ToJson method which uses local variables")]
        public string ToJson(object source, JsonParameters param)
        {
            Guard.ArgumentNotNull(source, "obj");
            Guard.ArgumentNotNull(param, "param");

            if (source == null)
            {
                return "null";
            }

            return new JsonSerializer(param).Serialize(source);
        }

        /// <summary>
        /// Builds the instance up with the provided values from the value object. Normally used after ReadJsonValue(String).
        /// </summary>
        /// <param name="instanceToBuild"></param>
        /// <param name="jsonValue"></param>
        public void BuildUp(Object instanceToBuild, IJsonValue jsonValue)
        {
            Guard.ArgumentNotNull(instanceToBuild, "instanceToBuild");

            if (jsonValue == null) { return; }

            if (jsonValue is JsonObject valueAsObject)
            {
                new JsonValueDeserializer(_defaultParameters).BuildUp(instanceToBuild, valueAsObject);
            }
            else
            {
                if (jsonValue is JsonArray valueAsArray)
                {
                    new JsonValueDeserializer(_defaultParameters).BuildUp(instanceToBuild, valueAsArray);
                }
                else
                {
                    throw new NotSupportedException("This type of json value is not supported.");
                }
            }
        }

        /// <summary>
        /// Returns the parsed json string as an object which is IDictionary or IList. Number types are parsed, however all nested objects remain generic implementations
        /// of IDictionary or IList.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public IEnumerable ReadSomeObject(string jsonString)
        {
            Guard.ArgumentNotNullOrEmpty(jsonString, "jsonString");

            return new JsonHybridDeserializer(_defaultParameters).Deserialize(ref jsonString);
        }

        /// <summary>
        /// Returns the strongly typed object for the given json string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public T ReadObject<T>(string jsonString)
        {
            Guard.ArgumentNotNullOrEmpty(jsonString, "jsonString");

            return (T)ReadObjectInternal(ref jsonString, typeof(T), _defaultParameters);
        }

        /// <summary>
        /// Returns the object with the best fitting type for the given json string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public object ReadObject(string jsonString)
        {
            Guard.ArgumentNotNullOrEmpty(jsonString, "jsonString");

            return ReadObjectInternal(ref jsonString, null, _defaultParameters);
        }

        /// <summary>
        /// Returns the object with the specified type for the given json string and the defined target type.
        /// </summary>
        public object ReadObject(string jsonString, Type targetType)
        {
            Guard.ArgumentNotNullOrEmpty(jsonString, "jsonString");
            Guard.ArgumentNotNull(targetType, "targetType");

            return ReadObjectInternal(ref jsonString, targetType, _defaultParameters);
        }

        /// <summary>
        /// Returns the object for the given json string and the defined target type.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private object ReadObjectInternal(ref string jsonString, Type targetType, JsonParameters parameters)
        {
            var deserializer = new JsonValueDeserializer(_defaultParameters);
            var jsonValue = deserializer.Deserialize(ref jsonString);

            if (targetType == null)
            {
                if (jsonValue.Type == JsonType.JsonObject)
                {
                    JsonObject valueAsObject = (JsonObject)jsonValue;
                    String rootLevelTypeName = valueAsObject.TryGetTypeName();

                    parameters.Registry.TypeDescriptor.TryGetType(rootLevelTypeName, out targetType);
                }
                else
                {
                    targetType = typeof(List<Object>);
                }
            }

            if (targetType == null) { throw new InvalidOperationException("Cannot deserialize when target type is not specified and json string does not contain a resolvable $type field."); }
            if (targetType.IsAbstract) { throw new InvalidOperationException("Cannot read into an abstract object and custom types are not yet supported at this level. You might want to use ReadJsonVlaue and BuildUp instead."); }

            var targetInstance = _defaultParameters.Registry.CreateInstanceFast(targetType);

            if (jsonValue.Type == JsonType.JsonObject)
            {
                deserializer.BuildUp(targetInstance, (JsonObject)jsonValue);
            }
            else
            {
                deserializer.BuildUp(targetInstance, (JsonArray)jsonValue);
            }

            return targetInstance;
        }

        /// <summary>
        /// Returns the json value object for the given json string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public IJsonValue ReadJsonValue(string jsonString)
        {
            Guard.ArgumentNotNullOrEmpty(jsonString, "jsonString");

            return new JsonValueDeserializer(_defaultParameters).Deserialize(ref jsonString);
        }

#if DESKTOP
        /// <summary>
        /// Returns the data set represented by the given json string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public DataSet ReadDataSet(string jsonString)
        {
            return (DataSet)new JsonObjectDeserializer(_defaultParameters).Deserialize(ref jsonString, typeof(DataSet));
        }

        /// <summary>
        /// Returns the data table represented by the given json string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public DataTable ReadDataTable(string jsonString)
        {
            return (DataTable)new JsonObjectDeserializer(_defaultParameters).Deserialize(ref jsonString, typeof(DataTable));
        }

#endif

        /// <summary>
        /// Builds up the instance based on the given json object values.
        /// </summary>
        /// <param name="instanceToBuild">A non-null instance which is populated with the provided values</param>
        /// <param name="values">The values which should fill the given instance.</param>
        /// <exception cref="ArgumentNullException"/>
        internal void BuildUp(Object instanceToBuild, JsonObject values)
        {
            Guard.ArgumentNotNull(instanceToBuild, "instanceToBuild");
            Guard.ArgumentNotNull(values, "values");

            new JsonValueDeserializer(_defaultParameters).BuildUp(instanceToBuild, values);
        }

        /// <summary>
        /// Returns a beautified string of the given json string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static string Beautify(string jsonString)
        {
            Guard.ArgumentNotNull(jsonString, "jsonString");

            if (jsonString == String.Empty) { return String.Empty; }

            return Formatter.PrettyPrint(jsonString);
        }

        /// <summary>
        /// Creates a clone of the object by serialize and deserialize it again.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public object DeepCopy(object source)
        {
            Guard.ArgumentNotNull(source, "source");

            return ReadObject(ToJson(source));
        }

        /// <summary>
        /// Creates a clone of the object by serialize and deserialize it again.
        /// </summary>
        public T DeepCopy<T>(T source)
        {
            Guard.ArgumentNotNull(source, "source");

            return ReadObject<T>(ToJson(source));
        }

        /// <summary>
        /// Registers a custom type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serializer"></param>
        /// <param name="deserializer"></param>
        public void RegisterCustomType(Type type, SerializationHandler serializer, DeserializationHandler deserializer)
        {
            _defaultParameters.RegisterCustomType(type, serializer, deserializer);
        }

        /// <summary>
        /// Gets the json instance.
        /// </summary>
        public static Json Current => _current ??= new Json(); 

        /// <summary>
        /// Gets or sets the default parameters.
        /// </summary>
        public JsonParameters DefaultParameters
        {
            get => _defaultParameters; 
            set => _defaultParameters = value; 
        }
    }

}