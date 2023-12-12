using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Denobrium.Json.Tests.Helpers;
using Denobrium.Json.TestsHelpers.StandardTypes;
using Denobrium.Json;
using Denobrium.Json.Data;
using System.Collections.Generic;

namespace Denobrium.Json.Tests
{
    [TestClass]
    public class JsonTests
    {
        #region JsonObject Method Tests

        [TestMethod]
        public void Json_ReadObject_IntClass()
        {
            String jsonString = "{\"Integer\":1,\"NullableInteger\":2}";

            var result = (IntClass)Json.Current.ReadObject(jsonString, typeof(IntClass));
            Assert.AreEqual(1, result.Integer);
            Assert.AreEqual(2, result.NullableInteger);
        }

        [TestMethod]
        public void Json_ReadObject_IgnoreCaseOnDeserialize()
        {
            Assert.Inconclusive("Not supported yet");

            String jsonString = "{\"integer\":1,\"nullableInteger\":2}";

            var result = (IntClass)Json.Current.ReadObject(jsonString, typeof(IntClass));
            Assert.AreEqual(1, result.Integer);
            Assert.AreEqual(2, result.NullableInteger);
        }

        [TestMethod]
        public void Json_ReadObject_UnsignedIntClass()
        {
            String jsonString = "{\"Integer\":1,\"NullableInteger\":2}";

            var result = (UnsignedIntClass)Json.Current.ReadObject(jsonString, typeof(UnsignedIntClass));
            Assert.AreEqual((uint)1, result.Integer);
            Assert.AreEqual((uint)2, result.NullableInteger);
        }

        [TestMethod]
        public void Json_ReadObject_LongClass()
        {
            String jsonString = "{\"Long\":1,\"NullableLong\":2}";

            var result = (LongClass)Json.Current.ReadObject(jsonString, typeof(LongClass));
            Assert.AreEqual((long)1, result.Long);
            Assert.AreEqual((long)2, result.NullableLong);
        }

        [TestMethod]
        public void Json_ReadObject_TimeSpanClass()
        {
            String jsonString = "{\"TimeSpan\":\"01:02:03\",\"NullableTimeSpan\":\"04:05:06\"}";

            var result = (TimespanClass)Json.Current.ReadObject(jsonString, typeof(TimespanClass));
            Assert.AreEqual(new TimeSpan(1, 2, 3), result.TimeSpan);
            Assert.AreEqual(new TimeSpan(4, 5, 6), result.NullableTimeSpan);
        }

        [TestMethod]
        public void Json_ReadObject_UnsignedLongClass()
        {
            String jsonString = "{\"Long\":1,\"NullableLong\":2}";

            var result = (UnsignedLongClass)Json.Current.ReadObject(jsonString, typeof(UnsignedLongClass));
            Assert.AreEqual((ulong)1, result.Long);
            Assert.AreEqual((ulong)2, result.NullableLong);
        }

        [TestMethod]
        public void Json_ReadObject_ShortClass()
        {
            String jsonString = "{\"Short\":1,\"NullableShort\":2}";

            var result = (ShortClass)Json.Current.ReadObject(jsonString, typeof(ShortClass));
            Assert.AreEqual((short)1, result.Short);
            Assert.AreEqual((short)2, result.NullableShort);
        }

        [TestMethod]
        public void Json_ReadObject_UnsignedShortClass()
        {
            String jsonString = "{\"Short\":1,\"NullableShort\":2}";

            var result = (UnsignedShortClass)Json.Current.ReadObject(jsonString, typeof(UnsignedShortClass));
            Assert.AreEqual((ushort)1, result.Short);
            Assert.AreEqual((ushort)2, result.NullableShort);
        }

        [TestMethod]
        public void Json_ReadObject_DoubleClass()
        {
            String jsonString = "{\"Double\":1,\"NullableDouble\":2}";

            var result = (DoubleClass)Json.Current.ReadObject(jsonString, typeof(DoubleClass));
            Assert.AreEqual(1, result.Double);
            Assert.AreEqual(2, result.NullableDouble);
        }

        [TestMethod]
        public void Json_ReadObject_StringClass()
        {
            String jsonString = "{\"String\":\"A\"}";

            var result = (StringClass)Json.Current.ReadObject(jsonString, typeof(StringClass));
            Assert.AreEqual("A", result.String);
        }

