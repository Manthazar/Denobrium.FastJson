using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.TestsHelpers.StandardTypes
{
    [DataContract]
    public class StringClass
    {
        [DataMember]
        public String String
        {
            get;
            set;
        }
    }
}
