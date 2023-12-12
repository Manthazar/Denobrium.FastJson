using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Denobrium.Json.Data
{
    /// <summary>
    /// Represents a json object.
    /// </summary>
    /// <remarks>
    /// For performance reasons, the best is to inherit from dictionary directly. Wrapping is costly.
    /// </remarks>
    public sealed class JsonObject : Dictionary<String, IJsonValue>, IJsonValue
    {
        /// <summary>
        /// Creates an instance of the json object.
        /// </summary>
        internal JsonObject()
        {
        }

        /// <summary>
        /// Returns the json object type.
        /// </summary>
        public JsonType Type
        {
            get { return JsonType.JsonObject; }
        }

        /// <summary>
        /// Returns a descriptive string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("JsonObject Count={0}", Count);
        }

        /// <summary>
        /// Tries to return the type name.
        /// </summary>
        /// <returns></returns>
        public String TryGetTypeName()
        {
            IJsonValue value = null;

            if (TryGetValue("$type", out value))
            {
                return value.AsString;
            }
            else
            {
                return null;
            }
        }

        #region Not supported on this class

        /// <summary>
        /// Throws a NotSupportedException
        /// </summary>
        IJsonValue IJsonValue.this[int i]
        {
            get { throw new NotImplementedException(); }
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
