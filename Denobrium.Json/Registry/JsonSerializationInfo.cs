using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denobrium.Json.Registry
{
    /// <summary>
    /// Represents a class which contains serialization information.
    /// </summary>
    public sealed class JsonSerializationInfo
    {
        /// <summary>
        /// Gets the type of the member which is serialized.
        /// </summary>
        public Type MemberType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the displayed member type (contains for instance generic arguments).
        /// </summary>
        public String MemberDisplayType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the member display name.
        /// </summary>
        public String MemberDisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the field or property which is serialized.
        /// </summary>
        public String MemberName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the member name with its declaring type, if possible. (eg. SomeClass.SomeProperty).
        /// </summary>
        public Type DeclaringType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the name of the member within the (de-)serialization stream.
        /// </summary>
        public String JsonFieldName
        {
            get;
            internal set;
        }
    }
}
