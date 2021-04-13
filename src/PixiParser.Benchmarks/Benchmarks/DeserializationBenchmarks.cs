using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace PixiEditor.Parser.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50, baseline: true)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [HtmlExporter]
    [MarkdownExporterAttribute.GitHub]
    public class DeserializationBenchmarks
    {
        private byte[] benchmarkDocument;

        [Params(32, 64, 1920)]
        public int Size;

        [Params(1, 4)]
        public int Layers;

        [GlobalSetup]
        public void Setup()
        {
            benchmarkDocument = PixiParser.Serialize(Helper.CreateDocument(Size, Layers));
        }

        [Benchmark]
        public void Deserialize()
        {
            PixiParser.Deserialize(benchmarkDocument);
        }
    }
}
