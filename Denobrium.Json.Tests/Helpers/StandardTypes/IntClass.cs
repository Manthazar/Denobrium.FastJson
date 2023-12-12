using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Denobrium.Json.Tests.Helpers
{
    [DataContract]
    public class IntClass
    {
        [DataMember]
        public int Integer
        {
            get;
            set;
        }

        [DataMember]
        public int? NullableInteger
        {
            get;
            set;
        }
    }
}
