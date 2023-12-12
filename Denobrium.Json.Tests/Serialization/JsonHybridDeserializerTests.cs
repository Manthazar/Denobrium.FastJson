using System;
using Denobrium.Json.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Denobrium.Json.Tests
{
    [TestClass]
    public class JsonHybridDeserializerTests
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
        public void JsonHybridDeserializer_Constructor()
        {
            JsonHybridDeserializer deserializer = new JsonHybridDeserializer(new JsonParameters());
        }

        [TestMethod]
        public void JsonHybridDeserializer_Integer()
        {
            JsonHybridDeserializer deserializer = new JsonHybridDeserializer(new JsonParameters());
            String jsonString = "{\"Integer\":1,\"NullableInteger\":2}";

            var jsonObject = (MutableJsonObject) deserializer.Deserialize(ref jsonString);
            Assert.IsTrue(jsonObject.ContainsKey("Integer"));
            Assert.IsTrue(jsonObject.ContainsKey("NullableInteger"));
            Assert.AreEqual((Int64) 1, jsonObject["Integer"]);
            Assert.AreEqual((Int64) 2, jsonObject["NullableInteger"]);
        }

        [TestMethod]
        public void JsonHybridDeserializer_Bool()
        {
            JsonHybridDeserializer deserializer = new JsonHybridDeserializer(new JsonParameters());
            String jsonString = "{\"Bool\":true,\"NullableBool\":true}";

            var jsonObject = (MutableJsonObject)deserializer.Deserialize(ref jsonString);
            Assert.IsTrue(jsonObject.ContainsKey("Bool"));
            Assert.IsTrue(jsonObject.ContainsKey("NullableBool"));
            Assert.AreEqual(true, jsonObject["Bool"]);
            Assert.AreEqual(true, jsonObject["NullableBool"]);
        }

        [TestMethod]
        public void JsonHybridDeserializer_Double()
        {
            JsonHybridDeserializer deserializer = new JsonHybridDeserializer(new JsonParameters());
            String jsonString = "{\"Double\":1.1,\"NullableDouble\":1.2}";

            var jsonObject = (MutableJsonObject)deserializer.Deserialize(ref jsonString);
            Assert.IsTrue(jsonObject.ContainsKey("Double"));
            Assert.IsTrue(jsonObject.ContainsKey("NullableDouble"));
            Assert.AreEqual(1.1, jsonObject["Double"]);
            Assert.AreEqual(1.2, jsonObject["NullableDouble"]);
        }

        /// <summary>
        /// Verifies that byte arrays are deserialized as strings (the serializer doesn't know it better).
        /// </summary>
        [TestMethod]
        public void JsonHybridDeserializer_ByteArray_AsString()
        {
            JsonHybridDeserializer deserializer = new JsonHybridDeserializer(new JsonParameters());
            String jsonString = "{\"ByteArray\":\"AQID\"}";

            var jsonObject = (MutableJsonObject)deserializer.Deserialize(ref jsonString);
            Assert.IsTrue(jsonObject.ContainsKey("ByteArray"));
            Assert.AreEqual("AQID", jsonObject["ByteArray"]);
        }

        /// <summary>
        /// Verifies that date values are deserialized as strings (the serializer doesn't know it better).
        /// </summary>
        [TestMethod]
        public void JsonHybridDeserializer_DateTime_AsString()
        {
            JsonHybridDeserializer deserializer = new JsonHybridDeserializer(new JsonParameters());
            String jsonString = "{\"DateTime\":\"1955-03-04 09:22:33Z\",\"NullableDateTime\":\"1956-04-05 08:22:33Z\"}";

            var jsonObject = (MutableJsonObject)deserializer.Deserialize(ref jsonString);
            Assert.IsTrue(jsonObject.ContainsKey("DateTime"));
            Assert.AreEqual("1955-03-04 09:22:33Z", jsonObject["DateTime"]);
            Assert.AreEqual("1956-04-05 08:22:33Z", jsonObject["NullableDateTime"]);
        }

        [TestMethod]
        public void JsonHybridDeserializer_Dictionary()
        {
            JsonHybridDeserializer deserializer = new JsonHybridDeserializer(new JsonParameters());
            String jsonString = "{\"Field1\":\"Value1\", \"Field2\":\"Value2\"}";

            var jsonObject = (MutableJsonObject)deserializer.Deserialize(ref jsonString);
            Assert.IsTrue(jsonObject.ContainsKey("Field1"));
            Assert.IsTrue(jsonObject.ContainsKey("Field2"));

            Assert.AreEqual("Value1", jsonObject["Field1"]);
            Assert.AreEqual("Value2", jsonObject["Field2"]);
        }

        [TestMethod]
        public void JsonHybridDeserializer_Enumerations()
        {
            JsonHybridDeserializer deserializer = new JsonHybridDeserializer(new JsonParameters());
            String jsonString = "{\"Dictionary\":[{\"k\":1,\"v\":2},{\"k\":3,\"v\":5}]}";

            var jsonObject = (MutableJsonObject)deserializer.Deserialize(ref jsonString);
            Assert.IsTrue(jsonObject.ContainsKey("Dictionary"));

            var array = (MutableJsonArray)jsonObject["Dictionary"];
            var dictionary = (MutableJsonObject)array[0];
            Assert.IsTrue(dictionary.ContainsKey("k"));
            Assert.IsTrue(dictionary.ContainsKey("v"));
        }
    }
}
