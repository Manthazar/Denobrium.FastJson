using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Apolyton.FastJson.Tests.Helpers;
using Apolyton.FastJson.Tests.Helpers.ComplexTypes;
using Apolyton.FastJson.Tests.Helpers.ParameterRelated;
using Apolyton.FastJson.TestsHelpers.ParameterRelated;
using Apolyton.FastJson.TestsHelpers.StandardTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apolyton.FastJson.Tests.Helpers.StandardTypes;
using Apolyton.FastJson.Registry;
using Apolyton.FastJson.Tests.Helpers.Polymorphism;

namespace Apolyton.FastJson.Tests
{
    [TestClass]
    public class JsonSerializerTests
    {
        private JsonParameters CreateTestParameters()
        {
            var parameter = new JsonParameters()
            {
                UseUtcDateTime = true,
                UseTypeExtension = false,
            };

            parameter.Serialization.SerializeNullValues = false;

            return parameter;
        }

        [TestInitialize]
        public void ResetRegistry()
        {
            //JsonRegistry.Current.Reset();
        }

        [TestMethod]
        public void JsonSerializer_Constructor()
        {
            JsonSerializer serializer = new JsonSerializer(new JsonParameters());
        }

        #region Serializing values.

        [TestMethod]
        public void JsonSerializer_Integer()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            IntClass given = new IntClass()
            {
                Integer = 1,
                NullableInteger = 2
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Integer\":1,\"NullableInteger\":2}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_UnsignedInteger()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            UnsignedIntClass given = new UnsignedIntClass()
            {
                Integer = 1,
                NullableInteger = 2
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Integer\":1,\"NullableInteger\":2}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Double()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            DoubleClass given = new DoubleClass()
            {
                Double = 1.1,
                NullableDouble = 2.2
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Double\":1.1,\"NullableDouble\":2.2}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Short()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            ShortClass given = new ShortClass()
            {
                Short = 1,
                NullableShort = 2
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Short\":1,\"NullableShort\":2}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_UnsignedShort()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            UnsignedShortClass given = new UnsignedShortClass()
            {
                Short = 1,
                NullableShort = 2
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Short\":1,\"NullableShort\":2}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Long()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            LongClass given = new LongClass()
            {
                Long = 1,
                NullableLong = 2
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Long\":1,\"NullableLong\":2}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_UnsignedLong()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            UnsignedLongClass given = new UnsignedLongClass()
            {
                Long = 1,
                NullableLong = 2
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Long\":1,\"NullableLong\":2}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_DateTime()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            DateTimeClass given = new DateTimeClass()
            {
                DateTime = new DateTime(1955, 3, 4, 10, 22, 33),
                NullableDateTime = new DateTime(1956, 4, 5, 10, 22, 33)
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual(TimeSpan.FromHours(1), TimeZone.CurrentTimeZone.GetUtcOffset(given.DateTime));
            Assert.AreEqual(TimeSpan.FromHours(2), TimeZone.CurrentTimeZone.GetUtcOffset((DateTime)given.NullableDateTime));
            Assert.IsTrue(CreateTestParameters().UseUtcDateTime, "Supposed to use the utc date.");

            Assert.AreEqual("{\"DateTime\":\"1955-03-04 10:22:33Z\",\"NullableDateTime\":\"1956-04-05 10:22:33Z\"}", jsonString, 
                "Since the kind of the date time is unknown, no adjustment to UTC should have happened.");
        }

        [TestMethod]
        public void JsonSerializer_DateTimeUtc()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            DateTimeClass given = new DateTimeClass()
            {
                DateTime = new DateTime(1955, 3, 4, 10, 22, 33).ToUniversalTime(),
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual(TimeSpan.FromHours(0), TimeZone.CurrentTimeZone.GetUtcOffset(given.DateTime));
            Assert.AreEqual("{\"DateTime\":\"1955-03-04 09:22:33Z\"}", jsonString, "No additional conversion should have happened, the date was already in utc.");
        }

        [TestMethod]
        public void JsonSerializer_TimeSpan()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            TimespanClass given = new TimespanClass()
            {
                TimeSpan = new TimeSpan(1, 2, 3),
                NullableTimeSpan = new TimeSpan(2, 3, 4)
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"TimeSpan\":\"01:02:03\",\"NullableTimeSpan\":\"02:03:04\"}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Bool()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            BoolClass given = new BoolClass()
            {
                Bool = true,
                NullableBool = true
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Bool\":true,\"NullableBool\":true}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Dictionary()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            DictionaryClass<int, int> given = new DictionaryClass<int, int>();
            given.Dictionary = new Dictionary<int, int>();
            given.Dictionary[1] = 2;
            given.Dictionary[3] = 5;

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Dictionary\":[{\"k\":1,\"v\":2},{\"k\":3,\"v\":5}]}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_StringDictionary()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            DictionaryClass<String, int> given = new DictionaryClass<String, int>();
            given.Dictionary = new Dictionary<String, int>();
            given.Dictionary["A"] = 2;
            given.Dictionary["B"] = 5;

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Dictionary\":{\"A\":2,\"B\":5}}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_String()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            StringClass given = new StringClass()
            {
                String = "A",
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"String\":\"A\"}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_List()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            List<String> list = new List<string>();
            list.Add("A");

            String jsonString = serializer.Serialize(list);
            Assert.AreEqual("[\"A\"]", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Enumeration()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            EnumerableClass given = new EnumerableClass()
            {
                Enumeration = new int[] { 1, 2, 3 },
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Enumeration\":[1,2,3]}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_ByteArray()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            ByteArrayClass given = new ByteArrayClass()
            {
                ByteArray = new byte[] { 1, 2, 3 },
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"ByteArray\":\"AQID\"}", jsonString, "Bytes should be encoded in Base64 String");
        }

        [TestMethod]
        public void JsonSerializer_ByteEnumeration()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            ByteArrayClass given = new ByteArrayClass()
            {
                ByteEnumeration = new List<byte>(new byte[] { 1, 2, 3 }),
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"ByteEnumeration\":\"AQID\"}", jsonString, "Bytes should be encoded in Base64 String");
        }

        [TestMethod]
        public void JsonSerializer_CustomStruct()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            CustomStruct given = new CustomStruct()
            {
                Text = "A"
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Text\":\"A\"}", jsonString);
        }

        #endregion

        #region Parameter Tests

        [TestMethod]
        public void JsonSerializer_Parameter_SerializeNullValues_False()
        {
            JsonParameters parameter = CreateTestParameters();
            parameter.Serialization.SerializeNullValues = false;

            JsonSerializer serializer = new JsonSerializer(parameter);
            StringClass given = new StringClass()
            {
                String = null,
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Parameter_SerializeNullValues_True()
        {
            JsonParameters parameter = CreateTestParameters();
            parameter.Serialization.SerializeNullValues = true;

            JsonSerializer serializer = new JsonSerializer(parameter);
            StringClass given = new StringClass()
            {
                String = null,
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"String\":null}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Parameter_OptOut()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            OptOutClass given = new OptOutClass()
            {
                Value = 1,
                NotSerialized = 2,
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Value\":1}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Parameter_OptIn()
        {
            var parameters = CreateTestParameters();
            parameters.MemberStrategy = MemberStrategy.PropertyOptIn;

            JsonSerializer serializer = new JsonSerializer(parameters);
            OptInClass given = new OptInClass()
            {
                Value = 1,
                NotSerialized = 2,
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Value\":1}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Parameter_ReadOnly_Excluded()
        {
            var parameters = CreateTestParameters();
            parameters.Serialization.IncludeReadOnly = false;

            JsonSerializer serializer = new JsonSerializer(parameters);
            var given = new ReadOnlyClass(5, 6);

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Value\":5}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Parameter_ReadOnly_Included()
        {
            var parameters = CreateTestParameters();
            parameters.MemberStrategy = MemberStrategy.PropertyOptOut;
            parameters.Serialization.IncludeReadOnly = true;

            JsonSerializer serializer = new JsonSerializer(parameters);
            var given = new ReadOnlyClass(5, 6);

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual("{\"Value\":5,\"ReadOnlyValue\":6}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_Parameter_UseUtcDateTime()
        {
            var parameters = CreateTestParameters();
            parameters.UseUtcDateTime = false;

            JsonSerializer serializer = new JsonSerializer(parameters);
            DateTimeClass given = new DateTimeClass()
            {
                DateTime = new DateTime(1955, 3, 4, 10, 22, 33),
                NullableDateTime = new DateTime(1956, 4, 5, 10, 22, 33)
            };

            String jsonString = serializer.Serialize(given);
            Assert.AreEqual(TimeSpan.FromHours(1), TimeZone.CurrentTimeZone.GetUtcOffset(given.DateTime));
            Assert.AreEqual(TimeSpan.FromHours(2), TimeZone.CurrentTimeZone.GetUtcOffset((DateTime)given.NullableDateTime));
            Assert.IsTrue(CreateTestParameters().UseUtcDateTime, "Supposed to use the utc date.");

            Assert.AreEqual("{\"DateTime\":\"1955-03-04 10:22:33\",\"NullableDateTime\":\"1956-04-05 10:22:33\"}", jsonString, "Although there are different time zones, the serialized output should not reflect the UTC difference");
        }

        #endregion

        #region Custom Types

        [TestMethod]
        public void JsonSerializer_CustomType_Serialize_ByDelegates()
        {
            JsonParameters parameters = CreateTestParameters();
            JsonSerializer serializer = new JsonSerializer(parameters);
            CustomTypeClass customTypeClass = new CustomTypeClass() { Custom = new CustomTypeClass.CustomType() };

            parameters.RegisterCustomType(typeof(CustomTypeClass.CustomType), (obj) => { return "yes"; }, (obj) => { return null; });

            String jsonString = serializer.Serialize(customTypeClass);
            Assert.AreEqual("{\"Custom\":\"yes\"}", jsonString);
        }

        [TestMethod]
        public void JsonSerializer_CustomType_Serialize_ByObject()
        {
            JsonParameters parameters = CreateTestParameters();
            JsonSerializer serializer = new JsonSerializer(parameters);
            CustomTypeClass customTypeClass = new CustomTypeClass() { Custom = new CustomTypeClass.CustomType() };

            parameters.RegisterCustomType(new CustomTypeSerializerFake());

            String jsonString = serializer.Serialize(customTypeClass);
            Assert.AreEqual("{\"Custom\":\"yes\"}", jsonString);
        }

        /// <summary>
        /// Verifies that when the value of the custom type is null, 'null' is serialized.
        /// </summary>
        [TestMethod]
        public void JsonSerializer_CustomType_Serialize_Null()
        {
            JsonParameters parameters = CreateTestParameters();
            parameters.Serialization.SerializeNullValues = true;

            JsonSerializer serializer = new JsonSerializer(parameters);
            CustomTypeClass customTypeClass = new CustomTypeClass();

            parameters.RegisterCustomType(typeof(CustomTypeClass.CustomType), (obj) => { return "yes"; }, (obj) => { return null; });

            String jsonString = serializer.Serialize(customTypeClass);
            Assert.AreEqual("{\"Custom\":null}", jsonString);
        }

        #endregion

        #region Type Extensions

        [TestMethod]
        public void JsonSerializer_TypeExtensions()
        {
             var parameters = CreateTestParameters();
            parameters.UseTypeExtension = true;
            parameters.RegisterTypeDescriptor(new DataContractTypeDescriptor());

            Zoo zoo = new Zoo();
            zoo.Animals = new List<Animal>(new Animal[] { new Dog(), new Cat() });

            var jsonString = new JsonSerializer(parameters).Serialize(zoo);
            Assert.AreEqual(
                "{\"$type\":\"Apolyton.FastJson.Tests.Helpers.Polymorphism.Zoo\",\"Animals\":[{\"$type\":\"doggy\",\"Power\":4},{\"$type\":\"kitty\",\"Cuteness\":2}]}",
                jsonString);
        }

        #endregion

        /// <summary>
        /// Verifies that when a class is passed without properties the serializer should either write 'null' or throw an exception. Otherwise non-trivial errors might
        /// appearch on client/ deserialization side.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void JsonSerializer_ClassWithNoProperties()
        {
            JsonSerializer serializer = new JsonSerializer(CreateTestParameters());
            ClassWithNoProperties given = new ClassWithNoProperties();

            serializer.Serialize(given);
        }
    }
}
