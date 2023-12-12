using System;
using System.Collections;
using System.Collections.Generic;
#if DESKTOP
using System.Data;
#endif
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Apolyton.FastJson.Registry;
using Apolyton.FastJson.Data;

namespace Apolyton.FastJson
{
    /// <summary>
    /// Represents a json serializer.
    /// </summary>
    /// <remarks>
    /// http://www.codeproject.com/Articles/159450/fastJSON
    /// The version over there (2.0.9) could not be taken directly, as its serializer is taking all public properties, disregarding any attribute policy. This is
    /// not good for our case, as we want to return (portions of) data objects as well.
    /// </remarks>
    internal class JsonSerializer
    {
        /// <summary>
        /// The purpose of this dictionary is to reduce the runtime of the long if-else statement in the WriteValue method.
        /// </summary>
        private static readonly Dictionary<Type, Action<JsonSerializer, Object>> _serializerCache;

        private readonly JsonRegistry _registry;

        private StringBuilder _output = new StringBuilder();

        private readonly int _MAX_DEPTH = 10;
        private int _current_depth = 0;

        private bool _useFastGuid;
        private bool _useUtcDateTime;
        private bool _useTypeExtension;
        private bool _useOptimizedDatasetSchema;
        private bool _serializeNullValues;

        static JsonSerializer()
        {
            _serializerCache = new Dictionary<Type, Action<JsonSerializer, Object>>();
        }

        /// <summary>
        /// Creates an instance of the json serializer.
        /// </summary>
        /// <param name="param"></param>
        internal JsonSerializer(JsonParameters param)
        {
            _registry = param.Registry;

            _useFastGuid = param.UseFastGuid;
            _useUtcDateTime = param.UseUtcDateTime;
            _useTypeExtension = param.UseTypeExtension;
            _useOptimizedDatasetSchema = param.Serialization.UseOptimizedDatasetSchema;
            _serializeNullValues = param.Serialization.SerializeNullValues;
        }

        /// <summary>
        /// Returns the json string representing the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal string Serialize(object obj)
        {
            WriteValue(obj);

            return _output.ToString();
        }

        private void WriteValue(object obj)
        {
            Action<JsonSerializer, object> cachedSerializer;

            if (obj == null || obj is DBNull)
            {
                _output.Append("null");
            }

            else if (obj is string || obj is char)
            {
                WriteString(obj.ToString());
            }
            else if (obj is Guid)
            {
                WriteGuid((Guid)obj);
            }
            else if (obj is bool)
            {
                _output.Append(((bool)obj) ? "true" : "false"); // conform to standard
            }
            else if (
                obj is int || obj is long || obj is double ||
                obj is decimal || obj is float ||
                obj is byte || obj is short ||
                obj is sbyte || obj is ushort ||
                obj is uint || obj is ulong
            )
            {
                _output.Append(((IConvertible)obj).ToString(NumberFormatInfo.InvariantInfo));
            }
            else if (obj is DateTime)
            {
                WriteDateTime((DateTime)obj);
            }
            else if (obj is TimeSpan)
            {
                WriteTimeSpan((TimeSpan)obj);
            }
            else if (_serializerCache.TryGetValue(obj.GetType(), out cachedSerializer))
            {
                cachedSerializer(this, obj);
            }
            else if (obj is IDictionary)
            {
                if (obj.GetType().IsGenericType && obj.GetType().GetGenericArguments()[0] == typeof(string))
                {
                    _serializerCache.Add(obj.GetType(), (ser, arg) => ser.WriteStringDictionary((IDictionary)arg));
                    WriteStringDictionary((IDictionary)obj);
                }
                else
                {
                    _serializerCache.Add(obj.GetType(), (ser, arg) => ser.WriteDictionary((IDictionary)arg));
                    WriteDictionary((IDictionary)obj);
                }
            }
#if DESKTOP
            else if (obj is DataSet)
            {
                _serializerCache.Add(obj.GetType(), (ser, arg) => ser.WriteDataset((DataSet)arg));
                WriteDataset((DataSet)obj);
            }
            else if (obj is DataTable)
            {
                _serializerCache.Add(obj.GetType(), (ser, arg) => ser.WriteDataTable((DataTable)arg));
                WriteDataTable((DataTable)obj);
            }
#endif
            else if (obj is byte[])
            {
                _serializerCache.Add(obj.GetType(), (ser, arg) => ser.WriteBytes((byte[])arg));
                WriteBytes((byte[])obj);
            }
            else if (obj is IEnumerable)
            {
                // Just this fix for the byte enumeration makes 10 % performance drop.
                if (obj.GetType().IsGenericType && obj.GetType().GetGenericArguments()[0] == typeof(byte))
                {
                    _serializerCache.Add(obj.GetType(), (ser, arg) => ser.WriteBytes((IEnumerable<byte>)arg));
                    WriteBytes((IEnumerable<byte>)obj);
                }
                else
                {
                    _serializerCache.Add(obj.GetType(), (ser, arg) => ser.WriteArray((IEnumerable)arg));
                    WriteArray((IEnumerable)obj);
                }
            }
            else if (obj is Enum)
            {
                _serializerCache.Add(obj.GetType(), (ser, arg) => ser.WriteEnum((Enum)arg));
                WriteEnum((Enum)obj);
            }
            else if (_registry.IsCustomTypeRegistered(obj.GetType()))
            {
                _serializerCache.Add(obj.GetType(), (ser, arg) => ser.WriteCustomType(arg));
                WriteCustomType(obj);
            }
            else
            {
                WriteObject(obj);
            }
        }

