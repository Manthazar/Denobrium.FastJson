using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace consoletest.DataObjects
{
    [DataContract(Name = "Class1")]
    public class Class1 : BaseClass
    {
        public Class1() { }

        public Class1(string name, string code, Guid g)
        {
            Name = name;
            Code = code;
            guid = g;
        }

        public Guid guid { get; set; }
    }
}
