using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using consoletest.DataObjects;

namespace consoletest
{
    public static partial class Benchmarks
    {
        private class FastJsonBenchmarks
        {
            internal static void Deserialize()
            {
                Console.WriteLine();
                Console.Write("fastJSON 2.0.13 decode               ");
                FastJsonBenchmarkClass c = CreateTestedObject();

                string jsonText = fastJSON.JSON.Instance.ToJSON(c);

                InitTestRun();

                for (int pp = 0; pp < tcount; pp++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    FastJsonBenchmarkClass deserializedStore;

                    for (int i = 0; i < count; i++)
                    {
                        deserializedStore = (FastJsonBenchmarkClass)fastJSON.JSON.Instance.ToObject(jsonText);
                    }

                    stopwatch.Stop();
                    Console.Write("\t" + stopwatch.ElapsedMilliseconds);
                    testRunDurations.Add(stopwatch.ElapsedMilliseconds);
                }

                WriteAverage(true);
            }

            internal static void Serialize()
            {
                Console.WriteLine();
                Console.Write("fastJSON 2.0.13 encode    ");
                FastJsonBenchmarkClass c = CreateTestedObject();

                InitTestRun();

                for (int pp = 0; pp < tcount; pp++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                    string jsonText = null;

                    for (int i = 0; i < count; i++)
                    {
                        jsonText = fastJSON.JSON.Instance.ToJSON(c);
                    }

                    stopwatch.Stop();
                    Console.Write("\t" + stopwatch.ElapsedMilliseconds);
                    testRunDurations.Add(stopwatch.ElapsedMilliseconds);
                }

                WriteAverage(true);
            }
        }
    }
}
