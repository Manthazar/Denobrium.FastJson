using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Denobrium.Json.TestsHelpers.ParameterRelated
{
    [DataContract]
    public class ReadOnlyClass
    {
        private int readOnlyValue;

        public ReadOnlyClass(int value, int readOnlyValue)
        {
            Value = value;
            this.readOnlyValue = readOnlyValue;
        }

        /// <summary>
        /// For reflection and trust level, this might not be a read-only property!
        /// </summary>
        [DataMember]
        public int Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Definitelly read-only.
        /// </summary>
        [DataMember]
        public int ReadOnlyValue
        {
            get { return readOnlyValue; }            
        }
    }
}
