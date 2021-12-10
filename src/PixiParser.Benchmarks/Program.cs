using BenchmarkDotNet.Running;

namespace PixiEditor.Parser.Benchmarks;

internal class Program
{
    static void Main()
    {
        BenchmarkRunner.Run<Benchmarks>();
    }
}
