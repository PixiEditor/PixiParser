using System;
using SkiaSharp;

namespace PixiEditor.Parser.Skia;

public abstract class ImageEncoder
{
    public abstract string EncodedFormatName { get; }
    /// <summary>
    ///     Encodes raw pixels into a compressed format.
    /// </summary>
    /// <param name="rawBitmap">The raw pixel data to encode.</param>
    /// <returns>The encoded data.</returns>
    public abstract byte[] Encode(byte[] rawBitmap, int width, int height, bool isSrgb);
    
    /// <summary>
    ///    Decodes compressed data into raw pixels.
    /// </summary>
    /// <param name="encodedData">The compressed data to decode.</param>
    /// <returns>The raw pixel data.</returns>
    public abstract byte[] Decode(byte[] encodedData, out SKImageInfo info);

    public abstract byte[] Decode(Span<byte> encodedData, out SKImageInfo info);
}
