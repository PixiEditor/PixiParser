using SkiaSharp;
using System;

namespace PixiEditor.Parser.Skia;

public static class SkiaExtensions
{
    /// <summary>
    /// Creates a new <see cref="SKBitmap"/> from the png bytes of the layer
    /// </summary>
    public static SKBitmap ToSKBitmap(this SerializableLayer layer) => 
        SKBitmap.Decode(layer.PngBytes, new SKImageInfo(layer.Width, layer.Height));

    /// <summary>
    /// Creates a new <see cref="SKImage"/> from the png bytes of the layer
    /// </summary>
    public static SKImage ToSKImage(this SerializableLayer layer) => SKImage.FromEncodedData(layer.PngBytes);
    
    /// <summary>
    /// Encodes the <paramref name="bitmap"/> into the png bytes of the layer
    /// </summary>
    /// <param name="bitmap">The bitmap that should be encoded</param>
    /// <returns><paramref name="layer"/></returns>
    public static SerializableLayer FromSKBitmap(this SerializableLayer layer, SKBitmap bitmap)
    {
        using SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100);

        layer.PngBytes = data.AsSpan().ToArray();
        layer.Width = bitmap.Width;
        layer.Height = bitmap.Height;

        return layer;
    }

    /// <summary>
    /// Encodes the <paramref name="image"/> into the png bytes of the layer
    /// </summary>
    /// <param name="bitmap">The bitmap that should be encoded</param>
    /// <returns><paramref name="image"/></returns>
    public static SerializableLayer FromSKImage(this SerializableLayer layer, SKImage image)
    {
        using SKData data = image.Encode();

        layer.PngBytes = data.AsSpan().ToArray();
        layer.Width = image.Width;
        layer.Height = image.Height;

        return layer;
    }

    /// <summary>
    /// Draws all layers on top of a <see cref="SKBitmap"/>
    /// </summary>
    /// <param name="document"></param>
    /// <returns>The <see cref="SKBitmap"/> instance</returns>
    public static SKBitmap LayersToSKBitmap(this SerializableDocument document)
    {
        SKImageInfo info = new(document.Width, document.Height, SKColorType.RgbaF32, SKAlphaType.Unpremul, SKColorSpace.CreateSrgb());
        using SKSurface surface = SKSurface.Create(info);
        SKCanvas canvas = surface.Canvas;
        using SKPaint paint = new();

        foreach (SerializableLayer layer in document)
        {
            if (layer.PngBytes == null || layer.PngBytes.Length == 0)
            {
                continue;
            }

            bool visible = document.Layers.GetFinalLayerVisibilty(layer);

            if (!visible)
            {
                continue;
            }

            double opacity = document.Layers.GetFinalLayerOpacity(layer);

            if (opacity == 0)
            {
                continue;
            }

            using SKColorFilter filter = SKColorFilter.CreateBlendMode(SKColors.White.WithAlpha((byte)(opacity * 255)), SKBlendMode.DstIn);
            paint.ColorFilter = filter;

            canvas.DrawImage(layer.ToSKImage(), layer.OffsetX, layer.OffsetY, paint);
        }

        SKBitmap bitmap = new(info);

        surface.ReadPixels(info, bitmap.GetPixels(), info.RowBytes, 0, 0);

        return bitmap;
    }
}
