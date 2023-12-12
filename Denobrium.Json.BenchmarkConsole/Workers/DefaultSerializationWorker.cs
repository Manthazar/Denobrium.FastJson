using Denobrium.Json.Benchmark.DataObjects;

namespace Denobrium.Json.Benchmark.Workers
{
    internal abstract class DefaultSerializationWorker : Worker<BenchmarkDataClass>
    {
        protected override BenchmarkDataClass CreateContext()
        {
            // Type information needs to be in the json string, otherwise readobject doesn't work.
            Denobrium.Json.Json.Current.DefaultParameters.UseTypeExtension = true;

            var dataClass = BenchmarkDataFactory.CreateDefaultDataClass();
            return dataClass;
        }
    }
}
