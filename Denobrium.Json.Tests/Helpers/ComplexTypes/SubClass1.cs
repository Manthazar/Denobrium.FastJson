using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apolyton.FastJson.Tests.Helpers.ComplexTypes
{
    public class SubClass1 : BaseClass
    {
        public SubClass1()
        { }

        public SubClass1(string name, string code, Guid g)
        {
            Name = name;
            Code = code;
            guid = g;
        }

        public Guid guid { get; set; }
    }
}
