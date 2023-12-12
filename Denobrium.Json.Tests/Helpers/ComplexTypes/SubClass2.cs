using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apolyton.FastJson.Tests.Helpers.ComplexTypes
{
    public class SubClass2 : BaseClass
    {
        public SubClass2()
        { }

        public SubClass2(string name, string code, string desc)
        {
            Name = name;
            Code = code;
            description = desc;
        }

        public string description { get; set; }
    }
}
