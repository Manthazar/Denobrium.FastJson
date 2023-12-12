using System;
using System.Diagnostics;
using System.Linq;
using Apolyton.FastJson.Registry;
using Apolyton.FastJson.Serialization;
using System.Globalization;
using Apolyton.FastJson.Common;

namespace Apolyton.FastJson.Data
{
    /// <summary>
    /// Represents a json primitive value.
    /// </summary>
    public sealed class JsonPrimitive : IJsonValue, IConvertible
    {
        private readonly JsonRegistry _registry;
        private readonly String _localValue;
        private object _typedValue;
        private DeserializationHandler _deserializationHandler;

        /// <summary>
        /// Defines the Info property for this json primitive.
        /// </summary>
        internal JsonPropertyInfo FieldInfo;

        #region Internal Constructors

        /// <summary>
        /// Creates an instance of the type.
        /// </summary>
        /// <param name="localValue">The string representation of the Json Primitive.</param>
        ///<remarks>Primarily used for unit tests.</remarks>
        internal JsonPrimitive(String localValue)
        {
            this._deserializationHandler = null;
            this._registry = null;
            this._localValue = localValue;
            this._typedValue = null;

            this.FieldInfo = JsonPropertyInfo.Empty;
        }

        /// <summary>
        /// Creates an instance of the type.
        /// </summary>
        /// <param name="boolean"></param>
        internal JsonPrimitive(bool boolean)
        {
            this._deserializationHandler = null;
            this._registry = null;
            this._localValue = null;
            this._typedValue = boolean;

            this.FieldInfo = JsonPropertyInfo.Empty;
        }

        /// <summary>
        /// Creates an instance of the primitive.
        /// </summary>
        /// <param name="localValue">The string representation of the Json Primitive.</param>
        /// <param name="registry">The registry which is used to resolve the custom deserializer, if required.</param>
        internal JsonPrimitive(String localValue, JsonRegistry registry)
        {
            this._deserializationHandler = null;
            this._registry = registry;
            this._localValue = localValue;
            this._typedValue = null;

            this.FieldInfo = JsonPropertyInfo.Empty;
        }

        #endregion

        /// <summary>
        /// Public helper method to convert the given string value with the primitives built in mechanism into the target type.
        /// </summary>
        /// <param name="value">The string value: eg. "15" "01.11.1988" etc.</param>
        /// <param name="targetType">The target primitive (!) type. Complex types (=classes) not supported.</param>
        /// <returns></returns>
        /// <exception cref="FormatException">If the value has the wrong format to be converted into the target type.</exception>
        public static object Convert(String value, Type targetType)
        {
            Guard.ArgumentNotNull(targetType, "targetType");

            if (String.IsNullOrEmpty(value))
            {
                return null;
            }

            var intermediateValue = new JsonPrimitive(value, Json.Current.DefaultParameters.Registry);
#if DESKTOP
            return System.Convert.ChangeType(intermediateValue, targetType);
#else 
            return System.Convert.ChangeType(intermediateValue, targetType, null);
#endif
        }

        /// <summary>
        /// Gets the json primitive type.
        /// </summary>
        public JsonType Type
        {
            get { return JsonType.JsonPrimitive; }
        }

        /// <summary>
        /// Returns json primitive.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_localValue != null)
            {
                return String.Format("JsonPrimitive ({0})", _localValue.ToString());
            }
            else
            {
                return "JsonPrimitive (<null>)";
            }
        }

        #region Explicit operators

        /// <summary>
        /// Returns an int representing the json primitive.
        /// </summary>
        /// <param name="primitive"></param>
        /// <returns></returns>
        public static explicit operator int(JsonPrimitive primitive)
        {
            return ((IConvertible)primitive).ToInt32(null);
        }

        /// <summary>
        /// Returns an int representing the json primitive.
        /// </summary>
        /// <param name="primitive"></param>
        /// <returns></returns>
        public static explicit operator short(JsonPrimitive primitive)
        {
            return ((IConvertible)primitive).ToInt16(null);
        }

        /// <summary>
        /// Returns an string representing the json primitive.
        /// </summary>
        /// <param name="primitive"></param>
        /// <returns></returns>
        public static explicit operator String(JsonPrimitive primitive)
        {
            if (primitive._typedValue == null)
            {
                primitive._typedValue = Deserialization.ChangeType(primitive._localValue, typeof(String));
            }

            return (string)primitive._typedValue;
        }

