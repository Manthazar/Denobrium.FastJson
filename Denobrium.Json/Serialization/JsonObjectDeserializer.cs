using System;
using System.Collections;
using System.Collections.Generic;
#if DESKTOP
using System.Data;
#endif
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Apolyton.FastJson.Data;
using Apolyton.FastJson.Registry;

namespace Apolyton.FastJson.Serialization
{
    /// <summary>
    /// This class decodes JSON strings. The root level object is always a mutable json array or a mustable json object. 
    /// Nested types are determined by the $type extension -or- by a native type.
    /// Spec. details, see http://www.json.org/
    internal sealed class JsonObjectDeserializer : JsonHybridDeserializer
    {
        private JsonRegistry _registry;
        private bool _useUtcDateTime;

        /// <summary>
        /// Creates a parser for the given json string. 
        /// </summary>
        /// <param name="ignorecase"></param>
        internal JsonObjectDeserializer(JsonParameters parameters)
            : base(parameters)
        {
            _registry = parameters.Registry;
            _useUtcDateTime = parameters.UseUtcDateTime;
        }

        internal Object Deserialize(ref String jsonString, Type targetType)
        {
            Object jsonValue = Deserialize(ref jsonString);

#if DESKTOP
            if (targetType != null && targetType == typeof(DataSet))
            {
                return CreateDataset((MutableJsonObject) jsonValue, null);
            }

            if (targetType != null && targetType == typeof(DataTable))
            {
                return CreateDataTable((MutableJsonObject) jsonValue, null);
            }
#endif

            MutableJsonObject asJsonObject = jsonValue as MutableJsonObject;
            if (asJsonObject != null)
            {
                if (targetType != null && targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) // deserialize a dictionary
                {
                    return CreateRootDictionary(asJsonObject, targetType);
                }
                else // deserialize an object
                {
                    return ParseDictionary(asJsonObject, null, targetType, null);
                }
            }
            
            MutableJsonObject asJsonArray = jsonValue as MutableJsonObject;
            if (asJsonArray != null)
            {
                if (targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) // kv format
                {
                    return CreateRootDictionary(asJsonArray, targetType);
                }

                if (targetType.GetGenericTypeDefinition() == typeof(List<>)) // deserialize to generic list
                {
                    return CreateRootList(jsonValue, targetType);
                }
                else
                {
                    return asJsonArray.ToArray();
                }
            }

            if (targetType != null && jsonValue.GetType() != targetType)
            {
                return Deserialization.ChangeType(jsonValue, targetType);
            }

            return jsonValue;
        }

        #region [   p r i v a t e   m e t h o d s   ]

        private object CreateRootList(object parse, Type type)
        {
            Type[] gtypes = type.GetGenericArguments();
            IList o = (IList)_registry.CreateInstanceFast(type);
            MutableJsonObject asObject;

            foreach (var k in (IList)parse)
            {
                object value = k;
                asObject = value as MutableJsonObject;

                if (asObject != null)
                {
                    value = ParseDictionary(asObject, null, gtypes[0], null);
                }
                else
                {
                    value = Deserialization.ChangeType(k, gtypes[0]);
                }

                o.Add(value);
            }
            return o;
        }

        private object CreateRootDictionary(MutableJsonArray jsonArray, Type type)
        {
            Type[] gtypes = type.GetGenericArguments();

            return CreateDictionary(jsonArray, type, gtypes, null);
        }
        
        private object CreateRootDictionary(MutableJsonObject jsonObject, Type type)
        {
            Type[] gtypes = type.GetGenericArguments();

                IDictionary o = (IDictionary)_registry.CreateInstanceFast(type);
                MutableJsonObject keyValueAsObject;
                MutableJsonArray keyValueAsArray;

                foreach (var kv in jsonObject)
                {
                    object v;
                    object k = Deserialization.ChangeType(kv.Key, gtypes[0]);
                    keyValueAsObject = kv.Value as MutableJsonObject;
                    
                    if (keyValueAsObject != null)
                    {
                        v = ParseDictionary(keyValueAsObject, null, gtypes[1], null);
                        continue;
                    }

                    keyValueAsArray = kv.Value as MutableJsonArray;
                    if (keyValueAsArray != null)
                    {
                        v = CreateArray(keyValueAsArray, typeof(object), typeof(object), null);
                    }
                    else
                    {
                        v = Deserialization.ChangeType(kv.Value, gtypes[1]);
                    }

                    o.Add(k, v);
                }

