using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Serialization
{
    /// <summary>
    /// This class contains deserialization helpers 
    /// </summary>
    internal static class Deserialization
    {
        private static HashSet<Type> internallyManagedTypes;

        static Deserialization()
        {
            internallyManagedTypes = new HashSet<Type>();
            internallyManagedTypes.Add(typeof(DBNull));
            internallyManagedTypes.Add(typeof(string));
            internallyManagedTypes.Add(typeof(char));
            internallyManagedTypes.Add(typeof(Guid));
            internallyManagedTypes.Add(typeof(bool));
            internallyManagedTypes.Add(typeof(int));
            internallyManagedTypes.Add(typeof(long));
            internallyManagedTypes.Add(typeof(double));
            internallyManagedTypes.Add(typeof(decimal));
            internallyManagedTypes.Add(typeof(float));
            internallyManagedTypes.Add(typeof(byte));
            internallyManagedTypes.Add(typeof(short));
            internallyManagedTypes.Add(typeof(sbyte));
            internallyManagedTypes.Add(typeof(ushort));
            internallyManagedTypes.Add(typeof(uint));
            internallyManagedTypes.Add(typeof(ulong));
            internallyManagedTypes.Add(typeof(DateTime));
            internallyManagedTypes.Add(typeof(TimeSpan));
            internallyManagedTypes.Add(typeof(IDictionary));
            internallyManagedTypes.Add(typeof(byte[]));
            internallyManagedTypes.Add(typeof(IEnumerable));
            internallyManagedTypes.Add(typeof(Enum));

#if DESKTOP
            internallyManagedTypes.Add(typeof(System.Data.DataSet));
            internallyManagedTypes.Add(typeof(System.Data.DataTable));
#endif
        }

        /// <summary>
        /// Returns true, if the given type is supported by FastJson itself.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsTypeInternallyManaged(Type type)
        {
            return internallyManagedTypes.Contains(type);
        }

        /// <summary>
        /// Changes the value type to the expected one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="convertTo"></param>
        /// <returns></returns>
        internal static object ChangeType(object value, Type convertTo)
        {
            if (convertTo == typeof(int))
                return (int)((long)value);

            else if (convertTo == typeof(long))
                return (long)value;

            else if (convertTo == typeof(string))
                return (string)value;

            else if (convertTo == typeof(Guid))
                return CreateGuid((string)value);

            else if (convertTo == typeof(TimeSpan))
                return CreateTimeSpan((String)value);

            else if (convertTo.IsEnum)
                return CreateEnum(convertTo, (string)value);

            return Convert.ChangeType(value, convertTo, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns an enum value for the given type.
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enumString"></param>
        /// <returns></returns>
        internal static object CreateEnum(Type enumType, string enumString)
        {
            // TODO : optimize create enum
            return Enum.Parse(enumType, enumString, true);
        }

        /// <summary>
        /// Returns a guid for the given string.
        /// </summary>
        /// <param name="base64OrGuidString"></param>
        /// <returns></returns>
        internal static Guid CreateGuid(string base64OrGuidString)
        {
            if (base64OrGuidString.Length > 30)
            {
                return new Guid(base64OrGuidString);
            }
            else
            {
                return new Guid(Convert.FromBase64String(base64OrGuidString));
            }
        }

        /// <summary>
        /// Returns a time span for the given string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static TimeSpan CreateTimeSpan(String s)
        {
            return TimeSpan.Parse(s);
        }

        /// <summary>
        /// Returns the parsed date time.
        /// </summary>
        /// <returns></returns>
        internal static DateTime CreateDateTime(string value, String format, DateTimeKind expectedKind)
        {
            if (String.IsNullOrEmpty(format))
            {
                return CreateDateTime(value, expectedKind);
            }
            else
            {
                DateTime result = DateTime.ParseExact(value, format, null);
                result = DateTime.SpecifyKind(result, expectedKind);

                return result;
            }
        }

        /// <summary>
        /// Returns the parsed date time.
        /// </summary>
        /// <returns></returns>
        internal static DateTime CreateDateTime(string value, DateTimeKind expectedKind)
        {
            //                   0123456789012345678
            // datetime format = yyyy-MM-dd HH:mm:ss
            int year = (int)CreateLong(value.Substring(0, 4));
            int month = (int)CreateLong(value.Substring(5, 2));
            int day = (int)CreateLong(value.Substring(8, 2));
            int hour = (int)CreateLong(value.Substring(11, 2));
            int min = (int)CreateLong(value.Substring(14, 2));
            int sec = (int)CreateLong(value.Substring(17, 2));

            if (!value.EndsWith("Z"))
            {
                return new DateTime(year, month, day, hour, min, sec, expectedKind);
            }
            else
            {
                if (expectedKind == DateTimeKind.Local)
                {
                    // date string is in utc: convert to local time.
                    return new DateTime(year, month, day, hour, min, sec, DateTimeKind.Utc).ToLocalTime();
                }
                else 
                {
                    return new DateTime(year, month, day, hour, min, sec, DateTimeKind.Utc);
                }
            }
        }

        /// <summary>
        /// Returns a long value for the given string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static long CreateLong(string s)
        {
            long result = 0;
            bool isNegative = false;

            foreach (char cc in s)
            {
                if (cc == '-')
                {
                    isNegative = true;
                }
                else if (cc == '+')
                {
                    isNegative = false;
                }
                else
                {
                    result *= 10;
                    result += (int)(cc - '0');
                }
            }

            return isNegative ? -result : result;
        }

        internal static IEnumerable<Byte> CreateByteArray(string base64ByteString, Type targetType)
        {
            if (targetType.IsArray || typeof(IEnumerable).IsAssignableFrom(targetType))
            {
                return Convert.FromBase64String(base64ByteString);
            }

            throw new SerializationException("Cannot create instance for type " + targetType);
        }
    }
}
