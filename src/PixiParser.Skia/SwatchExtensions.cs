using PixiEditor.Parser.Collections;
using SkiaSharp;
using System.Collections.Generic;
using System.Drawing;

namespace PixiEditor.Parser.Skia
{
    public static class SwatchExtensions
    {
        /// <summary>
        /// Iterates the <see cref="Color"/>'s in the <paramref name="collection"/> collection and converts them into <see cref="SKColor"/>'s
        /// </summary>
        public static IEnumerable<SKColor> ToSKColors(this SwatchCollection collection)
        {
            foreach (Color color in collection)
            {
                yield return new SKColor(color.R,
                                         color.G,
                                         color.B,
                                         color.A);
            }
        }

        public static void AddRange(this SwatchCollection collection, IEnumerable<SKColor> colors)
        {
            foreach (SKColor color in colors)
            {
                collection.Add(color);
            }
        }

        /// <summary>
        /// Add's the <paramref name="color"/> to the <paramref name="collection"/> and returns the new <see cref="Color"/> instance
        /// </summary>
        /// <returns>The created <see cref="Color"/> instance</returns>
        public static Color Add(this SwatchCollection collection, SKColor color)
        {
            return collection.Add(color.Alpha, color.Red, color.Green, color.Blue);
        }
    }
}
