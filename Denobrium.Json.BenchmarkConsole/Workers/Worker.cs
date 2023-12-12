using Denobrium.Json.Benchmark;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Denobrium.Json.Benchmark.Workers
{
    internal abstract class Worker<T>
    {
        private IList<long> durations = new List<long>();

        public void Work()
        {
            WriteName(Name);

            Initialize();

            var stopwatch = new Stopwatch();
            var numberOfRuns = BenchmarkOptions.Current.NumberOfRuns;
            var iterationsPerRun = BenchmarkOptions.Current.IterationsPerRun;
            var context = CreateContext();

            for (int pp = 0; pp < numberOfRuns; pp++)
            {
                stopwatch.Reset();
                stopwatch.Start();

                for (int i = 0; i < iterationsPerRun; i++)
                {
                    DoStuff(context);
                }

                stopwatch.Stop();

                LogDuration(stopwatch);
            }

            WriteAverage(true);
        }

        protected virtual void Initialize()
        {
            durations = new List<long>();
        }

        /// <summary>
        /// Logs the current duration into the log table.
        /// </summary>
        /// <param name="stopwatch"></param>
        protected void LogDuration(Stopwatch stopwatch)
        {
            Console.Write("\t" + stopwatch.ElapsedMilliseconds);
            durations.Add(stopwatch.ElapsedMilliseconds);
        }

        private void WriteAverage(bool excludeFirst)
        {
            if (!excludeFirst)
            {
                Console.Write("\t=>" + Math.Round(durations.Average(), 0));
            }
            else
            {
                durations.Remove(durations.First());
                Console.Write("\t=>" + Math.Round(durations.Average(), 0));
            }
        }

        protected void WriteName(string name)
        {
            Console.WriteLine();
            Console.Write($"{name}");
        }

        protected abstract string Name { get; }

        protected abstract T CreateContext();

        /// <summary>
        /// Performs the stuff under benchmark (usually serialization or deserialization).
        /// </summary>
        protected abstract void DoStuff(T context);
    }
}