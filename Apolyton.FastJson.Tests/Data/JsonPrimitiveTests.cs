using System;
using Apolyton.FastJson.Data;
using Apolyton.FastJson.Tests.Helpers.ParameterRelated;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Apolyton.FastJson.Tests.Data
{
    [TestClass]
    public class JsonPrimitiveTests
    {
        [TestMethod]
        public void JsonPrimitive_Constructor_Object()
        {
            JsonPrimitive primitive = new JsonPrimitive("A");
        }

        [TestMethod]
        public void JsonPrimitive_Bool()
        {
            JsonPrimitive primitive = new JsonPrimitive(true);
            Assert.AreEqual(true, (bool)primitive);
        }

        [TestMethod]
        public void JsonPrimitive_Bool_FromString()
        {
            JsonPrimitive primitive = new JsonPrimitive("true");
            Assert.AreEqual(true, (Boolean)primitive);
        }

        [TestMethod]
        public void JsonPrimitive_String()
        {
            JsonPrimitive primitive = new JsonPrimitive("A");
            Assert.AreEqual("A", (String)primitive);
        }

        [TestMethod]
        public void JsonPrimitive_Guid()
        {
            JsonPrimitive primitive = new JsonPrimitive("FA6E5DB1-93BB-4DFF-A630-101C8741C428");
            Assert.AreEqual(Guid.Parse("FA6E5DB1-93BB-4DFF-A630-101C8741C428"), (Guid)primitive);
        }

        [TestMethod]
        public void JsonPrimitive_Int()
        {
            JsonPrimitive primitive = new JsonPrimitive("5");
            Assert.AreEqual(5, (int)primitive);
        }

        [TestMethod]
        public void JsonPrimitive_Int_Nullable()
        {
            JsonPrimitive primitive = new JsonPrimitive("5");
            Assert.AreEqual((int?)5, Convert.ChangeType(primitive, typeof(int?)));
        }

        [TestMethod]
        public void JsonPrimitive_ULong()
        {
            JsonPrimitive primitive = new JsonPrimitive("5");
            Assert.AreEqual((ulong)5, Convert.ChangeType(primitive, typeof(ulong)));
        }

        [TestMethod]
        public void JsonPrimitive_ULong_Nullable()
        {
            JsonPrimitive primitive = new JsonPrimitive("5");
            Assert.AreEqual((ulong?)5, Convert.ChangeType(primitive, typeof(ulong?)));
        }

        [TestMethod]
        public void JsonPrimitive_UInt()
        {
            JsonPrimitive primitive = new JsonPrimitive("5");
            Assert.AreEqual((uint)5, Convert.ChangeType(primitive, typeof(uint)));
        }

        [TestMethod]
        public void JsonPrimitive_UInt_Nullable()
        {
            JsonPrimitive primitive = new JsonPrimitive("5");
            Assert.AreEqual((uint?)5, Convert.ChangeType(primitive, typeof(uint?)));
        }

        [TestMethod]
        public void JsonPrimitive_UShort()
        {
            JsonPrimitive primitive = new JsonPrimitive("5");
            Assert.AreEqual((ushort)5, Convert.ChangeType(primitive, typeof(ushort)));
        }

        [TestMethod]
        public void JsonPrimitive_UShort_Nullable()
        {
            JsonPrimitive primitive = new JsonPrimitive("5");
            Assert.AreEqual((ushort?)5, Convert.ChangeType(primitive, typeof(ushort?)));
        }

        [TestMethod]
        public void JsonPrimitive_Decimal()
        {
            JsonPrimitive primitive = new JsonPrimitive("5");
            Assert.AreEqual(5m, (decimal)primitive);
        }

        [TestMethod]
        public void JsonPrimitive_Decimal_WhenWithSeparator()
        {
            JsonPrimitive primitive = new JsonPrimitive("5.23");
            Assert.AreEqual(5.23m, (decimal)primitive);
        }


        [TestMethod]
        public void JsonPrimitive_Double()
        {
            JsonPrimitive primitive = new JsonPrimitive("5");
            Assert.AreEqual(5, (double)primitive);
        }

        [TestMethod]
        public void JsonPrimitive_Double_WhenWithSeparator()
        {
            JsonPrimitive primitive = new JsonPrimitive("5.23");
            Assert.AreEqual(5.23, (double)primitive);
            // To do: is there a risk of region incompatibility?
        }

        [TestMethod]
        public void JsonPrimitive_Float()
        {
            JsonPrimitive primitive = new JsonPrimitive("523");
            Assert.AreEqual(523f, (float)primitive);
        }

        [TestMethod]
        public void JsonPrimitive_Float_WhenWithSeparator()
        {
            JsonPrimitive primitive = new JsonPrimitive("5.23");
            Assert.AreEqual(5.23f, (float)primitive);
        }

        [TestMethod]
        public void JsonPrimitive_DateTime()
        {
            DateTime dt = DateTime.Now;
            JsonPrimitive primitive = new JsonPrimitive(dt.ToString("yyyy-MM-dd HH:mm:ss"));

            DateTime primitiveDate = (DateTime)primitive;
            Assert.AreEqual(dt.Year, primitiveDate.Year);
            Assert.AreEqual(dt.Month, primitiveDate.Month);
            Assert.AreEqual(dt.Day, primitiveDate.Day);
            Assert.AreEqual(dt.Hour, primitiveDate.Hour);
            Assert.AreEqual(dt.Minute, primitiveDate.Minute);
            Assert.AreEqual(dt.Second, primitiveDate.Second);
        }

        [TestMethod]
        public void JsonPrimitive_DateTime_Utc()
        {
            DateTime utcDate = DateTime.UtcNow;
            DateTime localDate = DateTime.Now;
            DateTime expectedDate = localDate.ToUniversalTime();
            JsonPrimitive primitive = new JsonPrimitive(utcDate.ToString("yyyy-MM-dd HH:mm:ssZ"));

            DateTime primitiveDate = (DateTime)primitive;
            Assert.AreEqual(expectedDate.Year, primitiveDate.Year);
            Assert.AreEqual(expectedDate.Month, primitiveDate.Month);
            Assert.AreEqual(expectedDate.Day, primitiveDate.Day);
            Assert.AreEqual(expectedDate.Hour, primitiveDate.Hour);
            Assert.AreEqual(expectedDate.Minute, primitiveDate.Minute);
            Assert.AreEqual(expectedDate.Second, primitiveDate.Second);
        }

        [TestMethod]
        public void JsonPrimitive_TimeStamp()
        {
            TimeSpan ts = TimeSpan.FromSeconds(97);
            JsonPrimitive primitive = new JsonPrimitive(ts.ToString());
            Assert.AreEqual(ts, (TimeSpan)primitive);
        }

        [TestMethod]
        public void JsonPrimitive_GuidString()
        {
            Guid guid = Guid.NewGuid();
            JsonPrimitive primitive = new JsonPrimitive(guid.ToString());
            Assert.AreEqual(guid, (Guid)primitive);
        }

        [TestMethod]
        public void JsonPrimitive_FastGuidString()
        {
            Guid guid = Guid.NewGuid();
            byte[] bytes = guid.ToByteArray();
            JsonPrimitive primitive = new JsonPrimitive(Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None));
            Assert.AreEqual(guid, (Guid)primitive);
        }

        /// <summary>
        /// Verifies that when the value of the custom type is null, 'null' is serialized.
        /// </summary>
        [TestMethod]
        public void JsonPrimitive_CustomType()
        {
            JsonParameters parameters = new JsonParameters();
            JsonPrimitive primitive = new JsonPrimitive("ToTo", parameters.Registry);

            CustomTypeClass customTypeClass = new CustomTypeClass();

            parameters.RegisterCustomType(typeof(CustomTypeClass.CustomType), (obj) => { return null; }, (str) =>
            {
                if (str == "ToTo")
                {
                    return new CustomTypeClass();
                }

                return null;
            });

            ((IConvertible)primitive).ToType(typeof(CustomTypeClass.CustomType), null);
        }
    }
}
