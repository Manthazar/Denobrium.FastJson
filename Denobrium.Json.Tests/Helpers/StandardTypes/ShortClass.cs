using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers
{
    [DataContract]
    public class ShortClass
    {
        [DataMember]
        public short Short
        {
            get;
            set;
        }

        [DataMember]
        public short? NullableShort
        {
            get;
            set;
        }
    }
}
