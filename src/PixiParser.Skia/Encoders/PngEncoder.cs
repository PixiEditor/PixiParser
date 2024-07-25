using System;
using System.IO;
using SkiaSharp;

namespace PixiEditor.Parser.Skia.Encoders;

public class PngEncoder : ImageEncoder
{
    public override string EncodedFormatName { get; } = "PNG";

    /// <summary>
    /// Encodes raw pixel data into PNG format using SkiaSharp.
    /// </summary>
    /// <param name="rawBitmap">The raw pixel data to encode.</param>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <returns>The encoded PNG data as a byte array.</returns>
    public override byte[] Encode(byte[] rawBitmap, int width, int height)
    {
        // Validate input dimensions
        if (rawBitmap == null)
            throw new ArgumentNullException(nameof(rawBitmap));

        if (rawBitmap.Length != width * height * 4)
            throw new ArgumentException("Invalid raw bitmap size for the given dimensions.");

        using var bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
        var ptr = bitmap.GetPixels();
        System.Runtime.InteropServices.Marshal.Copy(rawBitmap, 0, ptr, rawBitmap.Length);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    /// <summary>
    /// Decodes PNG data into raw pixels using SkiaSharp.
    /// </summary>
    /// <param name="encodedData">The PNG data to decode.</param>
    /// <returns>The raw pixel data as a byte array.</returns>
    public override byte[] Decode(byte[] encodedData, out SKImageInfo info)
    {
        // Validate input
        if (encodedData == null)
            throw new ArgumentNullException(nameof(encodedData));

        using var data = SKData.CreateCopy(encodedData);
        using var codec = SKCodec.Create(data);
        // Get image info
        info = codec.Info;
        var bitmap = new SKBitmap(info.Width, info.Height, info.ColorType, info.AlphaType);

        // Decode the PNG data into the bitmap
        codec.GetPixels(bitmap.Info, bitmap.GetPixels());

        // Extract raw pixel data
        var pixelData = new byte[info.Width * info.Height * 4];
        System.Runtime.InteropServices.Marshal.Copy(bitmap.GetPixels(), pixelData, 0, pixelData.Length);

        return pixelData;
    }
}