        private void WriteCustomType(object obj)
        {
            SerializationHandler s;
            _registry.CustomSerializers.TryGetValue(obj.GetType(), out s);
            WriteStringFast(s(obj));
        }

        private void WriteEnum(Enum e)
        {
            // TODO : optimize enum write
            WriteStringFast(e.ToString());
        }

        private void WriteGuid(Guid g)
        {
            if (_useFastGuid == false)
            {
                WriteStringFast(g.ToString());
            }
            else
            {
                WriteBytes(g.ToByteArray());
            }
        }

        private void WriteBytes(byte[] bytes)
        {
            WriteStringFast(Convert.ToBase64String(bytes, 0, bytes.Length));
        }

        private void WriteBytes(IEnumerable<byte> bytes)
        {
            var byteArray = bytes as byte[];

            if (byteArray == null)
            {
                byteArray = bytes.ToArray();
            }

            WriteStringFast(Convert.ToBase64String(byteArray, 0, byteArray.Length));
        }

        private void WriteTimeSpan(TimeSpan timeSpan)
        {
            _output.Append("\"")
                    .Append(timeSpan.Hours.ToString("00", NumberFormatInfo.InvariantInfo))
                    .Append(":")
                    .Append(timeSpan.Minutes.ToString("00", NumberFormatInfo.InvariantInfo))
                    .Append(":")
                    .Append(timeSpan.Seconds.ToString("00", NumberFormatInfo.InvariantInfo))
                    .Append("\"");
        }

        private void WriteDateTime(DateTime dateTime)
        {
            // datetime format standard : yyyy-MM-dd HH:mm:ss
            DateTime dt = dateTime;

            if (_useUtcDateTime && dt.Kind == DateTimeKind.Local)
            {
                // Kind of unspecified or utc is not modified by this function; for unspecified we just expect that it is correct.
                dt = dateTime.ToUniversalTime();
            }

            _output.Append("\"")
                    .Append(dt.Year.ToString("0000", NumberFormatInfo.InvariantInfo))
                    .Append("-")
                    .Append(dt.Month.ToString("00", NumberFormatInfo.InvariantInfo))
                    .Append("-")
                    .Append(dt.Day.ToString("00", NumberFormatInfo.InvariantInfo))
                    .Append(" ")
                    .Append(dt.Hour.ToString("00", NumberFormatInfo.InvariantInfo))
                    .Append(":")
                    .Append(dt.Minute.ToString("00", NumberFormatInfo.InvariantInfo))
                    .Append(":")
                    .Append(dt.Second.ToString("00", NumberFormatInfo.InvariantInfo));

            if (_useUtcDateTime)
            {
                _output.Append("Z");
            }

            _output.Append("\"");
        }

#if DESKTOP

