using PixiEditor.Parser.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PixiEditor.Parser.Benchmarks
{
    public static class Helper
    {
        public static SerializableDocument CreateDocument(int size, int layers, bool encodePng = true)
        {
            var benchmarkDocument = new SerializableDocument()
            {
                Width = size,
                Height = size,
                Swatches = new List<Color> { Color.FromArgb(255, 255, 255, 255) },
                Layers = new List<SerializableLayer>()
            };

            for (int i = 0; i < layers; i++)
            {
                var layer = new SerializableLayer(size, size);

                if (encodePng)
                {
                    layer.FromSKBitmap(CreateSKBitmap(size));
                }

                benchmarkDocument.Layers.Add(layer);
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
}
