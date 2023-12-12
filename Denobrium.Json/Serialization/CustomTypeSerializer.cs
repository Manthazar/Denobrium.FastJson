using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denobrium.Json.Serialization
{
    /// <summary>
    /// Represents a base class for a custom type serializer
    /// </summary>
    public abstract class CustomTypeSerializer : ICustomTypeSerializer
    {
        /// <summary>
        /// Gets the type for which serializer is responsible for.
        /// </summary>
        public abstract Type Type 
        {
            get; 
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public virtual string TypeName
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the bool which indicates whether the custom type serializer implements a serialization method.
        /// </summary>
        public virtual bool CanSerialize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the bool which indicates whether the custom type serializer implements a deserialization method.
        /// </summary>
        public virtual bool CanDeserialize
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the custom serialized string value.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract String Serialize(object data);

        /// <summary>
        /// Returns the custom object instance which represents the given string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public abstract Object Deserialize(String jsonString);
    }
}
