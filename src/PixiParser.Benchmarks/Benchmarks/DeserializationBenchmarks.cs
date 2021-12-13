using BenchmarkDotNet.Attributes;

namespace PixiEditor.Parser.Benchmarks;

public partial class Benchmarks
{
    private byte[] benchmarkDocumentBytes;

    [Benchmark]
    public SerializableDocument Deserialize()
    {
        return PixiParser.Deserialize(benchmarkDocumentBytes);
    }
}
