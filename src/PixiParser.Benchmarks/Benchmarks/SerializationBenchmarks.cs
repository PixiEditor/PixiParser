using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace PixiEditor.Parser.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50, baseline: true)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [HtmlExporter]
    [MarkdownExporterAttribute.GitHub]
    public class SerializationBenchmarks
    {
        private SerializableDocument benchmarkDocument;

        [Params(32, 64, 1920)]
        public int Size;

        [Params(1, 4)]
        public int Layers;

        [GlobalSetup]
        public void Setup()
        {
            benchmarkDocument = Helper.CreateDocument(Size, Layers);
        }

        [Benchmark]
        public void Serialize()
        {
            PixiParser.Serialize(benchmarkDocument);
        }
    }
}
