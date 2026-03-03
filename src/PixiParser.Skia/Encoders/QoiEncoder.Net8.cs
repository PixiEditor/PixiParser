#if NET8_0
using System;
using QoiSharpNoGC;
using QoiSharpNoGC.Codec;
using SkiaSharp;

namespace PixiEditor.Parser.Skia.Encoders;

public class QoiEncoder : ImageEncoder
{
    public override string EncodedFormatName { get; } = "QOI";

    public override byte[] Encode(byte[] rawBitmap, int width, int height, bool isSrgb) =>
        QoiSharpNoGC.QoiEncoder.Encode(
            new QoiImage(rawBitmap, width, height, Channels.RgbWithAlpha, isSrgb ? ColorSpace.SRgb : ColorSpace.Linear));

    public override byte[] Decode(byte[] encodedData, out SKImageInfo info)
    {
        var qoiImage = QoiDecoder.Decode(encodedData);

        var colorSpace = qoiImage.ColorSpace switch
        {
            ColorSpace.SRgb => SKColorSpace.CreateSrgb(),
            ColorSpace.Linear => SKColorSpace.CreateSrgbLinear(),
            _ => throw new IndexOutOfRangeException($"QOI color space '{qoiImage.ColorSpace}' is not supported.")
        };

        info = new SKImageInfo(qoiImage.Width, qoiImage.Height, SKColorType.Rgba8888, SKAlphaType.Premul, colorSpace);

        return qoiImage.Data;
    }

    public override byte[] Decode(Span<byte> encodedData, out SKImageInfo info) => Decode(encodedData.ToArray(), out info);
}
#endif
