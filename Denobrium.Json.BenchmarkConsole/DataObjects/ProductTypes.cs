using System;
using System.Runtime.Serialization;

namespace Denobrium.Json.Benchmark.DataObjects
{
    [DataContract(Name = "ProductTypes")]
    [Serializable]
    public enum ProductTypes
    {
        Knife,
        Fork,
        Spoon
    }
}
