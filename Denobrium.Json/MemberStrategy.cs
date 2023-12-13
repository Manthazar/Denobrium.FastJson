using System;

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
        /// All publis non-static fields are also participating in the serialization process.
        /// </summary>
        [Obsolete("Not implemented yet, always true")]
        IncludeFields = 8
    }
}
