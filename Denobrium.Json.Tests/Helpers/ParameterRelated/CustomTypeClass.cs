using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denobrium.Json.Tests.Helpers.ParameterRelated
{
    public class CustomTypeClass
    {
        public CustomType? Custom
        {
            get;
            set;
        }

        public class CustomType
        {
            public object? Value
            {
                get;
                set;
            }
        }
    }
}
