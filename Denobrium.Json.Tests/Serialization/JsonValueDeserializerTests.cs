using System;
using System.Linq;
using Apolyton.FastJson.Data;
using Apolyton.FastJson.Serialization;
using Apolyton.FastJson.Tests.Data.Helpers.StandardTypes;
using Apolyton.FastJson.Tests.Helpers;
using Apolyton.FastJson.Tests.Helpers.ComplexTypes;
using Apolyton.FastJson.Tests.Helpers.StandardTypes;
using Apolyton.FastJson.TestsHelpers.StandardTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apolyton.FastJson.Registry;
using Apolyton.FastJson.Tests.Helpers.Polymorphism;
using System.Collections.Generic;

namespace Apolyton.FastJson.Tests
{
    [TestClass]
    public class JsonValueDeserializerTests
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
        public void JsonValueDeserializer_Constructor()
        {
            JsonValueDeserializer deserializer = new JsonValueDeserializer(new JsonParameters());
        }

        #region Deserialize Method Tests

        [TestMethod]
        public void JsonValueDeserializer_Deserialize_Number()
        {
            JsonValueDeserializer deserializer = new JsonValueDeserializer(new JsonParameters());
            String jsonString = "{\"Integer\":1,\"Double\":2.4}";

            var jsonObject = (JsonObject)deserializer.Deserialize(ref jsonString);
            Assert.IsTrue(jsonObject.ContainsKey("Integer"));
            Assert.IsTrue(jsonObject.ContainsKey("Double"));
            Assert.AreEqual(1, jsonObject["Integer"].AsInteger);
            Assert.AreEqual(2.4, jsonObject["Double"].AsDouble);
        }


        [TestMethod]
        public void JsonValueDeserializer_Deserialize_StringClass()
        {
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"String\":\"A\"}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            Assert.IsTrue(jsonObject.ContainsKey("String"));
        }

        [TestMethod]
        public void JsonValueDeserializer_Deserialize_Dictionary()
        {
            JsonValueDeserializer deserializer = new JsonValueDeserializer(new JsonParameters());
            String jsonString = "{\"Field1\":\"Value1\", \"Field2\":\"Value2\"}";

            var jsonObject = (JsonObject)deserializer.Deserialize(ref jsonString);
            Assert.IsTrue(jsonObject.ContainsKey("Field1"));
            Assert.IsTrue(jsonObject.ContainsKey("Field2"));

            Assert.IsInstanceOfType(jsonObject["Field1"], typeof(JsonPrimitive));
            Assert.IsInstanceOfType(jsonObject["Field2"], typeof(JsonPrimitive));

            Assert.AreEqual("Value1", jsonObject["Field1"].AsString);
            Assert.AreEqual("Value2", jsonObject["Field2"].AsString);
        }

        [TestMethod]
        public void JsonValueDeserializer_Deserialize_Array()
        {
            JsonValueDeserializer deserializer = new JsonValueDeserializer(new JsonParameters());
            String jsonString = "[1,2,3]";

            var jsonArray = (JsonArray)deserializer.Deserialize(ref jsonString);

            Assert.AreEqual(3, jsonArray.Count);
            Assert.AreEqual(1, jsonArray[0].AsInteger);
            Assert.AreEqual(2, jsonArray[1].AsInteger);
            Assert.AreEqual(3, jsonArray[2].AsInteger);
        }

        [TestMethod]
        public void JsonValueDeserializer_Deserialize_Array_ToIntList()
        {
            JsonValueDeserializer deserializer = new JsonValueDeserializer(new JsonParameters());
            String jsonString = "[1,2,3]";

            var jsonArray = (JsonArray)deserializer.Deserialize(ref jsonString);

            var result  = new List<int>();
            deserializer.BuildUp(result, jsonArray);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(3, result[2]);
        }

        #endregion

        #region BuildUp Method Tests

