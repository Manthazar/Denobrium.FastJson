using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Denobrium.Json.TestsHelpers.ParameterRelated
{
    [DataContract]
    public class OptInClass
    {
        [DataMember]
        public int Value
        {
            get;
            set;
        }

        public int NotSerialized
        {
            get;
            set;
        }
    }
}
