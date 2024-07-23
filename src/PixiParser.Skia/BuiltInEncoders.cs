using System.Collections.Generic;

namespace PixiEditor.Parser.Skia;

public static class BuiltInEncoders
{
    public static IReadOnlyDictionary<string, ImageEncoder> Encoders { get; } = new Dictionary<string, ImageEncoder>
    {
        { "QOI", new Encoders.QoiEncoder() },
        { "PNG", new Encoders.PngEncoder() }
    };
    
    public static ImageEncoder GetEncoder(BuiltInEncodersType type) =>
        type switch
        {
            BuiltInEncodersType.Qoi => Encoders["QOI"],
            BuiltInEncodersType.Png => Encoders["PNG"],
            _ => throw new System.ArgumentOutOfRangeException(nameof(type), type, null)
        };
}

public enum BuiltInEncodersType
{
    Qoi,
    Png
}