        /// <summary>
        /// Returns an bool representing the json primitive.
        /// </summary>
        /// <param name="primitive"></param>
        /// <returns></returns>
        public static explicit operator bool(JsonPrimitive primitive)
        {
            return ((IConvertible)primitive).ToBoolean(null);
        }

        /// <summary>
        /// Returns an double representing the json primitive.
        /// </summary>
        /// <param name="primitive"></param>
        /// <returns></returns>
        public static explicit operator double(JsonPrimitive primitive)
        {
            return ((IConvertible)primitive).ToDouble(null);
        }

        /// <summary>
        /// Returns an double representing the json primitive.
        /// </summary>
        /// <param name="primitive"></param>
        /// <returns></returns>
        public static explicit operator float(JsonPrimitive primitive)
        {
            return ((IConvertible)primitive).ToSingle(null);
        }

        /// <summary>
        /// Returns an double representing the json primitive.
        /// </summary>
        /// <param name="primitive"></param>
        /// <returns></returns>
        public static explicit operator DateTime(JsonPrimitive primitive)
        {
            return ((IConvertible)primitive).ToDateTime(null);
        }

        /// <summary>
        /// Returns an timespan representing the json primitive.
        /// </summary>
        /// <param name="primitive"></param>
        /// <returns></returns>
        public static explicit operator TimeSpan(JsonPrimitive primitive)
        {
            if (primitive._typedValue == null)
            {
                primitive._typedValue = TimeSpan.Parse(primitive._localValue);
            }

            return (TimeSpan)primitive._typedValue;
        }

        /// <summary>
        /// Returns an guid representing the json primitive.
        /// </summary>
        /// <param name="primitive"></param>
        /// <returns></returns>
        public static explicit operator Guid(JsonPrimitive primitive)
        {
            if (primitive._typedValue == null)
            {
                primitive._typedValue = Deserialization.CreateGuid(primitive._localValue);
            }

            return (Guid)primitive._typedValue;
        }

        /// <summary>
        /// Returns a long representing the json primitive.
        /// </summary>
        /// <param name="primitive"></param>
        /// <returns></returns>
        public static explicit operator long(JsonPrimitive primitive)
        {
            return ((IConvertible)primitive).ToInt64(null);
        }

        #endregion

        #region Accessor Members

        /// <summary>
        /// Gets the value as string.
        /// </summary>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        public string AsString
        {
            get { return (String)this; }
        }

        /// <summary>
        /// Gets the value as integer.
        /// </summary>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        public int AsInteger
        {
            get { return (int)this; }
        }


        /// <summary>
        /// Gets the value as bool.
        /// </summary>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        public bool AsBool
        {
            get { return (bool)this; }
        }

        /// <summary>
        /// Gets the value as double.
        /// </summary>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        public Double AsDouble
        {
            get { return (Double)this; }
        }

        /// <summary>
        /// Gets the value as float.
        /// </summary>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        public float AsFloat
        {
            get { return (float)this; }
        }

        /// <summary>
        /// Gets the value as date time.
        /// </summary>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        public DateTime AsDateTime
        {
            get { return (DateTime)this; }
        }

        /// <summary>
        /// Gets the value as time span.
        /// </summary>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        public TimeSpan AsTimeSpan
        {
            get { return (TimeSpan)this; }
        }

        /// <summary>
        /// Gets the value as guid.
        /// </summary>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        public Guid AsGuid
        {
            get { return (Guid)this; }
        }

        /// <summary>
        /// Gets the value as long.
        /// </summary>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        public long AsLong
        {
            get { return (long)this; }
        }

        /// <summary>
        /// Gets the value as single.
        /// </summary>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        public float AsSingle
        {
            get { return (float)this; }
        }

        /// <summary>
        /// Gets the value as short.
        /// </summary>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        public short AsShort
        {
            get { return (short)this; }
        }

