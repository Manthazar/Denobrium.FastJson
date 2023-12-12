using Denobrium.Json.Benchmark.DataObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Denobrium.Json.Benchmark.Workers
{
    internal class NewtonsoftBenchmarks
    {
        /// <summary>
        /// Verifies the speed of deserializing into a data class.
        /// </summary>
        internal class Deserialize_Into_DataClass : DefaultDeserializationWorker
        {
            protected override string Name => "Newtonsoft.Json.Deserialize.IntoClass";

            private JsonSerializerSettings settings;

            protected override void Initialize()
            {
                base.Initialize();

                settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.Auto;
                settings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                };
            }

            protected override string CreateContext()
            {
                var data = BenchmarkDataFactory.CreateDefaultDataClass();
                var json = JsonConvert.SerializeObject(data, settings);

                return json;
            }

            protected override void DoStuff(string jsonText)
            {
                // Note that the results are NOT comparable because the result object doesn't contain all expected
                // data.
                var dataObject = JsonConvert.DeserializeObject<BenchmarkDataClass>(jsonText, settings);
            }
        }

        /// <summary>
        /// Verifies the speed of deserializing into a data class.
        /// </summary>
        internal class Serialize : DefaultSerializationWorker
        {
            protected override string Name => "Newtonsoft.Json.Serialize";

            private JsonSerializerSettings settings;

            protected override void Initialize()
            {
                base.Initialize();

                settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.Auto;
                settings.ContractResolver = new DefaultContractResolver()
                {
                     NamingStrategy = new CamelCaseNamingStrategy(),
                };
            }

            protected override void DoStuff(BenchmarkDataClass data)
            {
                var json = JsonConvert.SerializeObject(data, typeof(BenchmarkDataClass), settings);
            }
        }
    }
}
