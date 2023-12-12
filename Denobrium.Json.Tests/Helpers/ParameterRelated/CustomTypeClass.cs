using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apolyton.FastJson.Tests.Helpers.ParameterRelated
{
    public class CustomTypeClass
    {
        public CustomType Custom
        {
            get;
            set;
        }

        public class CustomType
        {
            public object Value
            {
                get;
                set;
            }
        }
    }
}
