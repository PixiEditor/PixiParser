using PixiEditor.Parser;
using SkiaSharp;

namespace PixiEditor.Parser.Skia
{
    public static class SkiaExtensions
    {
        /// <summary>
        /// Creates a new <see cref="SKBitmap"/> from the png bytes of the layer
        /// </summary>
        public static SKBitmap ToSKBitmap(this SerializableLayer layer)
        {
            return SKBitmap.Decode(layer.PngBytes, new SKImageInfo(layer.Width, layer.Height));
        }

        /// <summary>
        /// Creates a new <see cref="SKImage"/> from the png bytes of the layer
        /// </summary>
        public static SKImage ToSKImage(this SerializableLayer layer)
        {
            return SKImage.FromEncodedData(layer.PngBytes);
        }

        /// <summary>
        /// Encodes the <paramref name="bitmap"/> into the png bytes of the layer.
        /// </summary>
        /// <param name="bitmap">The bitmap that should be encoded</param>
        public static void FromSKBitmap(this SerializableLayer layer, SKBitmap bitmap)
        {
            using SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100);

            layer.PngBytes = data.AsSpan().ToArray();
            layer.Width = bitmap.Width;
            layer.Height = bitmap.Height;
        }
    }
}
