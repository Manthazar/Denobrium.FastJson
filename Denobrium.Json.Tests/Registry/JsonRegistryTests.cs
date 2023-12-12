using System;
using System.Runtime.Serialization;
using Denobrium.Json.Registry;
using Denobrium.Json.Tests.Helpers.Polymorphism;
using Denobrium.Json.Tests.Helpers.RegistryHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Denobrium.Json.Tests.Registry
{
    [TestClass]
    public class JsonRegistryTests
    {
        [TestMethod]
        public void JsonRegistry_Constructor()
        {
            JsonRegistry registry = new JsonRegistry(new JsonParameters());
        }

        [TestMethod]
        public void JsonRegistry_GetPropertiesAndFields()
        {
            JsonRegistry registry = new JsonRegistry(new JsonParameters());
            registry.GetPropertiesAndFields(typeof(VisibilityScopeClass), null);
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void JsonRegistry_GetPropertiesAndFields_DuplicateName()
        {
            JsonRegistry registry = new JsonRegistry(new JsonParameters());
            registry.GetPropertiesAndFields(typeof(DuplicateMemberNameClass), null);
        }

        [TestMethod]
        public void JsonRegistry_GetPropertiesAndFields_PublicGetSet()
        {
            JsonRegistry registry = new JsonRegistry(new JsonParameters());
            RegistrySection<String, JsonPropertyInfo>  members = registry.GetPropertiesAndFields(typeof(VisibilityScopeClass), null);

            Assert.IsTrue(members.ContainsKey("PublicGetSet"));

            var member = members["PublicGetSet"];
            Assert.IsNotNull(member.Getter);
            Assert.IsNotNull(member.Setter);
        }

        [TestMethod]
        public void JsonRegistry_GetPropertiesAndFields_NoGetPublicSet()
        {
            JsonRegistry registry = new JsonRegistry(new JsonParameters());
            RegistrySection<String, JsonPropertyInfo> members = registry.GetPropertiesAndFields(typeof(VisibilityScopeClass), null);

            Assert.IsTrue(members.ContainsKey("NoGetPublicSet"));

            var member = members["NoGetPublicSet"];
            Assert.IsNull(member.Getter);
            Assert.IsNotNull(member.Setter);
        }

        [TestMethod]
        public void JsonRegistry_GetPropertiesAndFields_PublicGetInternalSet()
        {
            JsonRegistry registry = new JsonRegistry(new JsonParameters());
            RegistrySection<String, JsonPropertyInfo> members = registry.GetPropertiesAndFields(typeof(VisibilityScopeClass), null);

            Assert.IsTrue(members.ContainsKey("PublicGetInternalSet"));

            var member = members["PublicGetInternalSet"];
            Assert.IsNotNull(member.Getter);
            Assert.IsNull(member.Setter, "Internals setters are not supported.");
        }

        [TestMethod]
        public void JsonRegistry_GetPropertiesAndFields_InternalGetSet()
        {
            JsonRegistry registry = new JsonRegistry(new JsonParameters());
            RegistrySection<String, JsonPropertyInfo> members = registry.GetPropertiesAndFields(typeof(VisibilityScopeClass), null);

            Assert.IsFalse(members.ContainsKey("InternalGetSet"));
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void JsonRegistry_CreateInstanceFast_NoDefaultConstructor()
        {
            JsonRegistry registry = new JsonRegistry(new JsonParameters());
            registry.CreateInstanceFast(typeof(ClassWithoutDefaultConstructor));
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void JsonRegistry_CreateInstanceFast_AbstractClass()
        {
            JsonRegistry registry = new JsonRegistry(new JsonParameters());
            registry.CreateInstanceFast(typeof(Animal));
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void JsonRegistry_CreateInstanceFast_NonPublicClass()
        {
            JsonRegistry registry = new JsonRegistry(new JsonParameters());
            registry.CreateInstanceFast(typeof(NonPublicClass));
        }
    }
}
