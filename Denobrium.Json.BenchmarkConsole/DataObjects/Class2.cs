using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace consoletest.DataObjects
{
    [DataContract(Name = "Class2")]
    public class Class2 : BaseClass
    {
        public Class2() { }

        public Class2(string name, string code, string desc)
        {
            Name = name;
            Code = code;
            Description = desc;
        }

        public string Description { get; set; }
    }
}
