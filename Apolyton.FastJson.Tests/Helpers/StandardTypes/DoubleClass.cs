﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers
{
    [DataContract]
    public class DoubleClass
    {
        [DataMember]
        public double Double
        {
            get;
            set;
        }

        [DataMember]
        public double? NullableDouble
        {
            get;
            set;
        }
    }
}
