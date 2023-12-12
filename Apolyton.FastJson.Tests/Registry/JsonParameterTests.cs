using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apolyton.FastJson.Tests.Helpers.ParameterRelated;
using Apolyton.FastJson.Tests.Helpers;

namespace Apolyton.FastJson.Tests.Registry
{
    /// <summary>
    /// Summary description for JsonParameterTests
    /// </summary>
    [TestClass]
    public class JsonParameterTests
    {
        public JsonParameterTests()
        {
        }

        [TestMethod]
        public void JsonParameter_Constructor()
        {
            JsonParameters p = new JsonParameters();
        }

        [TestMethod]
        public void JsonParameter_RegisterCustomType_Delegate()
        {
            JsonParameters parameters = new JsonParameters();
            parameters.RegisterCustomType(typeof(CustomTypeClass.CustomType), (obj) => { return "yes"; }, (obj) => { return null; });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void JsonParameter_RegisterCustomType_Delegate_NullType()
        {
            JsonParameters parameters = new JsonParameters();
            parameters.RegisterCustomType(null, (obj) => { return "yes"; }, (obj) => { return null; });
        }

        [TestMethod]
        public void JsonParameter_RegisterCustomType_Delegate_NullSerialization()
        {
            JsonParameters parameters = new JsonParameters();
            parameters.RegisterCustomType(typeof(CustomTypeClass.CustomType), null, (obj) => { return null; });
        }

        [TestMethod]
        public void JsonParameter_RegisterCustomType_Delegate_NullDeserialization()
        {
            JsonParameters parameters = new JsonParameters();
            parameters.RegisterCustomType(typeof(CustomTypeClass.CustomType), (obj) => { return "yes"; }, null);
        }

        [TestMethod]
        public void JsonParameter_RegisterCustomType_Handler()
        {
            JsonParameters parameters = new JsonParameters();
            parameters.RegisterCustomType(new CustomTypeSerializerFake());
        }

        
    }
}
