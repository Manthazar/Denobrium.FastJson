using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers
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
