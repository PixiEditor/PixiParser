using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace PixiEditor.Parser.Benchmarks.Benchmarks;

public partial class Benchmarks
{
    private byte[] benchmarkDocumentBytes;

    [Benchmark]
    public Document Deserialize()
    {
        return PixiParser.V5.Deserialize(benchmarkDocumentBytes);
    }

    [Benchmark]
    public async Task<Document> DeserializeAsync()
    {
        return await PixiParser.V5.DeserializeAsync("test.pixi");
    }
}
