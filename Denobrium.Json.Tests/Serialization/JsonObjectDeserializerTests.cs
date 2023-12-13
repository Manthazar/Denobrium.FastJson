using Denobrium.Json.Serialization;
using Denobrium.Json.Tests.Data.Helpers.StandardTypes;
using Denobrium.Json.Tests.Helpers;
using Denobrium.Json.Tests.Helpers.ComplexTypes;
using Denobrium.Json.TestsHelpers.StandardTypes;

namespace Denobrium.Json.Tests
{
    [TestClass]
    public class JsonObjectDeserializerTests
    {
        private JsonParameters CreateTestParameters()
        {
            return new JsonParameters()
            {
                UseUtcDateTime = true,
                UseTypeExtension = false,
            };
        }

        [TestMethod]
        public void JsonObjectDeserializer_Constructor()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
        }

        #region Deserialize Method Tests

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_IntClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"Integer\":1,\"NullableInteger\":2}";

            var result = (IntClass)decoder.Deserialize(ref jsonString, typeof(IntClass));
            Assert.AreEqual(1, result.Integer);
            Assert.AreEqual(2, result.NullableInteger);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_GuidClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"Guid\":\"523E0E25-0C24-421D-B738-D700E865F98D\",\"NullableGuid\":\"21DCC819-6F3D-4068-9AE7-3521896E5CFF\"}";

            var result = (GuidClass)decoder.Deserialize(ref jsonString, typeof(GuidClass));
            Assert.AreEqual(new Guid("523E0E25-0C24-421D-B738-D700E865F98D"), result.Guid);
            Assert.AreEqual(new Guid("21DCC819-6F3D-4068-9AE7-3521896E5CFF"), result.NullableGuid);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_UnsignedIntClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"Integer\":" + uint.MaxValue.ToString() + ",\"NullableInteger\":2}";

            var result = (UnsignedIntClass)decoder.Deserialize(ref jsonString, typeof(UnsignedIntClass));
            Assert.AreEqual(uint.MaxValue, result.Integer);
            Assert.AreEqual((uint)2, result.NullableInteger);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_LongClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"Long\":1,\"NullableLong\":2}";

            var result = (LongClass)decoder.Deserialize(ref jsonString, typeof(LongClass));
            Assert.AreEqual((long)1, result.Long);
            Assert.AreEqual((long)2, result.NullableLong);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_EnumClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"Gender\":\"Male\"}";

            var result = (EnumClass)decoder.Deserialize(ref jsonString, typeof(EnumClass));

            Assert.AreEqual(Gender.Male, result.Gender);
        }


        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_TimeSpanClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"TimeSpan\":\"01:02:03\",\"NullableTimeSpan\":\"04:05:06\"}";

            var result = (TimespanClass)decoder.Deserialize(ref jsonString, typeof(TimespanClass));
            Assert.AreEqual(new TimeSpan(1, 2, 3), result.TimeSpan);
            Assert.AreEqual(new TimeSpan(4, 5, 6), result.NullableTimeSpan);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_UnsignedLongClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"Long\":1,\"NullableLong\":2}";

            var result = (UnsignedLongClass)decoder.Deserialize(ref jsonString, typeof(UnsignedLongClass));
            Assert.AreEqual((ulong)1, result.Long);
            Assert.AreEqual((ulong)2, result.NullableLong);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_ShortClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"Short\":1,\"NullableShort\":2}";

            var result = (ShortClass)decoder.Deserialize(ref jsonString, typeof(ShortClass));
            Assert.AreEqual((short)1, result.Short);
            Assert.AreEqual((short)2, result.NullableShort);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_UnsignedShortClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"Short\":1,\"NullableShort\":2}";

            var result = (UnsignedShortClass)decoder.Deserialize(ref jsonString, typeof(UnsignedShortClass));
            Assert.AreEqual((ushort)1, result.Short);
            Assert.AreEqual((ushort)2, result.NullableShort);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_DoubleClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"Double\":1,\"NullableDouble\":2}";

            var result = (DoubleClass)decoder.Deserialize(ref jsonString, typeof(DoubleClass));
            Assert.AreEqual(1, result.Double);
            Assert.AreEqual(2, result.NullableDouble);
        }

        [TestMethod]
        public void JsonObjectDeserializer_StringClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"String\":\"A\"}";

            var result = (StringClass)decoder.Deserialize(ref jsonString, typeof(StringClass));
            Assert.AreEqual("A", result.String);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_DateTimeClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"DateTime\":\"1955-03-04 09:22:33Z\",\"NullableDateTime\":\"1956-04-05 08:22:33Z\"}";

            var result = (DateTimeClass)decoder.Deserialize(ref jsonString, typeof(DateTimeClass));
            Assert.AreEqual(DateTimeKind.Utc, result.DateTime.Kind);
            Assert.AreEqual(DateTimeKind.Utc, result.NullableDateTime!.Value.Kind);

            Assert.AreEqual(new DateTime(1955, 3, 4, 9, 22, 33), result.DateTime);
            Assert.AreEqual(new DateTime(1956, 4, 5, 8, 22, 33), result.NullableDateTime);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_ByteArrayClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"ByteArray\":\"AQID\",\"ByteEnumeration\":null}";

            var result = (ByteArrayClass)decoder.Deserialize(ref jsonString, typeof(ByteArrayClass));
            Assert.AreEqual(1, result!.ByteArray[0]);
            Assert.AreEqual(2, result!.ByteArray[1]);
            Assert.AreEqual(3, result!.ByteArray[2]);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_ByteEnumerationClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"ByteArray\":null,\"ByteEnumeration\":\"AQID\"}";

            var result = (ByteArrayClass)decoder.Deserialize(ref jsonString, typeof(ByteArrayClass));
            Assert.IsTrue(new byte[] { 1, 2, 3 }.SequenceEqual(result.ByteEnumeration));
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_DictionaryClass()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"Dictionary\":[{\"k\":1,\"v\":2},{\"k\":3,\"v\":5}]}";

            var result = (DictionaryClass<int, int>)decoder.Deserialize(ref jsonString, typeof(DictionaryClass<int, int>));
            Assert.AreEqual(2, result!.Dictionary[1]);
            Assert.AreEqual(5, result!.Dictionary[3]);
        }

        [TestMethod]
        public void JsonObjectDeserializer_Deserialize_Enumeration()
        {
            JsonObjectDeserializer decoder = new JsonObjectDeserializer(CreateTestParameters());
            String jsonString = "{\"Enumeration\":[1,2,3]}";

            var result = (EnumerableClass)decoder.Deserialize(ref jsonString, typeof(EnumerableClass));
            Assert.AreEqual((Int64)1, result!.Enumeration.OfType<Object>().ElementAt(0));
            Assert.AreEqual((Int64)2, result!.Enumeration.OfType<Object>().ElementAt(1));
        }

        #endregion
    }
}
