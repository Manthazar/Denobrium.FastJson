using Denobrium.Json.Benchmark.Workers;
using System;

namespace Denobrium.Json.Benchmark
{
    public static partial class Benchmarks
    {
        internal static void Run()
        {
            WriteHeader();

            Console.WriteLine("\n");
            Console.WriteLine("==== SERIALIZATION   ====");
            new DenobriumJsonBenchmarks.Serialize().Work();

            new NewtonsoftBenchmarks.Serialize().Work();
            new SystemTextJsonBenchmarks.Serialize().Work();

            Console.WriteLine("\n\n");
            Console.WriteLine("==== DESERIALIZATION ====");
            new DenobriumJsonBenchmarks.Deserialize_Into_DataClass().Work();
            //new DenobriumJsonBenchmarks.Deserialize_Into_JsonValue().Work();
            //new DenobriumJsonBenchmarks.Deserialize_BuildUp().Work();
            //DenobriumJsonBenchmarks.Deserialize_JsonObject_BuildUp_NoTypeExtension();
            //DenobriumJsonBenchmarks.Deserialize_JsonObject_BuildUp_DataContractTypeExtension();
            //DenobriumJsonBenchmarks.DeserializeByType();

            new NewtonsoftBenchmarks.Deserialize_Into_DataClass().Work();
            new SystemTextJsonBenchmarks.Deserialize_Into_DataClass().Work();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private static void WriteHeader()
        {
            if (Console.BufferWidth < 100)
            {
                Console.BufferWidth = 100;
            }

            Console.WriteLine(String.Format("{0} runs with {1} samples.", BenchmarkOptions.Current.NumberOfRuns, BenchmarkOptions.Current.IterationsPerRun));

            if (BenchmarkOptions.Current.IncludeComplexTypes)
            {
                Console.WriteLine("Exotic types without embedded dataset");
                BenchmarkOptions.Current.IncludeDataSet = false;
            }
        }
    }
}
