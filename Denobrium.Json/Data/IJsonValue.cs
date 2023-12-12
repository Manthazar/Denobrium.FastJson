using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Denobrium.Json.Data;
using System.Diagnostics;

namespace Denobrium.Json.Data
{
    /// <summary>
    /// Represents a json object.
    /// </summary>
    public interface IJsonValue
    {
        /// <summary>
        /// Gets the count of the members.
        /// </summary>
        /// <exception cref="NotSupportedException">If item is not enumerable and has no count.</exception>
        int Count { get; }

        /// <summary>
        /// Gets the json value type.
        /// </summary>
        JsonType Type { get; }

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">If item is not a json array.</exception>
        IJsonValue this[int i] { get; }

        /// <summary>
        /// Gets the key at the specified index.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">If item is not a json object.</exception>s
        IJsonValue this[String key] { get; }

        /// <summary>
        /// Gets the value as bool.
        /// </summary>
        /// <exception cref="NotSupportedException">If json value does not support this conversion</exception>
        /// <exception cref="InvalidCastException">If json value failed in trying to cast its interal value to this type.</exception>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        Boolean AsBool{get;}

        /// <summary>
        /// Gets the value as bool.
        /// </summary>
        /// <exception cref="NotSupportedException">If json value does not support this conversion</exception>
        /// <exception cref="InvalidCastException">If json value failed in trying to cast its interal value to this type.</exception>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        DateTime AsDateTime { get; }

        /// <summary>
        /// Gets the value as double precision float.
        /// </summary>
        /// <exception cref="NotSupportedException">If json value does not support this conversion</exception>
        /// <exception cref="InvalidCastException">If json value failed in trying to cast its interal value to this type.</exception>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        Double AsDouble { get; }

        /// <summary>
        /// Gets the value as single precision float.
        /// </summary>
        /// <exception cref="NotSupportedException">If json value does not support this conversion</exception>
        /// <exception cref="InvalidCastException">If json value failed in trying to cast its interal value to this type.</exception>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        float AsSingle{ get; }

        /// <summary>
        /// Gets the value as guid.
        /// </summary>
        /// <exception cref="NotSupportedException">If json value does not support this conversion</exception>
        /// <exception cref="InvalidCastException">If json value failed in trying to cast its interal value to this type.</exception>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        Guid AsGuid { get; }

        /// <summary>
        /// Gets the value as integer.
        /// </summary>
        /// <exception cref="NotSupportedException">If json value does not support this conversion</exception>
        /// <exception cref="InvalidCastException">If json value failed in trying to cast its interal value to this type.</exception>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        Int32 AsInteger { get; }

        /// <summary>
        /// Gets the value as long integer.
        /// </summary>
        /// <exception cref="NotSupportedException">If json value does not support this conversion</exception>
        /// <exception cref="InvalidCastException">If json value failed in trying to cast its interal value to this type.</exception>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        Int64 AsLong { get; }

        /// <summary>
        /// Gets the value as short integer.
        /// </summary>
        /// <exception cref="NotSupportedException">If json value does not support this conversion</exception>
        /// <exception cref="InvalidCastException">If json value failed in trying to cast its interal value to this type.</exception>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        Int16 AsShort { get; }

        /// <summary>
        /// Gets the value as string.
        /// </summary>
        /// <exception cref="NotSupportedException">If json value does not support this conversion</exception>
        /// <exception cref="InvalidCastException">If json value failed in trying to cast its interal value to this type.</exception>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        String AsString { get; }

        /// <summary>
        /// Gets the value as time span.
        /// </summary>
        /// <exception cref="NotSupportedException">If json value does not support this conversion</exception>
        /// <exception cref="InvalidCastException">If json value failed in trying to cast its interal value to this type.</exception>
        [DebuggerDisplay("Click to expand. This operation has side effects.")]
        TimeSpan AsTimeSpan { get; }
    }
}
