using System;
using SkiaSharp;

namespace PixiEditor.Parser.Skia;

public static class BlendModeExtensions
{
    public static SKBlendMode ToSKBlendMode(this BlendMode mode) =>
        mode switch
        {
            BlendMode.Normal => SKBlendMode.SrcOver,
            BlendMode.Darken => SKBlendMode.Darken,
            BlendMode.Multiply => SKBlendMode.Multiply,
            BlendMode.ColorBurn => SKBlendMode.ColorBurn,
            BlendMode.Lighten => SKBlendMode.Lighten,
            BlendMode.Screen => SKBlendMode.Screen,
            BlendMode.ColorDodge => SKBlendMode.ColorDodge,
            BlendMode.LinearDodge => SKBlendMode.Plus,
            BlendMode.Overlay => SKBlendMode.Overlay,
            BlendMode.SoftLight => SKBlendMode.SoftLight,
            BlendMode.HardLight => SKBlendMode.HardLight,
            BlendMode.Difference => SKBlendMode.Difference,
            BlendMode.Exclusion => SKBlendMode.Exclusion,
            BlendMode.Hue => SKBlendMode.Hue,
            BlendMode.Saturation => SKBlendMode.Saturation,
            BlendMode.Luminosity => SKBlendMode.Luminosity,
            BlendMode.Color => SKBlendMode.Color,
            BlendMode.Unknown => (SKBlendMode)(-1),
            _ => throw  new NotImplementedException()
        };
}