                return o;
        }

        private object ParseDictionary(MutableJsonObject source, Dictionary<string, object> globaltypes, Type targetType, object targetInstance)
        {
            object value;

            bool found = source.TryGetValue("$type", out value);

#if DESKTOP
            if (found == false && targetType == typeof(System.Object))
            {
                return CreateDataset(source, globaltypes);
            }
#endif

            if (found)
            {
                targetType = _registry.TypeDescriptor.GetType((string)value);
            }

            if (targetType == null)
            {
                throw new SerializationException("Cannot determine type");
            }

            string typename = targetType.FullName;

            if (targetInstance == null)
            {
                targetInstance = _registry.CreateInstanceFast(targetType);
            }

            var properties = _registry.GetPropertiesAndFields(targetType, typename);
            foreach (string n in source.Keys)
            {
                string name = n;

                if (name == "$map")
                {
                    ProcessMap(targetInstance, properties, (MutableJsonObject)source[name]);
                    continue;
                }

                JsonPropertyInfo pi;

                if (properties.TryGetValue(name, out pi) == false)
                {
                    continue;
                }

                if (pi.CanWrite)
                {
                    object genericValue = source[name];

                    if (genericValue != null)
                    {
                        object targetValue = null; // The value to be set on the target instance (potentially strongly typed)

                        if (pi.IsInt)
                            targetValue = (int)((long)genericValue);

                        else if (pi.IsCustomType)
                            targetValue = CreateCustom((string)genericValue, pi.PropertyOrFieldType);

                        else if (pi.IsLong)
                            targetValue = (long)genericValue;

                        else if (pi.IsString)
                            targetValue = (string)genericValue;

                        else if (pi.IsBool)
                            targetValue = (bool)genericValue;

                        else if (pi.IsEnum)
                            targetValue = Deserialization.CreateEnum(pi.PropertyOrFieldType, (string)genericValue);

                        else if (pi.IsDateTime)
                            targetValue = Deserialization.CreateDateTime((string)genericValue, pi.DateTimeKind);

                        else if (pi.IsGuid)
                            targetValue = Deserialization.CreateGuid((string)genericValue);
                        else if (pi.IsValueType)
                            targetValue = Deserialization.ChangeType(genericValue, pi.NullableType);
                        else if (pi.IsGenericType && pi.GenericItemType == typeof(byte))
                            targetValue = Deserialization.CreateByteArray((string) genericValue, pi.PropertyOrFieldType);

                        else if (pi.IsGenericType && pi.IsValueType == false && pi.IsDictionary == false)
                            targetValue = CreateGenericList((MutableJsonArray)genericValue, pi.PropertyOrFieldType, pi.GenericItemType, globaltypes);

                        else if (pi.IsByteArray)
                            targetValue = Convert.FromBase64String((string)genericValue);

                        else if (pi.IsArray && pi.IsValueType == false)
                            targetValue = CreateArray((MutableJsonArray)genericValue, pi.PropertyOrFieldType, pi.GenericItemType, globaltypes);
                        
#if DESKTOP
                        else if (pi.IsDataSet)
                            targetValue = CreateDataset((MutableJsonObject)genericValue, globaltypes);

                        else if (pi.IsDataTable)
                            targetValue = CreateDataTable((MutableJsonObject)genericValue, globaltypes);
#endif
                        else if (pi.IsStringDictionary)
                            targetValue = CreateStringKeyDictionary((MutableJsonObject)genericValue, pi.PropertyOrFieldType, pi.GenericTypes, globaltypes);

                        else if (pi.IsDictionary || pi.IsHashtable)
                            targetValue = CreateDictionary((MutableJsonArray)genericValue, pi.PropertyOrFieldType, pi.GenericTypes, globaltypes);

                        else if (pi.IsClass && genericValue is MutableJsonObject)
                            targetValue = ParseDictionary((MutableJsonObject)genericValue, globaltypes, pi.PropertyOrFieldType, pi.Getter(targetInstance));

                        else if (genericValue is MutableJsonArray)
                            targetValue = CreateArray((MutableJsonArray)genericValue, pi.PropertyOrFieldType, typeof(object), globaltypes);

                        else
                            targetValue = genericValue;

                        pi.Setter(targetInstance, targetValue);
                    }
                }
            }