        private DatasetSchema GetSchema(DataTable ds)
        {
            if (ds == null) { return null; }

            DatasetSchema m = new DatasetSchema();
            m.Info = new List<string>();
            m.Name = ds.TableName;

            foreach (DataColumn c in ds.Columns)
            {
                m.Info.Add(ds.TableName);
                m.Info.Add(c.ColumnName);
                m.Info.Add(c.DataType.ToString());
            }
            // FEATURE : serialize relations and constraints here

            return m;
        }

        private DatasetSchema GetSchema(DataSet ds)
        {
            if (ds == null) { return null; }

            DatasetSchema m = new DatasetSchema();
            m.Info = new List<string>();
            m.Name = ds.DataSetName;

            foreach (DataTable t in ds.Tables)
            {
                foreach (DataColumn c in t.Columns)
                {
                    m.Info.Add(t.TableName);
                    m.Info.Add(c.ColumnName);
                    m.Info.Add(c.DataType.ToString());
                }
            }
            // FEATURE : serialize relations and constraints here

            return m;
        }

        private string GetXmlSchema(DataTable dt)
        {
            using (var writer = new StringWriter())
            {
                dt.WriteXmlSchema(writer);

                return dt.ToString();
            }
        }

        private void WriteDataset(DataSet ds)
        {
            _output.Append('{');

            if (_useTypeExtension)
            {
                WritePair("$schema", _useOptimizedDatasetSchema ? (object)GetSchema(ds) : ds.GetXmlSchema());
                _output.Append(',');
            }

            bool tablesep = false;
            foreach (DataTable table in ds.Tables)
            {
                if (tablesep) _output.Append(",");
                tablesep = true;
                WriteDataTableData(table);
            }

            // end dataset
            _output.Append('}');
        }

        private void WriteDataTableData(DataTable table)
        {
            _output.Append('\"')
                    .Append(table.TableName)
                    .Append("\":[");

            DataColumnCollection cols = table.Columns;
            bool rowseparator = false;

            foreach (DataRow row in table.Rows)
            {
                if (rowseparator) _output.Append(",");
                rowseparator = true;
                _output.Append('[');

                bool pendingSeperator = false;
                foreach (DataColumn column in cols)
                {
                    if (pendingSeperator) _output.Append(',');
                    WriteValue(row[column]);
                    pendingSeperator = true;
                }
                _output.Append(']');
            }


            _output.Append(']');
        }

        private void WriteDataTable(DataTable dt)
        {
            this._output.Append('{');

            if (_useTypeExtension)
            {
                this.WritePair("$schema", _useOptimizedDatasetSchema ? (object)this.GetSchema(dt) : this.GetXmlSchema(dt));
                this._output.Append(',');
            }

            WriteDataTableData(dt);

            // end datatable
            this._output.Append('}');
        }

#endif

