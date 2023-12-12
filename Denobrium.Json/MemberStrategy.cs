using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denobrium.Json
{
    /// <summary>
    /// Determines the serialization policy.
    /// </summary>
    [Flags]
    public enum MemberStrategy
    {
        /// <summary>
        /// Only public, non-static properties with Serializable or DataMember attribute are (de-serialized)
        /// </summary>
        PropertyOptIn = 1,

        /// <summary>
        /// All public, non-static instance properties are (de-)serialized. Default.
        /// </summary>
        PropertyOptOut = 2,

        /// <summary>
        /// All public non-static instance properties are serialized, read-only properties are not deserialized (obviously).
        /// </summary>
        [Obsolete("Use Serialization.IncludeReadOnly insead", true)]
        IncludeReadOnly = 4,

        /// <summary>
        /// All publis non-static fields are also participating in the serialization process.
        /// </summary>
        [Obsolete("Not implemented yet, always true")]
        IncludeFields = 8
    }
}
