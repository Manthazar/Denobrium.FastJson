using System;
using System.Collections;
using System.Collections.Generic;

namespace Denobrium.Json.Data
{
    /// <summary>
    /// Represents a list of values.
    /// </summary>
    /// <remarks>
    /// For performance reasons, the best is to inherit from dictionary directly. Wrapping is costly.
    /// </remarks>
    public sealed class JsonArray : List<IJsonValue>, IJsonValue
    {
        /// <summary>
        /// Creates an instance of the array.
        /// </summary>
        internal JsonArray()
        {
        }

        /// <summary>
        /// Gets json array.
        /// </summary>
        public JsonType Type
        {
            get { return JsonType.JsonArray; }
        }

        /// <summary>
        /// Returns a descriptive string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("JsonArray Count={0}", Count);
        }

        #region Not supported on this class

        /// <summary>
        /// Throws NotSupportedException.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IJsonValue this[string key]
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Throws NotSupportedException.
        /// </summary>
        string IJsonValue.AsString
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Throws NotSupportedException.
        /// </summary>
        int IJsonValue.AsInteger
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Throws NotSupportedException.
        /// </summary>
        double IJsonValue.AsDouble
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Throws NotSupportedException.
        /// </summary>
        Guid IJsonValue.AsGuid
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Throws a NotSupportedException
        /// </summary>
        bool IJsonValue.AsBool
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Throws a NotSupportedException
        /// </summary>
        DateTime IJsonValue.AsDateTime
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Throws a NotSupportedException
        /// </summary>
        float IJsonValue.AsSingle
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Throws a NotSupportedException
        /// </summary>
        long IJsonValue.AsLong
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Throws a NotSupportedException
        /// </summary>
        short IJsonValue.AsShort
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Throws a NotSupportedException
        /// </summary>
        TimeSpan IJsonValue.AsTimeSpan
        {
            get { throw new NotSupportedException(); }
        }

        #endregion
    }
}
