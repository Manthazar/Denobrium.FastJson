using Denobrium.Json.Benchmark.DataObjects;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Denobrium.Json.Benchmark.Workers
{
    internal class BinaryFormatterBenchmarks
    {
        /// <summary>
        /// Verifies the speed of deserializing into a data class.
        /// </summary>
        internal class Deserialize_Into_DataClass : Worker<Stream>
        {
            private readonly BinaryFormatter formatter = new BinaryFormatter();

            protected override string Name => "BinaryFormatter.Deserialize.IntoClass";

            protected override Stream CreateContext()
            {
                var dataClass = BenchmarkDataFactory.CreateDefaultDataClass();

                var stream = new MemoryStream();
                formatter.Serialize(stream, dataClass);

                return stream;
            }

            protected override void DoStuff(Stream stream)
            {
                stream.Seek(0L, SeekOrigin.Begin);
                var dataObject = (BenchmarkDataClass)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Verifies the speed of deserializing into a data class.
        /// </summary>
        internal class Serialize : Worker<BenchmarkDataClass>
        {
            private readonly BinaryFormatter formatter = new BinaryFormatter();

            protected override string Name => "BinaryFormatter.Serialize";

            protected override BenchmarkDataClass CreateContext() => BenchmarkDataFactory.CreateDefaultDataClass();

            protected override void DoStuff(BenchmarkDataClass context)
            {
                var stream = new MemoryStream();
                formatter.Serialize(stream, context);
            }
        }
    }
}
