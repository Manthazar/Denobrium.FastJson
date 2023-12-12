using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Denobrium.Json.Tests.Helpers
{
    public class NestedClass
    {
        [DataMember]
        public int Integer
        {
            get;
            set;
        }
    }
}
