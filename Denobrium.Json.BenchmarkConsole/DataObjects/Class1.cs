using System;
using System.Runtime.Serialization;

namespace Denobrium.Json.Benchmark.DataObjects
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