        #endregion

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Boolean | TypeCode.Char | TypeCode.DateTime | TypeCode.Decimal | TypeCode.Double | TypeCode.Int16 | TypeCode.Int32 | TypeCode.Int64
                | TypeCode.String | TypeCode.UInt16 | TypeCode.UInt32 | TypeCode.UInt64;
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                _typedValue = bool.Parse(_localValue);
            }

            return (bool)_typedValue;
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                _typedValue = _localValue.ToCharArray().Single();
            }

            return (char)_typedValue;
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                if (FieldInfo != JsonPropertyInfo.Empty)
                {
                    _typedValue = Deserialization.CreateDateTime(_localValue, FieldInfo.DateTimeFormat, FieldInfo.DateTimeKind);
                }
                else
                {
                    _typedValue = Deserialization.CreateDateTime(_localValue, DateTimeKind.Unspecified);
                }
            }

            return (DateTime)_typedValue;
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                _typedValue = decimal.Parse(_localValue);
            }

            return (decimal)_typedValue;
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                _typedValue = double.Parse(_localValue);
            }

            return (Double)_typedValue;
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                _typedValue = Int16.Parse(_localValue);
            }

            return (short)_typedValue;
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                _typedValue = Int32.Parse(_localValue);
            }

            return (int)_typedValue;
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                _typedValue = Deserialization.CreateLong(_localValue);
            }

            return (long)_typedValue;
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                _typedValue = float.Parse(_localValue);
            }

            return (float)_typedValue;
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return (String)this;
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(Guid))
            {
                return (Guid)this;
            }
            else if (conversionType == typeof(TimeSpan))
            {
                return (TimeSpan)this;
            }
            else if (conversionType.IsEnum)
            {
                if (_typedValue != null) { return _typedValue; }
                else { _typedValue = Deserialization.CreateEnum(conversionType, _localValue); return _typedValue; }
            }
            else if (conversionType == typeof(byte[]))
            {
                if (_typedValue != null) { return _typedValue; }
                else { _typedValue = Deserialization.CreateByteArray(_localValue, typeof(byte[])); return _typedValue; }
            }
            else if (_registry != null && _registry.CustomDeserializer.TryGetValue(conversionType, out _deserializationHandler))
            {
                if (_typedValue != null) { return _typedValue; }
                else { _typedValue = _deserializationHandler(_localValue); return _typedValue; }
            }
            else if (conversionType == typeof(int?))
            {
                if (_localValue != null) { return ((IConvertible)this).ToInt32(null); }
                else { return null; }
            }
            else if (conversionType == typeof(double?))
            {
                if (_localValue != null) { return ((IConvertible)this).ToDouble(null); }
                else { return null; }
            }
            else if (conversionType == typeof(Guid?))
            {
                if (_localValue != null) { return (Guid)this; }
                else { return null; }
            }
            else if (conversionType == typeof(short?))
            {
                if (_localValue != null) { return ((IConvertible)this).ToInt16(null); }
                else { return null; }
            }
            else if (conversionType == typeof(long?))
            {
                if (_localValue != null) { return ((IConvertible)this).ToInt64(null); }
                else { return null; }
            }
            else if (conversionType == typeof(DateTime?))
            {
                if (_localValue != null) { return ((IConvertible)this).ToDateTime(null); }
                else { return null; }
            }
            else if (conversionType == typeof(TimeSpan?))
            {
                if (_localValue != null) { return (TimeSpan)this; }
                else { return null; }
            }
            else if (conversionType == typeof(float?))
            {
                if (_localValue != null) { return ((IConvertible)this).ToSingle(null); }
                else { return null; }
            }
            else if (conversionType == typeof(uint?))
            {
                if (_localValue != null) { return ((IConvertible)this).ToUInt32(null); }
                else { return null; }
            }
            else if (conversionType == typeof(ushort?))
            {
                if (_localValue != null) { return ((IConvertible)this).ToUInt16(null); }
                else { return null; }
            }
            else if (conversionType == typeof(ulong?))
            {
                if (_localValue != null) { return ((IConvertible)this).ToUInt64(null); }
                else { return null; }
            }
            else if (conversionType == typeof(decimal?))
            {
                if (_localValue != null) { return ((IConvertible)this).ToDecimal(null); }
                else { return null; }
            }
            else
            {
                throw new NotSupportedException("Cannot convert json primitive to: " + conversionType.ToString());
            }
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                _typedValue = ushort.Parse(_localValue);
            }

            return (ushort)_typedValue;
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                _typedValue = UInt32.Parse(_localValue);
            }

            return (UInt32)_typedValue;
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            if (_typedValue == null)
            {
                _typedValue = UInt64.Parse(_localValue);
            }

            return (UInt64)_typedValue;
        }

        #endregion

        #region Not supported members

        int IJsonValue.Count
        {
            get { throw new NotSupportedException(); }
        }

        IJsonValue IJsonValue.this[int i]
        {
            get { throw new NotSupportedException(); }
        }

        IJsonValue IJsonValue.this[string key]
        {
            get { throw new NotSupportedException(); }
        }

        #endregion
    }
}
