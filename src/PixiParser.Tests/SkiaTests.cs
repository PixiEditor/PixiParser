using PixiEditor.Parser.Skia;
using SkiaSharp;
using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace PixiEditor.Parser.Tests;

public class SkiaTests
{
    [Fact]
    public void CanCreateSKBitmapFromLayer()
    {
        SerializableDocument document = PixiParser.Deserialize("./Files/16x16,PPD-3.pixi");

        SerializableLayer layer = document.Layers[0];
        using SKBitmap bitmap = layer.ToSKBitmap();

        Assert.Equal(layer.Width, bitmap.Width);
    }

    [Fact]
    public void CanCreateSKImageFromLayer()
    {
        SerializableDocument document = PixiParser.Deserialize("./Files/16x16,PPD-3.pixi");

        SerializableLayer layer = document.Layers[0];
        using SKImage image = layer.ToSKImage();

        Assert.Equal(layer.Width, image.Width);
    }

    [Fact]
    public void CanCombineLayers()
    {
        SerializableDocument document = new(20, 20);
        SerializableLayer layer1 = new(2, 3, 3, 3);
        SerializableLayer layer2 = new(4, 6, 2, 5);

        document.Layers.Add(layer1);
        document.Layers.Add(layer2);

        Assert.NotNull(document.LayersToSKBitmap());
    }

    [Fact]
    public void CanEncodeBitmapBytesFromSK()
    {
        const int width = 20;
        const int height = 20;

        SerializableDocument document = new(width, height);
        using SKBitmap bitmap = new(width, height);

        document.Layers.Add(new SerializableLayer(width, height));
        document.Layers.Add(new SerializableLayer(width, height));
        document.Layers[0].FromSKBitmap(bitmap);
        document.Layers[1].FromSKImage(SKImage.FromBitmap(bitmap));

        Assert.NotEmpty(document.Layers[0].PngBytes);
        Assert.NotEmpty(document.Layers[1].PngBytes);
    }

    [Fact]
    public void CanAddAndReturnSwatches()
    {
        SerializableDocument document = new();

        document.Swatches.Add(SKColors.White);

        Assert.Contains(Color.FromArgb(255, 255, 255, 255), document.Swatches);

        document.Swatches.AddRange(GetColors());

        Assert.Contains(Color.FromArgb(255, 0, 0, 255), document.Swatches);
        Assert.Contains(Color.FromArgb(255, 255, 0, 0), document.Swatches);

        static IEnumerable<SKColor> GetColors()
        {
            yield return SKColors.Blue;
            yield return SKColors.Red;
        }

        var skColors = document.Swatches.ToSKColors();

        Assert.Contains(SKColors.White, skColors);
        Assert.Contains(SKColors.Blue, skColors);
        Assert.Contains(SKColors.Red, skColors);
    }
}
