using Denobrium.Json.Benchmarks;
using consoletest.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace consoletest
{
    public static partial class Benchmarks
    {
        private static int iterationsPerRun = 1500;
        private static int numberOfRuns = 5;
        private static DataSet ds = new DataSet();
        private static Stopwatch stopwatch;
        private static List<long> testRunDurations;

        static Benchmarks()
        {
            stopwatch = new Stopwatch();
        }

        internal static void Run()
        {
            WriteHeader();

            Console.WriteLine("\n==== SERIALIZATION   ====");
            //BinaryFormatterBenchmark.Serialize();
            ApolytonFastJsonBenchmarks_Old.Serialize();

            Console.WriteLine("\n\n==== DESERIALIZATION ====");
            //BinaryFormatterBenchmark.Deserialize();
            ApolytonFastJsonBenchmarks_Old.Deserialize_JsonValue();
            ApolytonFastJsonBenchmarks_Old.Deserialize();
            new ApolytonFastJsonBenchmarks.Deserialize_Into_DataClass().Work();
            new NewtonsoftBenchmarks.Deserialize_Into_DataClass().Work();
            //ApolytonFastJsonBenchmarks.Deserialize_JsonObject_BuildUp_NoTypeExtension();
            //ApolytonFastJsonBenchmarks.Deserialize_JsonObject_BuildUp_DataContractTypeExtension();
            
            //ApolytonFastJsonBenchmarks.DeserializeByType();

            #region [ other tests]

            //			litjson_serialize();
            //			jsonnet_serialize();
            //			jsonnet4_serialize();
            //stack_serialize();

            //systemweb_deserialize();
            //bin_deserialize();
            //fastjson_deserialize();

            //			litjson_deserialize();
            //			jsonnet_deserialize();
            //			jsonnet4_deserialize();
            //			stack_deserialize();
            #endregion
        }

        private static void WriteHeader()
        {
            if (Console.BufferWidth < 100)
            {
                Console.BufferWidth = 100;
            }

            Console.WriteLine(String.Format("{0} runs with {1} samples.", numberOfRuns, iterationsPerRun));

            if (BenchmarkOptions.Current.IncludeExotic)
            {
                Console.WriteLine("Exotic types without embedded dataset");
                BenchmarkOptions.Current.IncludeDataSet = false;
            }
        }

        private static void InitTestRun()
        {
            testRunDurations = new List<long>();
        }

        private static void WriteAverage(bool excludeFirst)
        {
            if (!excludeFirst)
            {
                Console.Write("\t=>" + Math.Round(testRunDurations.Average(), 0));
            }
            else
            {
                testRunDurations.Remove(testRunDurations.First());
                Console.Write("\t=>" + Math.Round(testRunDurations.Average(), 0));
            }
        }

        private static BenchmarkDataClass CreateTestedObject()
        {
            var c = new BenchmarkDataClass();

            c.booleanValue = true;
            c.ordinaryDecimal = 3;

            if (BenchmarkOptions.Current.IncludeExotic)
            {
                c.nullableGuid = Guid.NewGuid();
                c.hash = new Hashtable();
                c.bytes = new byte[1024];
                c.stringDictionary = new Dictionary<string, BaseClass>();
                c.objectDictionary = new Dictionary<BaseClass, BaseClass>();
                c.intDictionary = new Dictionary<int, BaseClass>();
                c.nullableDouble = 100.003;

                if (BenchmarkOptions.Current.IncludeDataSet)
                {
                    c.dataset = ds;
                }

                c.nullableDecimal = 3.14M;

                c.hash.Add(new Class1("0", "hello", Guid.NewGuid()), new Class2("1", "code", "desc"));
                c.hash.Add(new Class2("0", "hello", "pppp"), new Class1("1", "code", Guid.NewGuid()));

                c.stringDictionary.Add("name1", new Class2("1", "code", "desc"));
                c.stringDictionary.Add("name2", new Class1("1", "code", Guid.NewGuid()));

                c.intDictionary.Add(1, new Class2("1", "code", "desc"));
                c.intDictionary.Add(2, new Class1("1", "code", Guid.NewGuid()));

                c.objectDictionary.Add(new Class1("0", "hello", Guid.NewGuid()), new Class2("1", "code", "desc"));
                c.objectDictionary.Add(new Class2("0", "hello", "pppp"), new Class1("1", "code", Guid.NewGuid()));

                c.arrayType = new BaseClass[2];
                c.arrayType[0] = new Class1();
                c.arrayType[1] = new Class2();
            }


            c.items.Add(new Class1("1", "1", Guid.NewGuid()));
            c.items.Add(new Class2("2", "2", "desc1"));
            c.items.Add(new Class1("3", "3", Guid.NewGuid()));
            c.items.Add(new Class2("4", "4", "desc2"));

            c.laststring = "" + DateTime.Now;

            return c;
        }
    }
}
