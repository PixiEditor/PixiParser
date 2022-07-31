using BenchmarkDotNet.Attributes;
using System.Threading.Tasks;

namespace PixiEditor.Parser.Benchmarks;

public partial class Benchmarks
{
    private byte[] benchmarkDocumentBytes;

    [Benchmark]
    public Document Deserialize()
    {
        return PixiParser.Deserialize(benchmarkDocumentBytes);
    }

    [Benchmark]
    public async Task<Document> DeserializeAsync()
    {
        return await PixiParser.DeserializeAsync("test.pixi");
    }
}
