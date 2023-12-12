using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apolyton.FastJson.Tests.Helpers;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers.StandardTypes
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
