using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers
{
    [DataContract]
    public class UnsignedLongClass
    {
        [DataMember]
        public ulong Long
        {
            get;
            set;
        }

        [DataMember]
        public ulong? NullableLong
        {
            get;
            set;
        }
    }
}
