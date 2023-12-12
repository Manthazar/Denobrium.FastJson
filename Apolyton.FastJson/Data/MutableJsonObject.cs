using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Apolyton.FastJson.Data
{
    /// <summary>
    /// Represents a mutable json object with already pre-parsed values like int32, double, classes etc.
    /// </summary>
    /// <remarks>
    /// Used by the HybridDeserializer.
    /// </remarks>
    internal class MutableJsonObject : Dictionary<String, object>
    {
        /// <summary>
        /// Creates an instance of the json object.
        /// </summary>
        public MutableJsonObject()
        {
        }
    }
}