        /// <summary>
        /// Verifies that the proper exception is thrown when the format doesn't match.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Json_ReadObject_DateTimeClass_CustomFormat_InvalidFormat()
        {
            String jsonString = "{\"CustomFormatDateTime\":\"1955-03-04 09:22:33Z\"}";

            var result = (DateTimeClass)Json.Current.ReadObject(jsonString, typeof(DateTimeClass));
        }

        /// <summary>
        /// Verifies that the 'zulu' trick is not supported when using custom format.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Json_ReadObject_DateTimeClass_CustomFormat_InvalidFormat2()
        {
            String jsonString = "{\"CustomFormatDateTime\":\"1955.03.04Z\"}";

            var result = (DateTimeClass)Json.Current.ReadObject(jsonString, typeof(DateTimeClass));
        }

        /// <summary>
        /// Verifies that the 'zulu' trick is not supported when using custom format.
        /// </summary>
        [TestMethod]
        public void Json_ReadObject_DateTimeClass_CustomFormat()
        {
            String jsonString = "{\"CustomFormatDateTime\":\"1955.03.04\"}";

            var result = (DateTimeClass)Json.Current.ReadObject(jsonString, typeof(DateTimeClass));
            Assert.AreEqual(DateTimeKind.Local, result.CustomFormatDateTime.Kind, "The kind should be local, but no conversion is done, since the format is custom");
            Assert.AreEqual(new DateTime(1955, 3, 4, 0, 0, 0), result.CustomFormatDateTime, "The custom date has been parsed badly.");
        }

        /// <summary>
        /// Verifies that the date time values are properly read.
        /// </summary>
        [TestMethod]
        public void Json_ReadObject_DateTimeClass()
        {
            String jsonString = "{\"DateTime\":\"1955-03-04 09:22:33Z\",\"NullableDateTime\":\"1956-04-05 08:22:33Z\"}";

            var result = (DateTimeClass)Json.Current.ReadObject(jsonString, typeof(DateTimeClass));
            Assert.AreEqual(DateTimeKind.Utc, result.DateTime.Kind);
            Assert.AreEqual(DateTimeKind.Utc, result.NullableDateTime.Value.Kind);
            Assert.AreEqual(new DateTime(1955, 3, 4, 9, 22, 33), result.DateTime, "DateTime.Kind is unspecified; no transformation should have been done.");
            Assert.AreEqual(new DateTime(1956, 4, 5, 8, 22, 33), result.NullableDateTime, "DateTime.Kind is unspecified; no transformation should have been done.");
        }

        /// <summary>
        /// Verifies that the local date time which is receiving a utc value is properly converted.
        /// </summary>
        [TestMethod]
        public void Json_ReadObject_DateTimeClass_DateTimeOptions_Local_UtcInput()
        {
            String jsonString = "{\"LocalDateTime\":\"1955-03-04 09:22:33Z\"}";

            var result = (DateTimeClass)Json.Current.ReadObject(jsonString, typeof(DateTimeClass));
            Assert.AreEqual(DateTimeKind.Local, result.LocalDateTime.Kind);
            Assert.AreEqual(new DateTime(1955, 3, 4, 10, 22, 33), result.LocalDateTime, "Given value was in utc, property declares local time: should have moved to local time.");
        }

        /// <summary>
        /// Verifies that the local date time which is receiving an unspecified date kind value is not converted.
        /// </summary>
        [TestMethod]
        public void Json_ReadObject_DateTimeClass_DateTimeOptions_Local_UnspecifiedInput()
        {
            String jsonString = "{\"LocalDateTime\":\"1955-03-04 09:22:33\"}"; // Note the missing 'z' which indicates Utc kind

            var result = (DateTimeClass)Json.Current.ReadObject(jsonString, typeof(DateTimeClass));
            Assert.AreEqual(DateTimeKind.Local, result.LocalDateTime.Kind);
            Assert.AreEqual(new DateTime(1955, 3, 4, 9, 22, 33), result.LocalDateTime, "DateTime.Kind is unspecified; no transformation should have been done.");
        }

