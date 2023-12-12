using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Denobrium.Json.Tests.Helpers;
using System.Runtime.Serialization;

namespace Denobrium.Json.Tests.Helpers.StandardTypes
{
    public class NestingClass
    {
        [DataMember]
        public long Long
        {
            get;
            set;
        }

        [DataMember]
        public NestedClass NestedClass
        {
            get;
            set;
        }
    }
}
