﻿using Denobrium.Json.Benchmark.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Denobrium.Json.Benchmark
{
    public static partial class Benchmarks
    {
        private class BinaryFormatterBenchmark
        {
            internal static void Deserialize()
            {
                Console.WriteLine();
                Console.Write("bin deserialize");
                BenchmarkDataClass c = CreateTestedObject();
                List<long> testRuns = new List<long>();

                for (int pp = 0; pp < numberOfRuns; pp++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    bf.Serialize(ms, c);
                    BenchmarkDataClass deserializedStore = null;
                    //Console.WriteLine(" size = " +ms.Length);

                    for (int i = 0; i < iterationsPerRun; i++)
                    {
                        ms.Seek(0L, SeekOrigin.Begin);
                        deserializedStore = (BenchmarkDataClass)bf.Deserialize(ms);
                    }

                    stopwatch.Stop();
                    Console.Write("\t" + stopwatch.ElapsedMilliseconds);
                    testRuns.Add(stopwatch.ElapsedMilliseconds);
                }

                WriteAverage(true);
            }

            internal static void Serialize()
            {
                Console.Write("\r\nbin serialize");
                BenchmarkDataClass c = CreateTestedObject();

                InitTestRun();

                for (int pp = 0; pp < numberOfRuns; pp++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    DateTime st = DateTime.Now;
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();

                    for (int i = 0; i < iterationsPerRun; i++)
                    {
                        ms = new MemoryStream();
                        bf.Serialize(ms, c);
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