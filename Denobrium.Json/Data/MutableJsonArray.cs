using System;
using System.Collections;
using System.Collections.Generic;

namespace Denobrium.Json.Data
{
    /// <summary>
    /// Represents a json array with already pre-parsed values like int32, double, classes etc.
    /// </summary>
    /// <remarks>
    /// Used by the HybridDeserializer.
    /// </remarks>
    internal class MutableJsonArray : List<Object>
    {
        /// <summary>
        /// Creates an instance of the array.
        /// </summary>
        public MutableJsonArray()
        {
        }
    }
}