            return targetInstance;
        }

        private object CreateCustom(string v, Type type)
        {
            DeserializationHandler d;

            _registry.CustomDeserializer.TryGetValue(type, out d);

            return d(v);
        }

        private void ProcessMap(object obj, RegistrySection<string, JsonPropertyInfo> properties, MutableJsonObject jsonObject)
        {
            foreach (KeyValuePair<string, object> kv in jsonObject)
            {
                JsonPropertyInfo p = properties[kv.Key];
                Object o = p.Getter(obj);
                Type t = Type.GetType((string)kv.Value);

                if (t == typeof(Guid))
                {
                    p.Setter(obj, Deserialization.CreateGuid((string)o));
                }
            }
        }

        private object CreateArray(MutableJsonArray data, Type arrayType, Type itemType, Dictionary<string, object> globalTypes)
        {
            Array col = Array.CreateInstance(itemType, data.Count);
            MutableJsonObject asObject;

            // create an array of objects
            for (int i = 0; i < data.Count; i++)// each (object ob in data)
            {
                object ob = data[i];
                asObject = ob as MutableJsonObject;

                if (asObject != null)
                {
                    col.SetValue(ParseDictionary(asObject, globalTypes, itemType, null), i);
                }
                else
                {
                    col.SetValue(Deserialization.ChangeType(ob, itemType), i);
                }
            }

            return col;
        }

        private object CreateGenericList(MutableJsonArray data, Type listType, Type listItemType, Dictionary<string, object> globalTypes)
        {
            IList result = (IList)_registry.CreateInstanceFast(listType);
            MutableJsonObject asObject;
            MutableJsonArray asArray;

            // create an array of objects
            foreach (object item in data)
            {
                asObject = item as MutableJsonObject;
                if (asObject != null)
                {
                    result.Add(ParseDictionary(asObject, globalTypes, listItemType, null));
                    continue;
                }

                asArray = item as MutableJsonArray;
                if (asArray != null)
                {
                    result.Add(asArray.ToArray());
                }
                else
                {
                    result.Add(Deserialization.ChangeType(item, listItemType));
                }
            }

            return result;
        }

        private object CreateStringKeyDictionary(MutableJsonObject reader, Type dictionaryType, Type[] types, Dictionary<string, object> globalTypes)
        {
            var result = (IDictionary)_registry.CreateInstanceFast(dictionaryType);
            Type tKey = null;
            Type tValue = null;

            if (types != null)
            {
                tKey = types[0];
                tValue = types[1];
            }

            MutableJsonObject valueAsObject;

            foreach (var values in reader)
            {
                var key = values.Key;//ChangeType(values.Key, t1);
                object val = null;

                valueAsObject = values.Value as MutableJsonObject;
                if (valueAsObject != null)
                {
                    val = ParseDictionary(valueAsObject, globalTypes, tValue, null);
                }
                else
                {
                    val = Deserialization.ChangeType(values.Value, tValue);
                }

                result.Add(key, val);
            }

            return result;
        }

        private object CreateDictionary(IList<object> reader, Type dictionaryType, Type[] types, Dictionary<string, object> globalTypes)
        {
            IDictionary result = (IDictionary)_registry.CreateInstanceFast(dictionaryType);
            Type tKey = null;
            Type tValue = null;

            if (types != null)
            {
                tKey = types[0];
                tValue = types[1];
            }

            MutableJsonObject typedKey;
            MutableJsonObject typedValue;

            foreach (MutableJsonObject values in reader)
            {
                object key = values["k"];
                object val = values["v"];

                typedKey = key as MutableJsonObject;
                if (typedKey != null)
                {
                    key = ParseDictionary(typedKey, globalTypes, tKey, null);
                }
                else
                {
                    key = Deserialization.ChangeType(key, tKey);
                }

                typedValue = val as MutableJsonObject;
                if (typedValue != null)
                {
                    val = ParseDictionary(typedValue, globalTypes, tValue, null);
                }
                else
                {
                    val = Deserialization.ChangeType(val, tValue);
                }

                result.Add(key, val);
            }

