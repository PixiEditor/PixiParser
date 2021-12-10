using BenchmarkDotNet.Attributes;
using SkiaSharp;

namespace PixiEditor.Parser.Benchmarks;

public partial class Benchmarks
{
    private SerializableDocument benchmarkDocument;
    private SKBitmap[] bitmaps;

    [Benchmark]
    public byte[] Serialize()
    {
        return PixiParser.Serialize(benchmarkDocument);
    }

    [Benchmark]
    public byte[] SerializeAndCreate()
    {
        SerializableDocument document = Helper.CreateDocument(Size, Layers, false);

        for (int i = 0; i < Layers; i++)
        {
            SKData encoded = bitmaps[i].Encode(SKEncodedImageFormat.Png, 100);
            document.Layers[i].PngBytes = encoded.AsSpan().ToArray();
        }

        return PixiParser.Serialize(benchmarkDocument);
    }
}
