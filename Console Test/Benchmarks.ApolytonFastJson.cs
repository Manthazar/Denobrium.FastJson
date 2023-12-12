using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apolyton.FastJson.Data;
using consoletest.DataObjects;

namespace consoletest
{
    public static partial class Benchmarks
    {
        private class ApolytonFastJsonBenchmarks
        {
            /// <summary>
            /// Deserialize from a string with type information.
            /// </summary>
            internal static void Deserialize()
            {
                Console.WriteLine();
                Console.Write("(A4) JSON readobject                  ");
                FastJsonBenchmarkClass c = CreateTestedObject();

                // Type information needs to be in the string, otherwise readobject doesn't work.
                Apolyton.FastJson.Json.Current.DefaultParameters.UseTypeExtension = true;
                string jsonText = Apolyton.FastJson.Json.Current.ToJson(c); 
                Apolyton.FastJson.Json.Current.DefaultParameters.UseTypeExtension = false;

                InitTestRun();

                for (int pp = 0; pp < tcount; pp++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    FastJsonBenchmarkClass deserializedStore;

                    for (int i = 0; i < count; i++)
                    {
                        deserializedStore = (FastJsonBenchmarkClass)Apolyton.FastJson.Json.Current.ReadObject(jsonText);
                    }

                    stopwatch.Stop();
                    Console.Write("\t" + stopwatch.ElapsedMilliseconds);
                    testRunDurations.Add(stopwatch.ElapsedMilliseconds);
                }

                WriteAverage(true);
            }

            /// <summary>
            /// Deserialize from a string with type information.
            /// </summary>
            internal static void DeserializeByType()
            {
                Console.WriteLine();
                Console.Write("(A5) JSON readobject with given type ");
                FastJsonBenchmarkClass c = CreateTestedObject();

                // No type information needs to be in the string, since we will provide the root level element later.s
                string jsonText = Apolyton.FastJson.Json.Current.ToJson(c); 
                InitTestRun();

                for (int pp = 0; pp < tcount; pp++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    FastJsonBenchmarkClass deserializedStore;

                    for (int i = 0; i < count; i++)
                    {
                        deserializedStore = (FastJsonBenchmarkClass)Apolyton.FastJson.Json.Current.ReadObject(jsonText, typeof(FastJsonBenchmarkClass));
                    }

                    stopwatch.Stop();
                    Console.Write("\t" + stopwatch.ElapsedMilliseconds);
                    testRunDurations.Add(stopwatch.ElapsedMilliseconds);
                }

                WriteAverage(true);
            }

            /// <summary>
            /// Deserialize into a json object.
            /// </summary>
            internal static void Apolyton_FastJson_Deserialize_JsonObject()
            {
                Console.WriteLine();
                Console.Write("(A3) JSON decode JsonValue         ");
                FastJsonBenchmarkClass c = CreateTestedObject();

                Apolyton.FastJson.Json.Current.DefaultParameters.UseTypeExtension = false;
                string jsonText = Apolyton.FastJson.Json.Current.ToJson(c);

                InitTestRun();

                for (int pp = 0; pp < tcount; pp++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    JsonObject deserializedStore;

                    for (int i = 0; i < count; i++)
                    {
                        deserializedStore = (JsonObject)Apolyton.FastJson.Json.Current.ReadJsonValue(jsonText);
                    }

                    stopwatch.Stop();
                    Console.Write("\t" + stopwatch.ElapsedMilliseconds);
                    testRunDurations.Add(stopwatch.ElapsedMilliseconds);
                }

                WriteAverage(true);
            }