        /// <summary>
        /// This test verifies that default values are written into the instance, when the json string contains a null.
        /// </summary>
        [TestMethod]
        public void JsonValueDeserializer_BuildUp_DefaultValues()
        {
            IntClass target = new IntClass() { Integer = 3, NullableInteger = 25 };

            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Integer\":null,\"NullableInteger\":null}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual(0, target.Integer);
            Assert.AreEqual(null, target.NullableInteger);
        }


        [TestMethod]
        public void JsonValueDeserializer_BuildUp_IntClass()
        {
            IntClass target = new IntClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Integer\":1,\"NullableInteger\":2}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual(1, target.Integer);
            Assert.AreEqual(2, target.NullableInteger);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_UnsignedIntClass()
        {
            UnsignedIntClass target = new UnsignedIntClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Integer\":" + uint.MaxValue.ToString() + ",\"NullableInteger\":2}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual(uint.MaxValue, target.Integer);
            Assert.AreEqual((uint)2, target.NullableInteger);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_LongClass()
        {
            LongClass target = new LongClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Long\":1,\"NullableLong\":2}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual((long)1, target.Long);
            Assert.AreEqual((long)2, target.NullableLong);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_StringClass()
        {
            StringClass target = new StringClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"String\":\"SomeString\"}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual("SomeString", target.String);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_TimeSpanClass()
        {
            TimespanClass target = new TimespanClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"TimeSpan\":\"01:02:03\",\"NullableTimeSpan\":\"04:05:06\"}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual(1, target.TimeSpan.Hours);
            Assert.AreEqual(2, target.TimeSpan.Minutes);
            Assert.AreEqual(3, target.TimeSpan.Seconds);
            Assert.AreEqual(4, target.NullableTimeSpan.Value.Hours);
            Assert.AreEqual(5, target.NullableTimeSpan.Value.Minutes);
            Assert.AreEqual(6, target.NullableTimeSpan.Value.Seconds);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_UnsignedLongClass()
        {
            UnsignedLongClass target = new UnsignedLongClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Long\":1,\"NullableLong\":2}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
            Assert.AreEqual((ulong)1, target.Long);
            Assert.AreEqual((ulong)2, target.NullableLong);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_ShortClass()
        {
            ShortClass target = new ShortClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Short\":1,\"NullableShort\":2}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
            Assert.AreEqual((short)1, target.Short);
            Assert.AreEqual((short)2, target.NullableShort);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_UnsignedShortClass()
        {
            UnsignedShortClass target = new UnsignedShortClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Short\":1,\"NullableShort\":2}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
            Assert.AreEqual((ushort)1, target.Short);
            Assert.AreEqual((ushort)2, target.NullableShort);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_DoubleClass()
        {
            DoubleClass target = new DoubleClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Double\":1.2,\"NullableDouble\":2.3}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
            Assert.AreEqual((double)1.2, target.Double);
            Assert.AreEqual((double)2.3, target.NullableDouble);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_NestingClass()
        {
            NestingClass target = new NestingClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Long\":5,\"NestedClass\":{\"Integer\":3}}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
            Assert.AreEqual((long)5, target.Long);
            Assert.IsNotNull(target.NestedClass);
            Assert.AreEqual(3, target.NestedClass.Integer);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_DateTimeClass()
        {
            DateTimeClass target = new DateTimeClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{ " +
                                  "\"DateTime\":\"1955-03-04 09:22:33\"" +
                                  "\"NullableDateTime\":\"1955-03-04 09:22:33Z\"" +
                                  "\"LocalDateTime\":\"1955-03-04 09:22:33Z\"" +
                                  "\"UtcDateTime\":\"1955-03-04 09:22:33Z\"" +
                                "}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual(DateTimeKind.Unspecified, target.DateTime.Kind, "All unflagged incoming date values are considered to be in local time zone.");
            Assert.AreEqual(DateTimeKind.Utc, target.NullableDateTime.Value.Kind);
            Assert.AreEqual(DateTimeKind.Local, target.LocalDateTime.Kind);
            Assert.AreEqual(DateTimeKind.Utc, target.UtcDateTime.Kind);

            Assert.AreEqual(new DateTime(1955, 3, 4, 9, 22, 33), target.DateTime, "The date should NOT have been changed.");
            Assert.AreEqual(new DateTime(1955, 3, 4, 9, 22, 33), target.NullableDateTime, "The date should NOT have been changed.");
            Assert.AreEqual(new DateTime(1955, 3, 4, 10, 22, 33), target.LocalDateTime, "The date should be in local time zone.");
            Assert.AreEqual(new DateTime(1955, 3, 4, 9, 22, 33), target.UtcDateTime, "The date should be in utc time zone.");
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_ComplexClass()
        {
            ComplexClass source = ComplexClass.CreateObject(false, false);
            ComplexClass target = new ComplexClass();
            JsonSerializer encoder = new JsonSerializer(CreateTestParameters());
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = encoder.Serialize(source);

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual(source.booleanValue, target.booleanValue);
            Assert.AreEqual(new DateTime(source.date.Year, source.date.Month, source.date.Day, source.date.Hour, source.date.Minute, source.date.Second), target.date);
            Assert.AreEqual(source.multilineString, target.multilineString);
            Assert.AreEqual(source.ordinaryDecimal, target.ordinaryDecimal);
            Assert.AreEqual(source.ordinaryDouble, target.ordinaryDouble);
            Assert.AreEqual(source.laststring, target.laststring);
            Assert.AreEqual(source.nullableGuid, target.nullableGuid);
            Assert.AreEqual(source.nullableDecimal, target.nullableDecimal);
            Assert.AreEqual(source.nullableDouble, target.nullableDouble);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_ComplexClass_Exotic()
        {
            ComplexClass source = ComplexClass.CreateObject(true, false);
            ComplexClass target = new ComplexClass();
            JsonSerializer encoder = new JsonSerializer(CreateTestParameters());
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = encoder.Serialize(source);

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_ByteArrayClass()
        {
            ByteArrayClass target = new ByteArrayClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"ByteArray\":\"AQID\",\"ByteEnumeration\":null}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_ByteEnumerationClass()
        {
            Assert.Inconclusive("This is not supported currently");

            ByteArrayClass target = new ByteArrayClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"ByteArray\":null,\"ByteEnumeration\":\"AQID\"}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
        }

        /// <summary>
        /// Verifies that the proper exception is thrown. Here we give a json string key and we ask to build into an int key -which is of course not possible.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void JsonValueDeserializer_BuildUp_DictionaryClass_KeyFormatIncompatible()
        {
            DictionaryClass<int, int> target = new DictionaryClass<int, int>();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Dictionary\": {\"k1\":1,\"k2\":2, \"k3\":3,\"k5\":5}}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
        }

        /// <summary>
        /// Verifies that the proper exception is thrown. Here we give a json double value and we ask to build into an int key -which is of course not possible.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void JsonValueDeserializer_BuildUp_DictionaryClass_ValueFormatIncompatible()
        {
            DictionaryClass<int, int> target = new DictionaryClass<int, int>();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Dictionary\": {\"k1\":1.1,\"k2\":2.2, \"k3\":3.2,\"k5\":5.2}}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_DictionaryClass()
        {
            DictionaryClass<int, int> target = new DictionaryClass<int, int>();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"1\":1,\"2\":2, \"3\":3,\"5\":5}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_ListClass()
        {
            //DictionaryClass<int, int> target = new DictionaryClass<int, int>();
            //JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            //String jsonString = "{\"1\":1,\"2\":2, \"3\":3,\"5\":5}";

            //var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            //decoder.BuildUp(target, jsonObject);
        }

        /// <summary>
        /// Verifies that the not supported exception is thrown.
        /// </summary>
        [TestMethod]
        public void JsonValueDeserializer_BuildUp_HashSetClass()
        {
            HashSetClass target = new HashSetClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Set\": [\"A\", \"B\"]}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.IsNotNull(target.Set);
            Assert.IsTrue(target.Set.Contains("A"));
            Assert.IsTrue(target.Set.Contains("B"));
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_DictionaryClass_ValueIsClass()
        {
            DictionaryClass<String, BoolClass> target = new DictionaryClass<string, BoolClass>();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"A\":{\"Bool\":true}, \"B\": {\"Bool\":null}}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_Enumeration()
        {
            EnumerableClass target = new EnumerableClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Enumeration\":[1,2,3]}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_GenericList()
        {
            JsonValueDeserializer deserializer = new JsonValueDeserializer(new JsonParameters());
            String jsonString = "[1,2,3]";

            var jsonArray = (JsonArray)deserializer.Deserialize(ref jsonString);

            var result = new List<int>();
            deserializer.BuildUp(result, jsonArray);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(3, result[2]);
        }

        /// <summary>
        /// Tests the build on the simple class BaseClass
        /// </summary>
        [TestMethod]
        public void JsonValueDeserializer_BuildUp_BaseClass()
        {
            BaseClass target = new BaseClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Code\":\"Alberta\",\"Name\":\"Toronto\"}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual("Alberta", target.Code);
            Assert.AreEqual("Toronto", target.Name);
        }

        /// <summary>
        /// Tests the build up on the inherited SubClass1
        /// </summary>
        [TestMethod]
        public void JsonValueDeserializer_BuildUp_SubClass1()
        {
            SubClass1 target = new SubClass1();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Code\":\"Alberta\",\"Name\":\"Toronto\",\"guid\":\"FA6E5DB1-93BB-4DFF-A630-101C8741C428\"}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual("Alberta", target.Code);
            Assert.AreEqual("Toronto", target.Name);
            Assert.AreEqual(Guid.Parse("FA6E5DB1-93BB-4DFF-A630-101C8741C428"), target.guid);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_GuidClass()
        {
            GuidClass target = new GuidClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Guid\":\"523E0E25-0C24-421D-B738-D700E865F98D\",\"NullableGuid\":\"21DCC819-6F3D-4068-9AE7-3521896E5CFF\"}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);
            Assert.AreEqual(new Guid("523E0E25-0C24-421D-B738-D700E865F98D"), target.Guid);
            Assert.AreEqual(new Guid("21DCC819-6F3D-4068-9AE7-3521896E5CFF"), target.NullableGuid);
        }

        /// <summary>
        /// Tests the build up on the inherited SubClass1
        /// </summary>
        [TestMethod]
        public void JsonValueDeserializer_BuildUp_EnumClass()
        {
            EnumClass target = new EnumClass();
            JsonValueDeserializer decoder = new JsonValueDeserializer(CreateTestParameters());
            String jsonString = "{\"Gender\":\"Male\"}";

            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual(Gender.Male, target.Gender);
        }

        [TestMethod]
        public void JsonValueDeserializer_BuildUp_PolymorphList()
        {
            var parameters = CreateTestParameters();
            parameters.UseTypeExtension = true;
            parameters.RegisterTypeDescriptor(new DataContractTypeDescriptor(this.GetType().Assembly));

            var jsonString = "{\"$type\":\"Apolyton.FastJson.Tests.Helpers.Polymorphism.Zoo\",\"Animals\":["+
                    "{\"$type\":\"doggy\",\"Power\":4},"+
                    "{\"$type\":\"kitty\",\"Cuteness\":2}]}";

            Zoo target = new Zoo();
            JsonValueDeserializer decoder = new JsonValueDeserializer(parameters);
            var jsonObject = (JsonObject)decoder.Deserialize(ref jsonString);
            decoder.BuildUp(target, jsonObject);

            Assert.AreEqual(2, target.Animals.Count);
            Assert.AreEqual(1, target.Animals.OfType<Dog>().Count());
            Assert.AreEqual(1, target.Animals.OfType<Cat>().Count());
            Assert.AreEqual(4, ((Dog)target.Animals[0]).Power);
            Assert.AreEqual(2, ((Cat)target.Animals[1]).Cuteness);
        }

        #endregion
    }
}
