using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;

namespace Denobrium.Json.TestsHelpers.StandardTypes
{
    [DataContract]
    public class EnumerableClass
    {
        [DataMember]
        public IEnumerable? Enumeration
        {
            get;
            set;
        }
    }
}
