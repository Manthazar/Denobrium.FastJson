using Denobrium.Json.Registry;
using Denobrium.Json.Tests.Helpers.Polymorphism;
using Denobrium.Json.Tests.Helpers.RegistryHelpers;
using System.Runtime.Serialization;

namespace Denobrium.Json.Tests.Registry
{
    [TestClass]
    public class JsonRegistryTests
    {
        [TestMethod]
        public void JsonRegistry_Constructor()
        {
            new JsonRegistry(new JsonParameters());
        }

        [TestMethod]
        public void JsonRegistry_GetPropertiesAndFields()
        {
            var registry = new JsonRegistry(new JsonParameters());
            registry.GetPropertiesAndFields(typeof(VisibilityScopeClass), null);
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void JsonRegistry_GetPropertiesAndFields_DuplicateName()
        {
            var registry = new JsonRegistry(new JsonParameters());
            registry.GetPropertiesAndFields(typeof(DuplicateMemberNameClass), null);
        }

        [TestMethod]
        public void JsonRegistry_GetPropertiesAndFields_PublicGetSet()
        {
            var registry = new JsonRegistry(new JsonParameters());
            var members= registry.GetPropertiesAndFields(typeof(VisibilityScopeClass), null);

            Assert.IsTrue(members.ContainsKey("PublicGetSet"));

            var member = members["PublicGetSet"];
            Assert.IsNotNull(member.Getter);
            Assert.IsNotNull(member.Setter);
        }

        [TestMethod]
        public void JsonRegistry_GetPropertiesAndFields_NoGetPublicSet()
        {
            var registry = new JsonRegistry(new JsonParameters());
            var members= registry.GetPropertiesAndFields(typeof(VisibilityScopeClass), null);

            Assert.IsTrue(members.ContainsKey("NoGetPublicSet"));

            var member = members["NoGetPublicSet"];
            Assert.IsNull(member.Getter);
            Assert.IsNotNull(member.Setter);
        }

        [TestMethod]
        public void JsonRegistry_GetPropertiesAndFields_PublicGetInternalSet()
        {
            var registry = new JsonRegistry(new JsonParameters());
            var members= registry.GetPropertiesAndFields(typeof(VisibilityScopeClass), null);

            Assert.IsTrue(members.ContainsKey("PublicGetInternalSet"));

            var member = members["PublicGetInternalSet"];
            Assert.IsNotNull(member.Getter);
            Assert.IsNull(member.Setter, "Internals setters are not supported.");
        }

        [TestMethod]
        public void JsonRegistry_GetPropertiesAndFields_InternalGetSet()
        {
            var registry = new JsonRegistry(new JsonParameters());
            var members= registry.GetPropertiesAndFields(typeof(VisibilityScopeClass), null);

            Assert.IsFalse(members.ContainsKey("InternalGetSet"));
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void JsonRegistry_CreateInstanceFast_NoDefaultConstructor()
        {
            var registry = new JsonRegistry(new JsonParameters());
            registry.CreateInstanceFast(typeof(ClassWithoutDefaultConstructor));
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void JsonRegistry_CreateInstanceFast_AbstractClass()
        {
            var registry = new JsonRegistry(new JsonParameters());
            registry.CreateInstanceFast(typeof(Animal));
        }

        [TestMethod]
        public void JsonRegistry_CreateInstanceFast_NonPublicClass()
        {
            var registry = new JsonRegistry(new JsonParameters());
            var instance = registry.CreateInstanceFast(typeof(InternalPublicClass));
        }
    }
}
