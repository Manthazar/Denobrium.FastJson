using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers
{
    [DataContract]
    public class UnsignedIntClass
    {
        [DataMember]
        public uint Integer
        {
            get;
            set;
        }

        [DataMember]
        public uint? NullableInteger
        {
            get;
            set;
        }
    }
}
