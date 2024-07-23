namespace PixiEditor.Parser.Skia;

public static class DocumentExtensions
{
    public static ImageEncoder? GetEncoder(this Document document) =>
        BuiltInEncoders.Encoders.TryGetValue(document.ImageEncoderUsed, out var encoder) ? encoder : null;
}