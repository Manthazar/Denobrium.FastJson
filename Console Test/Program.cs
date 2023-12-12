using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

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
            Console.WriteLine(".net version = " + Environment.Version);
            Console.WriteLine("Press key : (E)xotic ");

            if (Console.ReadKey().Key == ConsoleKey.E)
            {
                Benchmarks.exotic = true;
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