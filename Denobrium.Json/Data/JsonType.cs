using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denobrium.Json.Data
{
    /// <summary>
    /// Represents a public enumeration of json types.
    /// </summary>
    public enum JsonType
    {
        /// <summary>
        /// A primitive value. Typically a value type like int, float, etc.
        /// </summary>
        JsonPrimitive,

        /// <summary>
        /// A list of values
        /// </summary>
        JsonArray,

        /// <summary>
        /// An object represented as key/ value pairs.
        /// </summary>
        JsonObject
    }
}
