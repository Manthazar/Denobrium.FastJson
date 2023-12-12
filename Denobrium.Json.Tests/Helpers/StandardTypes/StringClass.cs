using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Denobrium.Json.TestsHelpers.StandardTypes
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
