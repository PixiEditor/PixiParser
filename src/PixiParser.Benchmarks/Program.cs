using BenchmarkDotNet.Running;

namespace PixiEditor.Parser.Benchmarks
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<SerializationBenchmarks>();
            BenchmarkRunner.Run<DeserializationBenchmarks>();
        }
    }
}
