using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace PixiEditor.Parser.Benchmarks;

[SimpleJob(RuntimeMoniker.Net60, baseline: true)]
[HtmlExporter]
[MarkdownExporterAttribute.GitHub]
public partial class Benchmarks
{
    [Params(64, 1920)]
    public int Size;

    [Params(1, 4)]
    public int Layers;

    [GlobalSetup]
    public void Setup()
    {
        benchmarkDocumentBytes = PixiParser.Serialize(Helper.CreateDocument(Size, Layers));
        benchmarkDocument = Helper.CreateDocument(Size, Layers);

        bitmaps = new SkiaSharp.SKBitmap[Layers];

        for (int i = 0; i < Layers; i++)
        {
            bitmaps[i] = Helper.CreateSKBitmap(Size);
        }
    }
}
