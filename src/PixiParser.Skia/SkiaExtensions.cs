using SkiaSharp;
using System;
using System.Linq;
using PixiEditor.Parser.Helpers;

namespace PixiEditor.Parser.Skia;

public static class SkiaExtensions
{
    /// <summary>
    /// Creates a new <see cref="SKBitmap"/> from the png bytes of the layer
    /// </summary>
    public static SKBitmap ToSKBitmap(this IImageContainer layer) => 
        SKBitmap.Decode(layer.ImageBytes);

    /// <summary>
    /// Creates a new <see cref="SKImage"/> from the png bytes of the layer
    /// </summary>
    public static SKImage ToSKImage(this IImageContainer layer) => SKImage.FromEncodedData(layer.ImageBytes);
    
    /// <summary>
    /// Encodes the <paramref name="bitmap"/> into the png bytes of the layer
    /// </summary>
    /// <param name="bitmap">The bitmap that should be encoded</param>
    /// <returns><paramref name="layer"/></returns>
    public static IImageContainer FromSKBitmap(this IImageContainer layer, SKBitmap bitmap)
    {
        using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);

        layer.ImageBytes = data.AsSpan().ToArray();

        if (layer is not ISize<int> size) return layer;
        
        size.Width = bitmap.Width;
        size.Height = bitmap.Height;

        return layer;
    }

    /// <summary>
    /// Encodes the <paramref name="image"/> into the png bytes of the layer
    /// </summary>
    /// <param name="bitmap">The bitmap that should be encoded</param>
    /// <returns><paramref name="image"/></returns>
    public static IImageContainer FromSKImage(this IImageContainer layer, SKImage image)
    {
        using var data = image.Encode();

        layer.ImageBytes = data.AsSpan().ToArray();
        
        if (layer is not ISize<int> size) return layer;
        
        size.Width = image.Width;
        size.Height = image.Height;

        return layer;
    }

    /// <summary>
    /// Draws all layers on top of a <see cref="SKBitmap"/>
    /// </summary>
    /// <param name="document"></param>
    /// <returns>The <see cref="SKBitmap"/> instance</returns>
    public static SKBitmap LayersToSKBitmap(this Document document)
    {
        SKImageInfo info = new(document.Width, document.Height, SKColorType.RgbaF32, SKAlphaType.Unpremul, SKColorSpace.CreateSrgb());
        using var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;
        using SKPaint paint = new();

        foreach (var layer in document.RootFolder.GetChildrenRecursive().OfType<IImageContainer>())
        {
            if (layer is not ISize<int> size) continue;
            
            if (layer.ImageBytes == null || layer.ImageBytes.Length == 0)
            {
                continue;
            }

            bool visible = layer.GetFinalVisibility(document);
            
            if (!visible)
            {
                continue;
            }

            float opacity = layer is IOpacity opacityLayer ? opacityLayer.GetFinalOpacity(document) : 1;

            if (opacity == 0)
            {
                continue;
            }

            var finalBlendMode = layer is IBlendMode blendMode
                ? (int)blendMode.BlendMode != -1 ? blendMode.BlendMode.ToSKBlendMode() : SKBlendMode.SrcOver
                : SKBlendMode.SrcOver;

            using var filter = SKColorFilter.CreateBlendMode(SKColors.White.WithAlpha((byte)(opacity * 255)), finalBlendMode);
            paint.ColorFilter = filter;

            canvas.DrawImage(layer.ToSKImage(), size.OffsetX, size.OffsetY, paint);
        }

        SKBitmap bitmap = new(info);

        surface.ReadPixels(info, bitmap.GetPixels(), info.RowBytes, 0, 0);

        return bitmap;
    }
}
