using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using consoletest.DataObjects;

namespace consoletest
{
    public static partial class Benchmarks
    {
        private static int count = 1500;
        private static int tcount = 5;
        private static DataSet ds = new DataSet();
        private static bool includeDataSet = false;
        private static Stopwatch stopwatch;
        private static List<long> testRunDurations;

        internal static bool exotic = false;

        static Benchmarks()
        {
            stopwatch = new Stopwatch();
        }

        internal static void Run()
        {
            if (Console.BufferWidth < 100)
            {
                Console.BufferWidth = 100;
            }

            Console.WriteLine(String.Format("{0} runs with {1} samples.", tcount, count));

            if (exotic)
            {
                Console.WriteLine("Exotic types without embedded dataset");
                includeDataSet = false;
            }

            if (includeDataSet)
            {
                ds = CreateTestedDataset();
            }

            Console.WriteLine("\n==== SERIALIZATION   ====");
            //BinaryFormatterBenchmark.Serialize();
            FastJsonBenchmarks.Serialize();
            ApolytonFastJsonBenchmarks.Serialize();

            Console.WriteLine("\n\n==== DESERIALIZATION ====");
            //BinaryFormatterBenchmark.Deserialize();
            FastJsonBenchmarks.Deserialize();
            ApolytonFastJsonBenchmarks.Deserialize_JsonValue();
            ApolytonFastJsonBenchmarks.Deserialize_JsonObject_BuildUp_NoTypeExtension();
            ApolytonFastJsonBenchmarks.Deserialize_JsonObject_BuildUp_DataContractTypeExtension();
            ApolytonFastJsonBenchmarks.Deserialize();
            ApolytonFastJsonBenchmarks.DeserializeByType();
            
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

        private static void InitTestRun()
        {
            testRunDurations = new List<long>();
        }

        private static void WriteAverage(bool excludeFirst)
        {
            if (!excludeFirst)
            {
                Console.Write("\t=>" + Math.Round(testRunDurations.Average(),0));
            }
            else
            {
                testRunDurations.Remove(testRunDurations.First());
                Console.Write("\t=>" + Math.Round(testRunDurations.Average(), 0));
            }
        }

        private static FastJsonBenchmarkClass CreateTestedObject()
        {
            var c = new FastJsonBenchmarkClass();

            c.booleanValue = true;
            c.ordinaryDecimal = 3;

            if (exotic)
            {
                c.nullableGuid = Guid.NewGuid();
                c.hash = new Hashtable();
                c.bytes = new byte[1024];
                c.stringDictionary = new Dictionary<string, BaseClass>();
                c.objectDictionary = new Dictionary<BaseClass, BaseClass>();
                c.intDictionary = new Dictionary<int, BaseClass>();
                c.nullableDouble = 100.003;

                if (includeDataSet)
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

        private static DataSet CreateTestedDataset()
        {
            DataSet ds = new DataSet();

            for (int j = 1; j < 3; j++)
            {
                DataTable dt = new DataTable();
                dt.TableName = "Table" + j;
                dt.Columns.Add("col1", typeof(int));
                dt.Columns.Add("col2", typeof(string));
                dt.Columns.Add("col3", typeof(Guid));
                dt.Columns.Add("col4", typeof(string));
                dt.Columns.Add("col5", typeof(bool));
                dt.Columns.Add("col6", typeof(string));
                dt.Columns.Add("col7", typeof(string));
                ds.Tables.Add(dt);

                Random rrr = new Random();

                for (int i = 0; i < 100; i++)
                {
                    DataRow dr = dt.NewRow();

                    dr[0] = rrr.Next(int.MaxValue);
                    dr[1] = "" + rrr.Next(int.MaxValue);
                    dr[2] = Guid.NewGuid();
                    dr[3] = "" + rrr.Next(int.MaxValue);
                    dr[4] = true;
                    dr[5] = "" + rrr.Next(int.MaxValue);
                    dr[6] = "" + rrr.Next(int.MaxValue);

                    dt.Rows.Add(dr);
                }
            }

            return ds;
        }
    }
}
