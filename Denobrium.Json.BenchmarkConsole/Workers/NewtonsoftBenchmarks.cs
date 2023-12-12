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
        internal class Deserialize_Into_DataClass : Worker<string>
        {
            protected override string Name => "Newtonsoft.Json.Deserialize.IntoClass";

            private JsonSerializerSettings settings;

            protected override void Initialize()
            {
                base.Initialize();

                settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.All;
            }
            protected override string CreateContext()
            {
                // Type information needs to be in the json string, otherwise readobject doesn't work.
                Denobrium.Json.Json.Current.DefaultParameters.UseTypeExtension = true;

                var dataClass = BenchmarkDataFactory.CreateDefaultDataClass();
                var jsonText = Denobrium.Json.Json.Current.ToJson(dataClass);

                return jsonText;
            }

            protected override void DoStuff(string jsonText)
            {
                // Note that the results are NOT comparable because the result object doesn't contain all expected
                // data.
                var dataObject = JsonConvert.DeserializeObject(jsonText, typeof(BenchmarkDataClass), settings);
            }
        }

        /// <summary>
        /// Verifies the speed of deserializing into a data class.
        /// </summary>
        internal class Serialize : Worker<BenchmarkDataClass>
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

            protected override BenchmarkDataClass CreateContext()
            {
                // Type information needs to be in the json string, otherwise readobject doesn't work.
                Denobrium.Json.Json.Current.DefaultParameters.UseTypeExtension = true;

                var dataClass = BenchmarkDataFactory.CreateDefaultDataClass();

                return dataClass;
            }

            protected override void DoStuff(BenchmarkDataClass data)
            {
                var json = JsonConvert.SerializeObject(data, typeof(BenchmarkDataClass), settings);
            }
        }
    }
}
