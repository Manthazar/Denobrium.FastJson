using System.Runtime.Serialization;

namespace Denobrium.Json.Benchmark.DataObjects
{
    [DataContract(Name="Base")]
    public class BaseClass
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
