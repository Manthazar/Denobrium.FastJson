using System.Runtime.Serialization;

namespace Denobrium.Json.Benchmark.DataObjects
{
    [DataContract(Name = "Gender")]
    public enum Gender
    {
        Male,
        Female
    }
}