            return result;
        }

#if DESKTOP

        private void ReadSchema(MutableJsonObject reader, DataSet ds, Dictionary<string, object> globalTypes)
        {
            var schema = reader["$schema"];

            if (schema is string)
            {
                TextReader tr = new StringReader((string)schema);
                ds.ReadXmlSchema(tr);
            }
            else
            {
                DatasetSchema ms = (DatasetSchema)ParseDictionary((MutableJsonObject)schema, globalTypes, typeof(DatasetSchema), null);
                ds.DataSetName = ms.Name;
                for (int i = 0; i < ms.Info.Count; i += 3)
                {
                    if (ds.Tables.Contains(ms.Info[i]) == false)
                    {
                        ds.Tables.Add(ms.Info[i]);
                    }

                    ds.Tables[ms.Info[i]].Columns.Add(ms.Info[i + 1], Type.GetType(ms.Info[i + 2]));
                }
            }
        }

        private DataSet CreateDataset(MutableJsonObject reader, Dictionary<string, object> globalTypes)
        {
            DataSet ds = new DataSet();
            ds.EnforceConstraints = false;
            ds.BeginInit();

            // read dataset schema here
            ReadSchema(reader, ds, globalTypes);

            foreach (var pair in reader)
            {
                if (pair.Key == "$type" || pair.Key == "$schema") continue;

                List<object> rows = (List<object>)pair.Value;
                if (rows == null) continue;

                DataTable dt = ds.Tables[(String) pair.Key];
                ReadDataTable(rows, dt);
            }

            ds.EndInit();

            return ds;
        }

        private void ReadDataTable(IList<object> rows, DataTable dt)
        {
            dt.BeginInit();
            dt.BeginLoadData();

            List<int> guidcols = new List<int>();
            List<int> datecol = new List<int>();

            foreach (DataColumn c in dt.Columns)
            {
                if (c.DataType == typeof(Guid) || c.DataType == typeof(Guid?))
                {
                    guidcols.Add(c.Ordinal);
                }

                if (_useUtcDateTime && (c.DataType == typeof(DateTime) || c.DataType == typeof(DateTime?)))
                {
                    datecol.Add(c.Ordinal);
                }
            }

            foreach (List<object> row in rows)
            {
                object[] v = new object[row.Count];

                row.CopyTo(v, 0);

                foreach (int i in guidcols)
                {
                    string s = (string)v[i];
                    if (s != null && s.Length < 36)
                    {
                        v[i] = new Guid(Convert.FromBase64String(s));
                    }
                }

                if (_useUtcDateTime)
                {
                    foreach (int i in datecol)
                    {
                        string s = (string)v[i];
                        if (s != null)
                        {
                            v[i] = Deserialization.CreateDateTime(s, DateTimeKind.Utc);
                        }
                    }
                }

                dt.Rows.Add(v);
            }

            dt.EndLoadData();
            dt.EndInit();
        }

        private DataTable CreateDataTable(MutableJsonObject reader, Dictionary<string, object> globalTypes)
        {
            var dt = new DataTable();

            // read dataset schema here
            var schema = reader["$schema"];

            if (schema is string)
            {
                TextReader tr = new StringReader((string)schema);
                dt.ReadXmlSchema(tr);
            }
            else
            {
                var ms = (DatasetSchema)ParseDictionary((MutableJsonObject)schema, globalTypes, typeof(DatasetSchema), null);
                dt.TableName = ms.Info[0];

                for (int i = 0; i < ms.Info.Count; i += 3)
                {
                    dt.Columns.Add(ms.Info[i + 1], Type.GetType(ms.Info[i + 2]));
                }
            }

            foreach (var pair in reader)
            {
                if (pair.Key == "$type" || pair.Key == "$schema")
                {
                    continue;
                }

                var rows = (List<object>)pair.Value;

                if (rows == null)
                {
                    continue;
                }

                if (!dt.TableName.Equals((String) pair.Key, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                ReadDataTable(rows, dt);
            }

            return dt;
        }
#endif

        #endregion
    }
}
