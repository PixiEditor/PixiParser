using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace PixiEditor.Parser.Benchmarks;

[SimpleJob(RuntimeMoniker.Net60, baseline: true)]
[HtmlExporter]
[MarkdownExporterAttribute.GitHub]
public partial class Benchmarks
{
    [Params(1920)]
    public int Size;

    [Params(4)]
    public int Layers;

    [GlobalSetup]
    public void Setup()
    {
        benchmarkDocument = Helper.CreateDocument(Size, Layers);
        benchmarkDocumentBytes = PixiParser.Serialize(benchmarkDocument);

        bitmaps = new SkiaSharp.SKBitmap[Layers];

        for (int i = 0; i < Layers; i++)
        {
            bitmaps[i] = Helper.CreateSKBitmap(Size);
        }

        PixiParser.Serialize(benchmarkDocument, "./test.pixi");
    }
}