        private void WriteObject(object source)
        {
            _output.Append('{');

            _current_depth++;

            if (_current_depth > _MAX_DEPTH)
            {
                throw new SerializationException("Serializer encountered maximum depth of " + _MAX_DEPTH);
            }

            Type sourceType = source.GetType();

            if (_useTypeExtension)
            {
                WritePairFast("$type", _registry.TypeDescriptor.GetTypeName(sourceType));
            }

            IEnumerable<GetterDescriptor> getters = _registry.GetGetters(sourceType);
            int count = getters.Count();
            int i = 0;

            if (count == 0)
            {
                throw new SerializationException(String.Format(
                    "Failed to serialize object with type '{0}' since no useful information has been found on it. " +
                    "Consider implementing a custom serializer, adding the IgnoreDataMember attribute on the related property or increase the visibility scope of ." +
                    "data properties of the type", sourceType));
            }

            if (_useTypeExtension) // count $type as a property
            {
                i++;
            }

            foreach (var descriptor in getters)
            {
                object value = null;

                try
                {
                    value = descriptor.Getter(source);
                }
                catch (InvalidProgramException)
                {
                }
                catch (TypeAccessException e)
                {
                    throw new SerializationException(String.Format(
                        "Could not read value from property '{0}' on type '{1}'. \n\nThis is likely because the property getter/ setter of '{0}' are not public. ",
                        descriptor.Name, sourceType), e);
                }

                if (_serializeNullValues == false && (value == null || value is DBNull || value.Equals(descriptor.DefaultValue)))
                {
                    continue;
                }
                else
                {
                    if (i != 0)  // if not first, append the separator.
                    {
                        _output.Append(",");
                    }

                    WritePair(descriptor.Name, value);

                    i++; // Only increment i, if we have written a value. 
                }
            }

            _current_depth--;
            _output.Append('}');
            _current_depth--;
        }

        private void WritePairFast(string name, string value)
        {
            if ((value == null) && _serializeNullValues == false)
            {
                return;
            }

            WriteStringFast(name);

            _output.Append(':');

            WriteStringFast(value);
        }

        private void WritePair(string name, object value)
        {
            if ((value == null || value is DBNull) && _serializeNullValues == false)
            {
                return;
            }

            WriteStringFast(name);

            _output.Append(':');

            WriteValue(value);
        }

        private void WriteArray(IEnumerable array)
        {
            _output.Append('[');

            bool pendingSeperator = false;

            foreach (object obj in array)
            {
                if (pendingSeperator) _output.Append(',');

                WriteValue(obj);

                pendingSeperator = true;
            }

            _output.Append(']');
        }

        private void WriteStringDictionary(IDictionary dic)
        {
            _output.Append('{');

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) _output.Append(',');

                WritePair((string)entry.Key, entry.Value);

                pendingSeparator = true;
            }

            _output.Append('}');
        }

        private void WriteDictionary(IDictionary dic)
        {
            _output.Append('[');

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) { _output.Append(','); }

                _output.Append('{');
                WritePair("k", entry.Key);

                _output.Append(",");
                WritePair("v", entry.Value);

                _output.Append('}');
                pendingSeparator = true;
            }

            _output.Append(']');
        }

        /// <summary>
        /// Writes a string without masking it.
        /// </summary>
        /// <param name="s"></param>
        private void WriteStringFast(string s)
        {
            _output.Append('\"');
            _output.Append(s);
            _output.Append('\"');
        }

        /// <summary>
        /// Writes a masked string (may contain multiple lines etc);
        /// </summary>
        /// <param name="s"></param>
        private void WriteString(string s)
        {
            _output.Append('\"');

            int runIndex = -1;

            for (var index = 0; index < s.Length; ++index)
            {
                var c = s[index];

                if (c >= ' ' && c < 128 && c != '\"' && c != '\\')
                {
                    if (runIndex == -1)
                    {
                        runIndex = index;
                    }

                    continue;
                }

                if (runIndex != -1)
                {
                    _output.Append(s, runIndex, index - runIndex);
                    runIndex = -1;
                }

                switch (c)
                {
                    case '\t': _output.Append("\\t"); break;
                    case '\r': _output.Append("\\r"); break;
                    case '\n': _output.Append("\\n"); break;
                    case '"':
                    case '\\': _output.Append('\\'); _output.Append(c); break;
                    default:
                        _output.Append("\\u");
                        _output.Append(((int)c).ToString("X4", NumberFormatInfo.InvariantInfo));
                        break;
                }
            }

            if (runIndex != -1)
            {
                _output.Append(s, runIndex, s.Length - runIndex);
            }

            _output.Append('\"');
        }
    }
}
