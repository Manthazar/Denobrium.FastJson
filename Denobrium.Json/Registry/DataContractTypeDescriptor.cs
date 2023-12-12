using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Denobrium.Json.Registry
{
    /// <summary>
    /// Represents a type descriptor which describes types through their DataContract attribute, if present. 
    /// Otherwise, the full type name is used.
    /// </summary>
    public class DataContractTypeDescriptor : JsonTypeDescriptor
    {
        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        public DataContractTypeDescriptor()
        {
        }

        /// <summary>
        /// Creates an instance of the class and collects the data contract types from the given list of assemblies
        /// </summary>
        public DataContractTypeDescriptor(params Assembly[] assembliesToCollectFrom)
        {
            foreach (Assembly assembly in assembliesToCollectFrom)
            {
                CollectDataContracts(assembly);
            }
        }

        /// <summary>
        /// Collects and registers all types decorated with DataContract attribute. If the Name property is set, that type name will be used.
        /// Otherwise, the type name is used.
        /// <param name="assembly"></param>
        /// <exception cref="DuplicateKeyException">If type name is duplicate.</exception>
        public void CollectDataContracts(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes().Where(t => t.IsDefined(typeof(DataContractAttribute), false)))
            {
                String name = CreateTypeName(type);
                Register(type, name);
            }
        }

        /// <summary>
        /// Returns the first data contract name of the type or the full name of it. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected override string CreateTypeName(Type type)
        {
            var contract = type.GetCustomAttributes(typeof(DataContractAttribute), false).Cast<DataContractAttribute>().FirstOrDefault();

            if (contract != null && !String.IsNullOrEmpty(contract.Name))
            {
                if (String.IsNullOrEmpty(contract.Namespace))
                {
                    return contract.Name;
                }
                else
                {
                    return String.Concat(contract.Namespace, "/", contract.Name);
                }
                
            }

            return type.FullName;
        }
    }
}
