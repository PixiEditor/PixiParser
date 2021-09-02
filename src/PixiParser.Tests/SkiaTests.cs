using PixiEditor.Parser.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PixiEditor.Parser.Tests
{
    public class SkiaTests
    {
        [Fact]
        public void CanCreateSKBitmapFromLayer()
        {
            SerializableDocument document = PixiParser.Deserialize("./Files/Room.pixi");

            SerializableLayer layer = document.Layers[0];
            using SKBitmap bitmap = layer.ToSKBitmap();

            Assert.Equal(layer.Width, bitmap.Width);
        }

        [Fact]
        public void CanCreateSKImageFromLayer()
        {
            SerializableDocument document = PixiParser.Deserialize("./Files/Room.pixi");

            SerializableLayer layer = document.Layers[0];
            using SKImage image = layer.ToSKImage();

            Assert.Equal(layer.Width, image.Width);
        }

        [Fact]
        public void CanEncodeBitmapBytesFromSKBitmap()
        {
            const int width = 20;
            const int height = 20;

            SerializableDocument document = new(width, height);
            using SKBitmap bitmap = new(width, height);

            document.Layers.Add(new SerializableLayer(width, height));
            document.Layers[0].FromSKBitmap(bitmap);

            Assert.NotEmpty(document.Layers[0].PngBytes);
        }
    }
}
