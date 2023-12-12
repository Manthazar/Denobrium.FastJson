using System;
using Apolyton.FastJson.Registry;
using Apolyton.FastJson.Tests.Helpers.Polymorphism;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Apolyton.FastJson.Tests.Registry
{
    /// <summary>
    /// Summary description for DataContractTypeDescriptorTests
    /// </summary>
    [TestClass]
    public class DataContractTypeDescriptorTests
    {
        [TestMethod]
        public void DataContractTypeDescriptor_Constructor()
        {
            DataContractTypeDescriptor descriptor = new DataContractTypeDescriptor();
        }

        [TestMethod]
        public void DataContractTypeDescriptor_Constructor_Assembly()
        {
            DataContractTypeDescriptor descriptor = new DataContractTypeDescriptor(this.GetType().Assembly);

            Assert.AreEqual(typeof(Cat), descriptor.GetType("kitty"));// Note that no explicit call to collect is done.
        }

        [TestMethod]
        public void DataContractTypeDescriptor_Collect()
        {
            DataContractTypeDescriptor descriptor = new DataContractTypeDescriptor();
            descriptor.CollectDataContracts(this.GetType().Assembly);

            Assert.AreEqual(typeof(Cat), descriptor.GetType("kitty"));
        }

        [TestMethod]
        public void DataContractTypeDescriptor_GetTypeName()
        {
            DataContractTypeDescriptor descriptor = new DataContractTypeDescriptor();
            String typeName = descriptor.GetTypeName(typeof(Cat));

            Assert.AreEqual("kitty", typeName);
        }
    }
}
