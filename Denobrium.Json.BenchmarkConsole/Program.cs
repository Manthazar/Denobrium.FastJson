using System;

namespace Denobrium.Json.Benchmark
{
    class Program
    {
        /// <summary>
        /// Represents the entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.WriteLine("OPTIONS");
            Console.WriteLine("Include complex types?: (y)");
            Console.Clear();

            Console.WriteLine("Denobrium.Json indicative benchmark tool.");

            Console.WriteLine("ENVIRONMENT");
            Console.WriteLine(".net version = " + Environment.Version);

            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                BenchmarkOptions.Current.IncludeComplexTypes = true;
            }

            Benchmarks.Run();

            Console.ReadKey();
            Console.ReadKey();
        }
    }
}