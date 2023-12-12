using System;
using System.Runtime.Serialization;

namespace Denobrium.Json.Benchmark.DataObjects
{
    [DataContract(Name = "Class2")]
    [Serializable]
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
