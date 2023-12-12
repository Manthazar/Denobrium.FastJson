using System;
using Denobrium.Json.Registry;

namespace Denobrium.Json.Registry
{
    /// <summary>
    /// Represents the internal property info.
    /// </summary>
    internal struct JsonPropertyInfo
    {
        /// <summary>
        /// The empty instance.
        /// </summary>
        internal static JsonPropertyInfo Empty = new JsonPropertyInfo();

        /// <summary>
        /// Specifies the type of the member.
        /// </summary>
        public Type PropertyOrFieldType;

        /// <summary>
        /// Specifies the declaring type of the member.
        /// </summary>
        public Type DeclaringType;

        /// <summary>
        /// Represents the non-nullable value behind the primary type. Normally, a cast of a value to the primary type should be possible.
        /// </summary>
        /// <example>
        /// typeof(Double?) for instance is Nullable{double} with NullableType = typeof(double).
        /// </example>
        public Type NullableType;

        /// <summary>
        /// Represents the first item type for generic enumerations and collections (relevant for performance).
        /// </summary>
        public Type GenericItemType;

        /// <summary>
        /// Represents all generic types of the Primary Type.
        /// </summary>
        public Type[] GenericTypes;

        /// <summary>
        /// Specifies the property of field name.
        /// </summary>
        public string PropertyOrFieldName;

        /// <summary>
        /// Specifies the json field name for the given property.
        /// </summary>
        public string JsonFieldName;

        /// <summary>
        /// True, if member type is a dictionary.
        /// </summary>
        public bool IsDictionary;

        /// <summary>
        /// Determines, if member type is value type.
        /// </summary>
        public bool IsValueType;

        /// <summary>
        /// Determines, if the type is a generic type.
        /// </summary>
        public bool IsGenericType;

        /// <summary>
        /// Determines, if the member type is an array.
        /// </summary>
        public bool IsArray;

        /// <summary>
        /// Determines, if the member type is a byte array.
        /// </summary>
        public bool IsByteArray;

        /// <summary>
        /// Detemines, if the member type is a guid.
        /// </summary>
        public bool IsGuid;

        /// <summary>
        /// Determines, if the member type is a data set.
        /// </summary>
        public bool IsDataSet;

        /// <summary>
        /// Determines, if the member type is a data table.
        /// </summary>
        public bool IsDataTable;

        /// <summary>
        /// Determines, if the member type is a hash table.
        /// </summary>
        public bool IsHashtable;

        /// <summary>
        /// Determines, if the member type is a hash set.
        /// </summary>
        /// <remarks>
        /// Hash Set is in the way specific that it doesn't implement IList or ICollection AND it is always generic so it cannot be populated during deserialization
        /// directly.
        /// </remarks>
        public bool IsHashSet;

        /// <summary>
        /// Determines, if the member type is an Enum.
        /// </summary>
        public bool IsEnum;

        /// <summary>
        /// Determines, if the member type is a date time.
        /// </summary>
        public bool IsDateTime;

        /// <summary>
        /// Determines the date time format a particular field is supposed to be implemented.
        /// </summary>
        public String DateTimeFormat;

        /// <summary>
        /// Determines, if the date kind of the member. Relevant for deserialization to now, if time zone adjustment is desired or not.
        /// Defined through the JsonDateTimeOptions attribute.
        /// </summary>
        public DateTimeKind DateTimeKind;

        /// <summary>
        /// Determines, if the member type is an int.
        /// </summary>
        public bool IsInt;

        /// <summary>
        /// Determines, if the member type is a long.
        /// </summary>
        public bool IsLong;

        /// <summary>
        /// Determines, if the member type is a string.
        /// </summary>
        public bool IsString;

        /// <summary>
        /// Determines, if the member type is a bool.
        /// </summary>
        public bool IsBool;

        /// <summary>
        /// Determines, if the member type is a class.
        /// </summary>
        public bool IsClass;

        /// <summary>
        /// Determines, if the member type is a string dictionary.
        /// </summary>
        public bool IsStringDictionary;

