using System;

namespace Apolyton.FastJson.Serialization
{
    /// <summary>
    /// Specifies serialization options for a DateTime field or property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class JsonDateTimeOptionsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the BsonDateTimeOptionsAttribute class.
        /// </summary>
        public JsonDateTimeOptionsAttribute()
        {
        }

        /// <summary>
        /// Gets or sets the DateTimeKind (Local, Unspecified or Utc).
        /// </summary>
        public DateTimeKind Kind { get; set; }

        /// <summary>
        /// Gets or sets the format string of the date time field.
        /// </summary>
        public String Format{get;set;}
    }
}
