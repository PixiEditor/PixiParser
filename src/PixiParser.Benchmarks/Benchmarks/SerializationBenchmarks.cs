using BenchmarkDotNet.Attributes;
using SkiaSharp;

namespace PixiEditor.Parser.Benchmarks;

public partial class Benchmarks
{
    private Document benchmarkDocument;
    private SKBitmap[] bitmaps;

    [Benchmark]
    public byte[] Serialize()
    {
        return PixiParser.Serialize(benchmarkDocument);
    }

    [Benchmark]
    public byte[] SerializeAndCreate()
    {
        Document document = Helper.CreateDocument(Size, Layers, false);

        for (int i = 0; i < Layers; i++)
        {
            SKData encoded = bitmaps[i].Encode(SKEncodedImageFormat.Png, 100);
            ((IImageContainer)document.RootFolder.Children[i]).ImageBytes = encoded.AsSpan().ToArray();
        }
        
        return PixiParser.Serialize(benchmarkDocument);
    }
}
