namespace Denobrium.Json.Benchmark.Workers
{
    internal abstract class DefaultDeserializationWorker : Worker<string>
    {
        protected override string CreateContext()
        {
            // Type information needs to be in the json string, otherwise readobject doesn't work.
            Denobrium.Json.Json.Current.DefaultParameters.UseTypeExtension = true;

            var dataClass = BenchmarkDataFactory.CreateDefaultDataClass();
            var jsonText = Denobrium.Json.Json.Current.ToJson(dataClass);

            return jsonText;
        }
    }
}
