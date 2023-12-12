using Denobrium.Json.Benchmark;
using Denobrium.Json.Benchmark.DataObjects;

namespace Denobrium.Json.Benchmark.Workers
{
    internal class ApolytonFastJsonBenchmarks 
    {
        /// <summary>
        /// Verifies the speed of deserializing into a data class.
        /// </summary>
        internal class Deserialize_Into_DataClass : BenchmarkWorker<string>
        {
            protected override string Name => "Denobrium.Json.Deserialize.IntoClass";

            protected override string CreateContext()
            {
                // Type information needs to be in the json string, otherwise readobject doesn't work.
                Denobrium.Json.Json.Current.DefaultParameters.UseTypeExtension = true;

                var dataClass = BenchmarkDataFactory.CreateDefaultDataClass();
                var jsonText = Denobrium.Json.Json.Current.ToJson(dataClass);

                Denobrium.Json.Json.Current.DefaultParameters.UseTypeExtension = false;

                return jsonText;
            }

            protected override void DoStuff(string jsonText)
            {
                var dataObject = (BenchmarkDataClass)Denobrium.Json.Json.Current.ReadObject(jsonText);
            }
        }
    }
}
