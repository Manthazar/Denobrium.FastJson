using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apolyton.FastJson.Data;

namespace Apolyton.FastJson.Tests.Data
{
    [TestClass]
    public class JsonObjectTests
    {
        [TestMethod]
        public void JsonObject_Constructor()
        {
            JsonObject array = new JsonObject();
        }

        [TestMethod]
        public void JsonObject_Type()
        {
            JsonObject array = new JsonObject();

            Assert.AreEqual(JsonType.JsonObject, array.Type);
        }

        [TestMethod]
        public void JsonObject_Add()
        {
            JsonObject array = new JsonObject();
            JsonObject item = new JsonObject();
            array.Add("A", item);

            Assert.AreEqual(1, array.Count);
            Assert.AreEqual(item, array["A"]);
        }

        [TestMethod]
        public void JsonObject_Remove()
        {
            JsonObject array = new JsonObject();
            JsonObject item = new JsonObject();
            array.Add("A", item);

            Assert.AreEqual(1, array.Count);
            Assert.AreEqual(item, array["A"]);

            array.Remove("A");
            Assert.AreEqual(0, array.Count);
        }
    }
}
