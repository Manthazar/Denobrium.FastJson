using Denobrium.Json.Benchmark.Workers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

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
            new BinaryFormatterBenchmarks.Serialize().Work();

            Console.WriteLine("\n\n");
            Console.WriteLine("==== DESERIALIZATION ====");
            new DenobriumJsonBenchmarks.Deserialize_Into_DataClass().Work();
            new DenobriumJsonBenchmarks.Deserialize_Into_JsonValue().Work();
            new DenobriumJsonBenchmarks.Deserialize_BuildUp().Work();

            new NewtonsoftBenchmarks.Deserialize_Into_DataClass().Work();
            new BinaryFormatterBenchmarks.Deserialize_Into_DataClass().Work();

            //ApolytonFastJsonBenchmarks.Deserialize_JsonObject_BuildUp_NoTypeExtension();
            //ApolytonFastJsonBenchmarks.Deserialize_JsonObject_BuildUp_DataContractTypeExtension();
            //ApolytonFastJsonBenchmarks.DeserializeByType();
        }

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
