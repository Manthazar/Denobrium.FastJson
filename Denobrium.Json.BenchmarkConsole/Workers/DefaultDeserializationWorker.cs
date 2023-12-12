using Denobrium.Json.Registry;

namespace Denobrium.Json.Benchmark.Workers
{
    internal abstract class DefaultDeserializationWorker : Worker<string>
    {
        protected override string CreateContext()
        {
            // Type information needs to be in the json string, otherwise readobject doesn't work.
            var jsonParameters = new JsonParameters()
            {
                UseTypeExtension = true,
            };

            jsonParameters.RegisterTypeDescriptor(new DataContractTypeDescriptor());

            var dataClass = BenchmarkDataFactory.CreateDefaultDataClass();
            var jsonText = Denobrium.Json.Json.Current.ToJson(dataClass, jsonParameters);

            return jsonText;
        }
    }
}