        /// <summary>
        /// Represents a fast set function for the property or field.
        /// </summary>
        public GenericSetterHandler Setter;

        /// <summary>
        /// Represents a fast get function for the property or field.
        /// </summary>
        public GenericGetterHandler Getter;

        /// <summary>
        /// Represents a fast construct method using the default constructor of the property of field type.
        /// </summary>
        internal CreateObjectHandler Constructor;

        /// <summary>
        /// Determines, if the member type is a custom type.
        /// </summary>
        public bool IsCustomType;

        /// <summary>
        /// Determines, if the member type can be written to.
        /// </summary>
        public bool CanWrite;

        /// <summary>
        /// Specifies the default value of the property. For nullable types it is 'null', for other types determined throught the activator.
        /// </summary>
        public object DefaultValue;

        /// <summary>
        /// Returns true, if the references are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(JsonPropertyInfo a, JsonPropertyInfo b)
        {
            return Object.ReferenceEquals(a, b);
        }

        /// <summary>
        /// Returns true, if the references are not equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(JsonPropertyInfo a, JsonPropertyInfo b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns the hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns true, if the objects are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        private void CopyFrom(JsonPropertyInfo info)
        {
            CanWrite = info.CanWrite;
            DateTimeFormat = info.DateTimeFormat;
            DateTimeKind = info.DateTimeKind;
            DeclaringType = info.DeclaringType;
            DefaultValue = info.DefaultValue;
            GenericItemType = info.GenericItemType;
            GenericTypes = info.GenericTypes;
            Getter = info.Getter;

            IsArray = info.IsArray;
            IsBool = info.IsBool;
            IsByteArray = info.IsByteArray;
            IsClass = info.IsClass;
            IsCustomType = info.IsCustomType;
#if DESKTOP
            IsDataSet = info.IsDataSet;
            IsDataTable = info.IsDataTable;
            IsHashtable = info.IsHashtable;
#endif
            IsDateTime = info.IsDateTime;
            IsDictionary = info.IsDictionary;
            IsEnum = info.IsEnum;
            IsGenericType = info.IsGenericType;
            IsGuid = info.IsGuid;
            IsHashSet = info.IsHashSet;
            IsInt = info.IsInt;
            IsLong = info.IsLong;
            IsString = info.IsString;
            IsStringDictionary = info.IsStringDictionary;
            IsValueType = info.IsValueType;
            JsonFieldName = info.JsonFieldName;
            NullableType = info.NullableType;
            PropertyOrFieldName = info.PropertyOrFieldName;
            PropertyOrFieldType = info.PropertyOrFieldType;
            Setter = info.Setter;
            Constructor = info.Constructor;
        }

        internal JsonPropertyInfo Clone()
        {
            JsonPropertyInfo info = new JsonPropertyInfo();
            info.CopyFrom(this);
            return info;
        }

        ///// <summary>
        ///// Returns a new instance of the type described by this info.
        ///// </summary>
        ///// <returns></returns>
        //internal object NewInstance()
        //{
        //    return Constructor();
        //}

        /// <summary>
        /// Returns a lightweight object, which describes the serialization criteria.
        /// </summary>
        /// <returns></returns>
        internal JsonSerializationInfo ToSerializationInfo()
        {
            JsonSerializationInfo info = new JsonSerializationInfo();
            info.JsonFieldName = JsonFieldName;
            info.MemberName = PropertyOrFieldName;
            info.MemberType = PropertyOrFieldType;
            info.DeclaringType = DeclaringType;
            info.MemberDisplayName = info.MemberName;

            if (GenericTypes == null)
            {
                info.MemberDisplayType = PropertyOrFieldType.Name;
            }
            else
            {
                info.MemberDisplayType = String.Format("{0}<{1}>", PropertyOrFieldType.Name, GenericTypes[0].Name);
            }

            return info;
        }

        /// <summary>
        /// Returns a more meaningful string representing the info object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}: {1}", PropertyOrFieldName, PropertyOrFieldType);
        }
    }
}