        /// <summary>
        /// Verifies that the utc date which receives a utc input is not converted.
        /// </summary>
        [TestMethod]
        public void Json_ReadObject_DateTimeClass_DateTimeOptions_Utc_UtcInput()
        {
            String jsonString = "{\"UtcDateTime\":\"1955-03-04 09:22:33Z\"}";

            var result = (DateTimeClass)Json.Current.ReadObject(jsonString, typeof(DateTimeClass));
            Assert.AreEqual(new DateTime(1955, 3, 4, 9, 22, 33), result.UtcDateTime, "Given value was in utc, property declares utc time: should not have moved to utc time.");
            Assert.AreEqual(DateTimeKind.Utc, result.UtcDateTime.Kind);
        }

        /// <summary>
        /// Verifies that the utc date which receives an unspecified date kind value is not converted.
        /// </summary>
        [TestMethod]
        public void Json_ReadObject_DateTimeClass_DateTimeOptions_Utc_UnspecifiedInput()
        {
            String jsonString = "{\"UtcDateTime\":\"1955-03-04 09:22:33\"}";// Note the missing 'z' which indicates Utc kind

            var result = (DateTimeClass)Json.Current.ReadObject(jsonString, typeof(DateTimeClass));
            Assert.AreEqual(DateTimeKind.Utc, result.UtcDateTime.Kind);
            Assert.AreEqual(new DateTime(1955, 3, 4, 9, 22, 33), result.UtcDateTime, "Given value was not specified, property declares utc time: should have moved to utc time according to local time zone.");
        }

        [TestMethod]
        public void Json_ReadObject_ByteArrayClass()
        {
            String jsonString = "{\"ByteArray\":\"AQID\",\"ByteEnumeration\":null}";

            var result = (ByteArrayClass)Json.Current.ReadObject(jsonString, typeof(ByteArrayClass));
            Assert.AreEqual(1, result.ByteArray[0]);
            Assert.AreEqual(2, result.ByteArray[1]);
            Assert.AreEqual(3, result.ByteArray[2]);
        }

        [TestMethod]
        public void Json_ReadObject_ByteEnumerationClass()
        {
            Assert.Inconclusive("Not supported");

            String jsonString = "{\"ByteArray\":null,\"ByteEnumeration\":\"AQID\"}";

            var result = (ByteArrayClass)Json.Current.ReadObject(jsonString, typeof(ByteArrayClass));
            Assert.IsTrue(new byte[] { 1, 2, 3 }.SequenceEqual(result.ByteEnumeration));
        }

        [TestMethod]
        public void Json_ReadObject_DictionaryClass()
        {
            String jsonString = "{\"Dictionary\":[{\"k\":1,\"v\":2},{\"k\":3,\"v\":5}]}";

            var result = (DictionaryClass<int, int>)Json.Current.ReadObject(jsonString, typeof(DictionaryClass<int, int>));
            Assert.AreEqual(2, result.Dictionary[1]);
            Assert.AreEqual(5, result.Dictionary[3]);
        }

        [TestMethod]
        public void Json_ReadObject_Enumeration()
        {
            String jsonString = "{\"Enumeration\":[1,2,3]}";

            var result = (EnumerableClass)Json.Current.ReadObject(jsonString, typeof(EnumerableClass));
            Assert.IsInstanceOfType(result.Enumeration.OfType<Object>().ElementAt(0), typeof(JsonPrimitive));
            Assert.IsInstanceOfType(result.Enumeration.OfType<Object>().ElementAt(1), typeof(JsonPrimitive));
        }

        [TestMethod]
        public void Json_ReadObject_GenericList()
        {
            String jsonString = "{[1,2,3]}";

            var result = (List<int>)Json.Current.ReadObject(jsonString, typeof(List<int>));
            Assert.IsTrue(result.Contains(1));
            Assert.IsTrue(result.Contains(2));
            Assert.IsTrue(result.Contains(3));
        }

        #endregion

        [TestMethod]
        public void Json_ToJson_AnonymousType()
        {
            var q = new { Name = "asassa", Address = "asadasd", Age = 12 };
            string sq = Json.Current.ToJson(q, new JsonParameters { EnableAnonymousTypes = true });
            Console.WriteLine(sq);
        }
    }
}
