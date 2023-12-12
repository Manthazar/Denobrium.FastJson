using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Denobrium.Json.Data;

namespace Denobrium.Json.Tests.Data
{
    [TestClass]
    public class JsonArrayTests
    {
        [TestMethod]
        public void JsonArray_Constructor()
        {
            JsonArray array = new JsonArray();
        }

        [TestMethod]
        public void JsonArray_Type()
        {
            JsonArray array = new JsonArray();

            Assert.AreEqual(JsonType.JsonArray, array.Type);
        }

        [TestMethod]
        public void JsonArray_Add()
        {
            JsonArray array = new JsonArray();
            JsonArray item = new JsonArray();
            array.Add(item);

            Assert.AreEqual(1, array.Count);
            Assert.AreEqual(item, array[0]);
        }

        [TestMethod]
        public void JsonArray_Remove()
        {
            JsonArray array = new JsonArray();
            JsonArray item = new JsonArray();
            array.Add(item);

            Assert.AreEqual(1, array.Count);
            Assert.AreEqual(item, array[0]);

            array.Remove(item);
            Assert.AreEqual(0, array.Count);
        }
    }
}
