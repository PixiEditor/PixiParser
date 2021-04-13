using System;

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
                Swatches = new Tuple<byte, byte, byte, byte>[] { new Tuple<byte, byte, byte, byte>(0, 0, 0, 0) },
                Layers = new SerializableLayer[Layers]
            };

            for (int i = 0; i < Layers; i++)
            {
                var layer = benchmarkDocument.Layers[i] = new SerializableLayer();
                layer.BitmapBytes = new byte[Size * Size * 4];

                new Random().NextBytes(layer.BitmapBytes);
            }

            return benchmarkDocument;
        }
    }
}
