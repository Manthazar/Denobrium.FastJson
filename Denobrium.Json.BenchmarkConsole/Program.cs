using System;

namespace consoletest
{
    class Program
    {
        /// <summary>
        /// Represents the entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Apolyton.FastJson indicative benchmark tool.");

            Console.WriteLine("ENVIRONMENT");
            Console.WriteLine(".net version = " + Environment.Version);

            Console.WriteLine("OPTIONS");
            Console.WriteLine("Include exotic types?: (E)");

            if (Console.ReadKey().Key == ConsoleKey.E)
            {
                BenchmarkOptions.Current.IncludeExotic = true;
            }

            Benchmarks.Run();

            Console.ReadKey();
            Console.ReadKey();
        }

        #region What is that?

        private static string pser(object data)
        {
            System.Drawing.Point p = (System.Drawing.Point)data;
            return p.X.ToString() + "," + p.Y.ToString();
        }

        private static object pdes(string data)
        {
            string[] ss = data.Split(',');

            return new System.Drawing.Point(
                int.Parse(ss[0]),
                int.Parse(ss[1])
                );
        }

        private static string tsser(object data)
        {
            return ((TimeSpan)data).Ticks.ToString();
        }

        private static object tsdes(string data)
        {
            return new TimeSpan(long.Parse(data));
        }

        #endregion
    }
}