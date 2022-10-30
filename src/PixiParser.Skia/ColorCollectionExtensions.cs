using PixiEditor.Parser.Collections;
using SkiaSharp;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PixiEditor.Parser.Skia;

public static class ColorCollectionExtensions
{
    /// <summary>
    /// Iterates the <see cref="Color"/>'s in the <paramref name="collection"/> collection and converts them into <see cref="SKColor"/>'s
    /// </summary>
    public static IEnumerable<SKColor> ToSKColors(this ColorCollection collection) =>
        collection.Select(color => new SKColor(color.R,
            color.G,
            color.B,
            color.A));

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <paramref name="collection"/>
    /// </summary>
    /// <return>The created <see cref="Color"/> instances</return>
    public static void AddRange(this ColorCollection collection, IEnumerable<SKColor> colors)
    {
        foreach (var color in colors)
        {
            collection.Add(color);
        }
    }

    /// <summary>
    /// Add's the <paramref name="color"/> to the <paramref name="collection"/> and returns the new <see cref="Color"/> instance
    /// </summary>
    /// <returns>The created <see cref="Color"/> instance</returns>
    public static Color Add(this ColorCollection collection, SKColor color) =>
        collection.Add(color.Alpha, color.Red, color.Green, color.Blue);
}
