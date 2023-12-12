using System;
using System.Runtime.Serialization;

namespace Denobrium.Json.Benchmark.DataObjects
{
    [DataContract(Name = "Gender")]
    [Serializable]
    public enum Gender
    {
        Male,
        Female
    }
}
