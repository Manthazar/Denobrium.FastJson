using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Denobrium.Json.Registry;
using Denobrium.Json.Tests.Helpers.ComplexTypes;
using Denobrium.Json.Tests.Helpers.Polymorphism;

namespace Denobrium.Json.Tests.Registry
{
    /// <summary>
    /// Summary description for TypeDescriptorTests
    /// </summary>
    [TestClass]
    public class JsonTypeDescriptorTests
    {
        public JsonTypeDescriptorTests()
        {
        }

        [TestMethod]
        public void JsonTypeDescriptor_Constructor()
        {
            JsonTypeDescriptor descriptor = new JsonTypeDescriptor();
        }

        [TestMethod]
        public void JsonTypeDescriptor_Initialize()
        {
            InitTypeDescriptor descriptor = new InitTypeDescriptor();
            Assert.AreEqual(true, descriptor.WasInitialized);
        }

        [TestMethod]
        public void JsonTypeDescriptor_GetType()
        {
            JsonTypeDescriptor descriptor = new JsonTypeDescriptor();

            Type result = descriptor.GetType(typeof(Object).AssemblyQualifiedName);
            Assert.AreEqual(typeof(Object), result);
        }

        [TestMethod]
        public void JsonTypeDescriptor_GetType_Abstract()
        {
            TypeDescriptorMock descriptor = new TypeDescriptorMock();
            descriptor.ResolvedType = typeof(Animal);

            Type result = descriptor.GetType("Somestring");
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void JsonTypeDescriptor_GetType_NoDefaultConstructor()
        {
            Assert.Inconclusive("Only a problem when deserializing.");

            TypeDescriptorMock descriptor = new TypeDescriptorMock();
            descriptor.ResolvedType = typeof(ClassWithoutDefaultConstructor);

            Type result = descriptor.GetType("Somestring");
        }

        public class ClassWithoutDefaultConstructor
        {
            public ClassWithoutDefaultConstructor(int t) { }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void JsonTypeDescriptor_GetType_NotPublic()
        {
            Assert.Inconclusive("For silverlight only");

            TypeDescriptorMock descriptor = new TypeDescriptorMock();
            descriptor.ResolvedType = typeof(NonPublicClass);

            Type result = descriptor.GetType("Somestring");
        }

        private class NonPublicClass
        {
        }


        [TestMethod]
        public void JsonTypeDescriptor_GetTypeName()
        {
            JsonTypeDescriptor descriptor = new JsonTypeDescriptor();

            String typeName = descriptor.GetTypeName(typeof(String));
            Assert.AreEqual(typeof(String).AssemblyQualifiedName, typeName);
        }

        [TestMethod]
        public void JsonTypeDescriptor_Register()
        {
            JsonTypeDescriptor descriptor = new JsonTypeDescriptor();

            descriptor.Register(typeof(Object), "Tomo");

            Assert.AreEqual(typeof(Object), descriptor.GetType("Tomo"));
            Assert.AreEqual("Tomo", descriptor.GetTypeName(typeof(Object)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void JsonTypeDescriptor_Register_Twice()
        {
            JsonTypeDescriptor descriptor = new JsonTypeDescriptor();

            descriptor.Register(typeof(Object), "Tomo");
            descriptor.Register(typeof(Object), "Tada");
        }

        [TestMethod]
        public void JsonTypeDescriptor_Initialize_AfterReset()
        {
            InitTypeDescriptor descriptor = new InitTypeDescriptor();

            descriptor.WasInitialized = false;
            descriptor.Reset();
            Assert.AreEqual(true, descriptor.WasInitialized);
        }

        public class InitTypeDescriptor : JsonTypeDescriptor
        {
            public bool WasInitialized = false;

            protected override void Initialize()
            {
                WasInitialized = true;

                base.Initialize();
            }
        }

        public class TypeDescriptorMock : JsonTypeDescriptor
        {
            public Type ResolvedType = null;

            protected override Type ResolveType(string typeName)
            {
                return ResolvedType;
            }
        }

        /// <summary>
        /// This test verifies, if the result of the resolution methods is cached properly. 
        /// </summary>
        [TestMethod]
        public void JsonTypeDescriptor_Caching_GetTypeName()
        {
            JsonTypeDescriptor_Accessor descriptor = new JsonTypeDescriptor_Accessor();

            Assert.AreEqual(0, descriptor.Count);
            descriptor.GetTypeName(typeof(String));
            Assert.AreEqual(1, descriptor.Count);

            descriptor.GetTypeName(typeof(String));
            Assert.AreEqual(1, descriptor.Count, "Additional calls to the gettypename should not have resulted in calling the resolution method");
        }

        /// <summary>
        /// This test verifies, if the result of the resolution methods is cached properly. 
        /// </summary>
        [TestMethod]
        public void JsonTypeDescriptor_Caching_ResolveType()
        {
            JsonTypeDescriptor_Accessor descriptor = new JsonTypeDescriptor_Accessor();

            Assert.AreEqual(0, descriptor.Count);
            descriptor.GetType(typeof(Object).AssemblyQualifiedName);
            Assert.AreEqual(1, descriptor.Count);

            descriptor.GetType(typeof(Object).AssemblyQualifiedName);
            Assert.AreEqual(1, descriptor.Count, "Additional calls to the gettypename should not have resulted in calling the resolution method");
        }

        /// <summary>
        /// This test verifies, if the result of the resolution methods is cached properly. 
        /// </summary>
        [TestMethod]
        public void JsonTypeDescriptor_Caching_AfterReset()
        {
            JsonTypeDescriptor_Accessor descriptor = new JsonTypeDescriptor_Accessor();

            Assert.AreEqual(0, descriptor.Count);
            descriptor.GetTypeName(typeof(String));
            Assert.AreEqual(1, descriptor.Count);

            descriptor.Reset();
            descriptor.GetTypeName(typeof(String));
            Assert.AreEqual(2, descriptor.Count);
        }

        /// <summary>
        /// This test verifies, if the result of the resolution methods is cached properly. 
        /// </summary>
        [TestMethod]
        public void JsonTypeDescriptor_TryGetType()
        {
            JsonTypeDescriptor descriptor = new JsonTypeDescriptor();
            Type type = null;
            String typeName = this.GetType().AssemblyQualifiedName;

            var found = descriptor.TryGetType(typeName, out type);

            Assert.IsTrue(found);
            Assert.IsNotNull(type);
            Assert.AreEqual(this.GetType(), type);
        }

        [TestMethod]
        public void JsonTypeDescriptor_TryGetType_Null()
        {
            JsonTypeDescriptor descriptor = new JsonTypeDescriptor();

            Type type = null;
            Assert.IsFalse(descriptor.TryGetType(null, out type));
        }

        public class JsonTypeDescriptor_Accessor : JsonTypeDescriptor
        {
            public int Count = 0;

            protected override string CreateTypeName(Type type)
            {
                Count++;
                return base.CreateTypeName(type);
            }

            protected override Type ResolveType(string typeName)
            {
                Count++;
                return base.ResolveType(typeName);
            }
        }
    }
}
