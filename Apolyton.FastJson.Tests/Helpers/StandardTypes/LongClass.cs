using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers
{
    [DataContract]
    public class LongClass
    {
        [DataMember]
        public long Long
        {
            get;
            set;
        }

        [DataMember]
        public long? NullableLong
        {
            get;
            set;
        }
    }
}
