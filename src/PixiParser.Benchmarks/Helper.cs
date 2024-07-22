using PixiEditor.Parser.Skia;
using SkiaSharp;
using System;
using PixiEditor.Parser.Deprecated;

namespace PixiEditor.Parser.Benchmarks;

public static class Helper
{
    public static Document CreateDocument(int size, int layers, bool encodePng = true)
    {
        var benchmarkDocument = new Document()
        {
            Width = size,
            Height = size
        };

        benchmarkDocument.Swatches.Add(255, 255, 255, 255);
        benchmarkDocument.RootFolder = new Folder();

        for (int i = 0; i < layers; i++)
        {
            var layer = new ImageLayer();

            if (encodePng)
            {
                layer.FromSKBitmap(CreateSKBitmap(size));
            }

            benchmarkDocument.RootFolder.Children.Add(layer);
        }

        return benchmarkDocument;
    }

    public static SKBitmap CreateSKBitmap(int size)
    {
        Random random = new(2);
        SKBitmap bitmap = new(size, size);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bitmap.SetPixel(x, y, new SKColor((uint)random.Next()));
            }
        }

        return bitmap;
    }
}
