using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denobrium.Json.Serialization
{
    /// <summary>
    /// Represents an interface for a custom type serializer.
    /// </summary>
    public interface ICustomTypeSerializer
    {
        /// <summary>
        /// Gets the type for which serializer is responsible for.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the custom name of the type. Returns null, if default is to be taken.
        /// </summary>
        String TypeName { get; }

        /// <summary>
        /// Gets the bool which indicates whether the custom type serializer implements a serialization method.
        /// </summary>
        bool CanSerialize { get; }

        /// <summary>
        /// Gets the bool which indicates whether the custom type serializer implements a deserialization method.
        /// </summary>
        bool CanDeserialize { get; }

        /// <summary>
        /// Represents a serialization method which returns a json-value-string for the given data object.
        /// </summary>
        /// <example>
        /// if (data != null) { return "Yes"; } else { return "No"; }
        /// </example>
        string Serialize (object data);

        /// <summary>
        /// Represents a serialization handler which returns a deserialized object for the given data string.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <example>
        /// if (jsonValueString == "Yes") { return true; } else { return false; }
        /// </example>
        object Deserialize(string jsonValueString);
    }
}
