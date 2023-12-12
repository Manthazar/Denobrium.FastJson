using System;
using System.Runtime.Serialization;

namespace Denobrium.Json.Benchmark.DataObjects
{
    [DataContract(Name= "Product")]
    [KnownType(typeof(Knife))]
    [KnownType(typeof(Spoon))]
    [Serializable]
    public class Product
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
