using Qoi.NetStandard;

namespace PixiEditor.Parser.Skia.Encoders;

public class QoiEncoder : ImageEncoder
{
    public override string EncodedFormatName { get; } = "QOI";

    public override byte[] Encode(byte[] rawBitmap, int width, int height) => 
        Qoi.NetStandard.QoiEncoder.EncodeToQoi(width, height, rawBitmap, true, true);

    public override byte[] Decode(byte[] encodedData) =>
        Qoi.NetStandard.QoiEncoder.DecodeQoi(encodedData, out _);
}