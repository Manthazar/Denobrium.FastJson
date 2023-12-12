using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers
{
    [DataContract]
    public class UnsignedShortClass
    {
        [DataMember]
        public ushort Short
        {
            get;
            set;
        }

        [DataMember]
        public ushort? NullableShort
        {
            get;
            set;
        }
    }
}
