using Denobrium.Json.Benchmark.DataObjects;
using Denobrium.Json.Data;

namespace Denobrium.Json.Benchmark.Workers
{
    internal class DenobriumJsonBenchmarks
    {
        internal class Serialize : DefaultSerializationWorker
        {
            protected override string Name => "Denobrium.Json.Serialize";

            protected override void DoStuff(BenchmarkDataClass data)
            {
                var result = Denobrium.Json.Json.Current.ToJson(data);
            }
        }

        internal class Deserialize_Into_DataClass : DefaultDeserializationWorker
        {
            protected override string Name => "Denobrium.Json.Deserialize.IntoClass";

            protected override void DoStuff(string jsonText)
            {
                var result = Denobrium.Json.Json.Current.ReadObject<BenchmarkDataClass>(jsonText);
            }
        }

        internal class Deserialize_Into_JsonValue : DefaultDeserializationWorker
        {
            protected override string Name => "Denobrium.Json.Deserialize.JsonValue";

            protected override void DoStuff(string jsonText)
            {
                var result = (JsonObject)Denobrium.Json.Json.Current.ReadJsonValue(jsonText);
            }
        }

        internal class Deserialize_BuildUp: DefaultDeserializationWorker
        {
            private readonly BenchmarkDataClass target = new BenchmarkDataClass();

            protected override string Name => "Denobrium.Json.Deserialize.BuildUp";

            protected override void DoStuff(string jsonText)
            {
                var keyValues = (JsonObject)Denobrium.Json.Json.Current.ReadJsonValue(jsonText);

                Denobrium.Json.Json.Current.BuildUp(target, keyValues);
            }
        }
    }
}
