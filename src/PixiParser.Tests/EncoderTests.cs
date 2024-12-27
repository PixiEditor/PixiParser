using System;
using PixiEditor.Parser.Skia;
using SkiaSharp;
using Xunit;

namespace PixiEditor.Parser.Tests;

public class EncoderTests
{
    [Theory]
    [InlineData(32, 32)]
    [InlineData(64, 64)]
    [InlineData(128, 128)]
    [InlineData(256, 256)]
    [InlineData(512, 512)]
    [InlineData(1024, 1024)]
    public void TestThatBuiltInEncodersEncodesSampleData(int width, int height)
    {
        using SKBitmap bitmap = CreateSKBitmap(width, height);

        foreach (var encoder in BuiltInEncoders.Encoders)
        {
            byte[] encoded = encoder.Value.Encode(bitmap.Bytes, width, height, true);
            byte[] decoded = encoder.Value.Decode(encoded, out _);
        }
    }

    public static SKBitmap CreateSKBitmap(int width, int height)
    {
        Random random = new(2);
        SKBitmap bitmap = new(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bitmap.SetPixel(x, y, new SKColor((uint)random.Next()));
            }
        }

        return bitmap;
    }
}