            /// <summary>
            /// Deserialize into a json object and build up a given instance with its values.
            /// </summary>
            internal static void Deserialize_JsonObject_BuildUp_NoTypeExtension()
            {
                Console.WriteLine();
                Console.Write("(A2) JSON decode JVal+Bld           ");
                FastJsonBenchmarkClass c = CreateTestedObject();
                FastJsonBenchmarkClass target;

                Apolyton.FastJson.Json.Current.DefaultParameters.UseTypeExtension = false;
                string jsonText = Apolyton.FastJson.Json.Current.ToJson(c);

                InitTestRun();

                for (int pp = 0; pp < tcount; pp++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    JsonObject deserializedStore;
                    target = new FastJsonBenchmarkClass();

                    for (int i = 0; i < count; i++)
                    {
                        deserializedStore = (JsonObject)Apolyton.FastJson.Json.Current.ReadJsonValue(jsonText);
                        Apolyton.FastJson.Json.Current.BuildUp(target, deserializedStore);
                    }

                    stopwatch.Stop();
                    Console.Write("\t" + stopwatch.ElapsedMilliseconds);
                    testRunDurations.Add(stopwatch.ElapsedMilliseconds);
                }

                WriteAverage(true);
            }


            /// <summary>
            /// Deserialize into a json value.
            /// </summary>
            internal static void Deserialize_JsonValue()
            {
                Console.WriteLine();
                Console.Write("(A1) JSON decode JVal                 ");
                FastJsonBenchmarkClass c = CreateTestedObject();
                FastJsonBenchmarkClass target;

                Apolyton.FastJson.Json.Current.DefaultParameters.UseTypeExtension = false;
                string jsonText = Apolyton.FastJson.Json.Current.ToJson(c);

                InitTestRun();

                for (int pp = 0; pp < tcount; pp++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    JsonObject deserializedStore;
                    target = new FastJsonBenchmarkClass();

                    for (int i = 0; i < count; i++)
                    {
                        deserializedStore = (JsonObject)Apolyton.FastJson.Json.Current.ReadJsonValue(jsonText);
                    }

                    stopwatch.Stop();
                    Console.Write("\t" + stopwatch.ElapsedMilliseconds);
                    testRunDurations.Add(stopwatch.ElapsedMilliseconds);
                }

                WriteAverage(true);
            }

            /// <summary>
            /// Deserialize into a json object, build it up into a given instance with type extension support 
            /// and data contract type descriptor.
            /// </summary>
            internal static void Deserialize_JsonObject_BuildUp_DataContractTypeExtension()
            {
                Console.WriteLine();
                Console.Write("(A3) JSON decode JVal+Bld+TExt+DC");
                FastJsonBenchmarkClass c = CreateTestedObject();
                FastJsonBenchmarkClass target;

                Apolyton.FastJson.Json.Current.DefaultParameters.UseTypeExtension = true;
                Apolyton.FastJson.Json.Current.DefaultParameters.RegisterTypeDescriptor(
                    new Apolyton.FastJson.Registry.DataContractTypeDescriptor(typeof(Benchmarks).Assembly));
                string jsonText = Apolyton.FastJson.Json.Current.ToJson(c);

                InitTestRun();

                for (int pp = 0; pp < tcount; pp++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    JsonObject deserializedStore;
                    target = new FastJsonBenchmarkClass();

                    for (int i = 0; i < count; i++)
                    {
                        deserializedStore = (JsonObject)Apolyton.FastJson.Json.Current.ReadJsonValue(jsonText);
                        Apolyton.FastJson.Json.Current.BuildUp(target, deserializedStore);
                    }

                    stopwatch.Stop();
                    Console.Write("\t" + stopwatch.ElapsedMilliseconds);
                    testRunDurations.Add(stopwatch.ElapsedMilliseconds);
                }

                WriteAverage(true);
            }

            /// <summary>
            /// Serialize into a json string.
            /// </summary>
            internal static void Serialize()
            {
                Console.WriteLine();
                Console.Write("(A) FastJSON encode      ");
                FastJsonBenchmarkClass c = CreateTestedObject();

                InitTestRun();

                for (int pp = 0; pp < tcount; pp++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                    string jsonText = null;

                    for (int i = 0; i < count; i++)
                    {
                        jsonText = Apolyton.FastJson.Json.Current.ToJson(c);
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
