using consoletest;
using consoletest.DataObjects;
using Newtonsoft.Json;

namespace Apolyton.FastJson.Benchmarks
{
    internal class NewtonsoftBenchmarks
    {
        /// <summary>
        /// Verifies the speed of deserializing into a data class.
        /// </summary>
        internal class Deserialize_Into_DataClass : BenchmarkWorker<string>
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
                Apolyton.FastJson.Json.Current.DefaultParameters.UseTypeExtension = true;

                var dataClass = BenchmarkDataFactory.CreateDefaultDataClass();
                var jsonText = Apolyton.FastJson.Json.Current.ToJson(dataClass);

                return jsonText;
            }

            protected override void DoStuff(string jsonText)
            {
                // Note that the results are NOT comparable because the result object doesn't contain all expected
                // data.
                var dataObject = JsonConvert.DeserializeObject(jsonText, typeof(BenchmarkDataClass), settings);
            }
        }
    }
}
