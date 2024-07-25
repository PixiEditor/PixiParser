using Qoi.NetStandard;
using SkiaSharp;

namespace PixiEditor.Parser.Skia.Encoders;

public class QoiEncoder : ImageEncoder
{
    public override string EncodedFormatName { get; } = "QOI";

    public override byte[] Encode(byte[] rawBitmap, int width, int height) => 
        Qoi.NetStandard.QoiEncoder.EncodeToQoi(width, height, rawBitmap, true, true);

    public override byte[] Decode(byte[] encodedData, out SKImageInfo info)
    {
        var decoded = Qoi.NetStandard.QoiEncoder.DecodeQoi(encodedData, out var dataHeader);
        info = new SKImageInfo((int)dataHeader.Width, (int)dataHeader.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        
        return decoded;
    }
}