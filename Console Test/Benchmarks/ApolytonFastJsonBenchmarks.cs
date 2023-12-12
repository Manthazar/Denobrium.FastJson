using consoletest;
using consoletest.DataObjects;

namespace Apolyton.FastJson.Benchmarks
{
    internal class ApolytonFastJsonBenchmarks 
    {
        /// <summary>
        /// Verifies the speed of deserializing into a data class.
        /// </summary>
        internal class Deserialize_Into_DataClass : BenchmarkWorker<string>
        {
            protected override string Name => "Apolyton.FastJson.Deserialize.IntoClass";

            protected override string CreateContext()
            {
                // Type information needs to be in the json string, otherwise readobject doesn't work.
                Apolyton.FastJson.Json.Current.DefaultParameters.UseTypeExtension = true;

                var dataClass = BenchmarkDataFactory.CreateDefaultDataClass();
                var jsonText = Apolyton.FastJson.Json.Current.ToJson(dataClass);

                Apolyton.FastJson.Json.Current.DefaultParameters.UseTypeExtension = false;

                return jsonText;
            }

            protected override void DoStuff(string jsonText)
            {
                var dataObject = (BenchmarkDataClass)Apolyton.FastJson.Json.Current.ReadObject(jsonText);
            }
        }
    }
}
