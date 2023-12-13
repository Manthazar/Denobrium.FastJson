using Denobrium.Json.Benchmark.DataObjects;
using System.Text.Json;

namespace Denobrium.Json.Benchmark.Workers
{
    internal class SystemTextJsonBenchmarks
    {
        /// <summary>
        /// Verifies the speed of deserializing into a data class.
        /// </summary>
        internal class Deserialize_Into_DataClass : DefaultDeserializationWorker
        {
            protected override string Name => "Net.Json.Deserialize.IntoClass";

            private JsonSerializerOptions options;

            protected override void Initialize()
            {
                base.Initialize();

                options = new()
                {
                    PropertyNameCaseInsensitive = true,
                };
            }

            protected override string CreateContext()
            {
                var data = BenchmarkDataFactory.CreateDefaultDataClass();
                var jsonString = System.Text.Json.JsonSerializer.Serialize(data, options);
                return jsonString;
            }

            protected override void DoStuff(string jsonText)
            {
                // Note that the results are NOT comparable because the result object doesn't contain all expected
                // data.
                var dataObject = System.Text.Json.JsonSerializer.Deserialize<BenchmarkDataClass>(jsonText, options);
            }
        }

        /// <summary>
        /// Verifies the speed of deserializing into a data class.
        /// </summary>
        internal class Serialize : DefaultSerializationWorker
        {
            protected override string Name => "Net.Json.Serialize      ";

            private JsonSerializerOptions options;

            protected override void Initialize()
            {
                base.Initialize();

                options = new()
                { 
                     PropertyNameCaseInsensitive= true,
                };
            }

            protected override void DoStuff(BenchmarkDataClass data)
            {
                var jsonString = System.Text.Json.JsonSerializer.Serialize(data, options);
            }
        }
    }
}
