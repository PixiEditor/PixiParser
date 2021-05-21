using System;
using System.Collections.Generic;
using System.Drawing;

namespace PixiEditor.Parser.Benchmarks
{
    public static class Helper
    {
        public static SerializableDocument CreateDocument(int Size, int Layers)
        {
            var benchmarkDocument = new SerializableDocument()
            {
                Width = Size,
                Height = Size,
                Swatches = new List<Color> { Color.FromArgb(255, 255, 255, 255) },
                Layers = new List<SerializableLayer>()
            };

            for (int i = 0; i < Layers; i++)
            {
                var layer = benchmarkDocument.Layers[i] = new SerializableLayer();

                layer.Width = Size;
                layer.Height = Size;

                layer.BitmapBytes = new byte[Size * Size * 4];

                new Random().NextBytes(layer.BitmapBytes);
            }

            return benchmarkDocument;
        }
    }
}
