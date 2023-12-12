using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.TestsHelpers.StandardTypes
{
    [DataContract]
    public class BoolClass
    {
        [DataMember]
        public bool Bool
        {
            get;
            set;
        }

        [DataMember]
        public bool? NullableBool
        {
            get;
            set;
        }
    }
}
