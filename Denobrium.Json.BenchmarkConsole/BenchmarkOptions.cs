using System;

namespace Denobrium.Json.Benchmark
{
    internal class BenchmarkOptions
    {
        internal int IterationsPerRun { get; set; } = 1500;

        internal int NumberOfRuns { get; set; } = 5;

        internal bool IncludeDataSet { get; set; } = false;

        internal bool IncludeComplexTypes { get; set; } = false;

        private static readonly Lazy<BenchmarkOptions> current = new Lazy<BenchmarkOptions>(isThreadSafe:true);

        internal static BenchmarkOptions Current => current.Value;
    }
}
