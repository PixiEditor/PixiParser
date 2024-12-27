using SkiaSharp;
using System;
using System.Linq;
using PixiEditor.Parser.Old.PixiV4;
using PixiEditor.Parser.Old.PixiV4.Helpers;

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
    /// <returns><paramref name="container"/></returns>
    public static IImageContainer FromSKBitmap(this IImageContainer container, SKBitmap bitmap, ImageEncoder encoder)
    {
        var data = encoder.Encode(bitmap.Bytes, bitmap.Width, bitmap.Height, bitmap.ColorSpace?.IsSrgb ?? true);

        container.ImageBytes = data.AsSpan().ToArray();

        if (container is not ISize<int> size) return container;
        
        size.Width = bitmap.Width;
        size.Height = bitmap.Height;

        return container;
    }

    /// <summary>
    /// Encodes the <paramref name="image"/> into the png bytes of the layer
    /// </summary>
    /// <param name="bitmap">The bitmap that should be encoded</param>
    /// <returns><paramref name="image"/></returns>
    public static IImageContainer FromSKImage(this IImageContainer imageContainer, SKImage image, ImageEncoder encoder)
    {
        var data = encoder.Encode(image.EncodedData.ToArray(), image.Width, image.Height, image.ColorSpace?.IsSrgb ?? true);

        imageContainer.ImageBytes = data.AsSpan().ToArray();
        
        if (imageContainer is not ISize<int> size) return imageContainer;
        
        size.Width = image.Width;
        size.Height = image.Height;

        return imageContainer;
    }

    /// <summary>
    /// Draws all layers on top of a <see cref="SKBitmap"/>
    /// </summary>
    /// <param name="document"></param>
    /// <returns>The <see cref="SKBitmap"/> instance</returns>
    
    [Obsolete("This is a helper method for the deprecated document model. Use the new document model instead.")]
    public static SKBitmap LayersToSKBitmap(this DocumentV4 document)
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

            bool visible = layer is IStructureMember sm && sm.GetFinalVisibility(document);
            
            if (!visible)
            {
                continue;
            }

            float opacity = layer is IStructureOpacity opacityLayer ? opacityLayer.GetFinalOpacity(document) : 1;

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
