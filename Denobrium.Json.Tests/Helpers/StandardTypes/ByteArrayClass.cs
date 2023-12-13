using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace Denobrium.Json.TestsHelpers.StandardTypes
{
    [DataContract]
    public class ByteArrayClass
    {
        [DataMember]
        public byte[]? ByteArray
        {
            get;
            set;
        }

        [DataMember]
        public IEnumerable<Byte>? ByteEnumeration
        {
            get;
            set;
        }
    }
}
