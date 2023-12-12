using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Apolyton.FastJson.TestsHelpers.ParameterRelated
{
    public class OptOutClass
    {
        public int Value
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public int NotSerialized
        {
            get;
            set;
        }
    }
}
