using System;
using System.Collections.Generic;
using System.Drawing;
using MessagePack;

namespace PixiEditor.Parser.Collections;

public class ColorCollection : List<Color>
{
    /// <summary>
    /// Initializes a new, empty ColorCollection
    /// </summary>
    public ColorCollection()
    { }

    /// <summary>
    /// Initializes a new, empty ColorCollection with the <paramref name="capacity"/>
    /// </summary>
    public ColorCollection(int capacity)
        : base(capacity)
    { }

    /// <summary>
    /// Initializes a new ColorCollection, containing the <paramref name="colors"/>
    /// </summary>
    /// <param name="colors">A enumerable of <see cref="Color"/>s</param>
    public ColorCollection(IEnumerable<Color> colors)
        : base(colors)
    { }

    /// <summary>
    /// Initializes a new ColorCollection, containing the <paramref name="colors"/>
    /// </summary>
    /// <param name="colors">A array of <see cref="Color"/>s</param>
    public ColorCollection(params Color[] colors)
        : base(colors)
    { }

    /// <summary>
    /// Adds a color to the collection, Alpha will be set to 255
    /// </summary>
    /// <param name="r">A value between 0 and 255</param>
    /// <param name="g">A value between 0 and 255</param>
    /// <param name="b">A value between 0 and 255</param>
    /// <returns>The created <see cref="Color"/></returns>
    /// <exception cref="ArgumentException">red, green, or blue is less than 0 or greater than 255.</exception>
    public Color Add(byte r, byte g, byte b) => 
        AddReturn(Color.FromArgb(r, g, b));

    /// <summary>
    /// Adds a color to the collection
    /// </summary>
    /// <param name="a">A value between 0 and 255</param>
    /// <param name="r">A value between 0 and 255</param>
    /// <param name="g">A value between 0 and 255</param>
    /// <param name="b">A value between 0 and 255</param>
    /// <returns>The created <see cref="Color"/></returns>
    /// <exception cref="ArgumentException">alpha, red, green, or blue is less than 0 or greater than 255.</exception>
    public Color Add(byte a, byte r, byte g, byte b) => 
        AddReturn(Color.FromArgb(a, r, g, b));

    /// <summary>
    /// Adds a color to the collection. Bytes order being #AARRGGBB
    /// </summary>
    /// <param name="argb">A value between 0 and 255</param>
    /// <returns>The created <see cref="Color"/></returns>
    /// <exception cref="ArgumentException">alpha, red, green, or blue is less than 0 or greater than 255.</exception>
    public Color Add(int argb) => 
        AddReturn(Color.FromArgb(argb));

    /// <summary>
    /// Adds a color to the collection. Alpha will be 255
    /// </summary>
    /// <param name="color">The color as a value tuple</param>
    /// <returns>The created <see cref="Color"/></returns>
    /// <exception cref="ArgumentException">red, green, or blue is less than 0 or greater than 255.</exception>
    public Color Add((byte r, byte g, byte b) color) => 
        AddReturn(Color.FromArgb(color.r, color.g, color.b));
    
    /// <summary>
    /// Adds a color to the collection.
    /// </summary>
    /// <param name="color">The color as a value tuple</param>
    /// <returns>The created <see cref="Color"/></returns>
    /// <exception cref="ArgumentException">alpha, red, green, or blue is less than 0 or greater than 255.</exception>
    public Color Add((byte a, byte r, byte g, byte b) color) => 
        AddReturn(Color.FromArgb(color.a, color.r, color.g, color.b));

    private Color AddReturn(Color color)
    {
        Add(color);
        return color;
    }

    internal byte[] ToByteArray()
    {
        if (Count == 0)
        {
            return Array.Empty<byte>();
        }

        byte[] array = new byte[Count * 4];

        for (int i = 0; i < Count; i++)
        {
            int arrayIndex = i * 4;
            Color swatch = this[i];

            array[arrayIndex] = swatch.A;
            array[arrayIndex + 1] = swatch.R;
            array[arrayIndex + 2] = swatch.G;
            array[arrayIndex + 3] = swatch.B;
        }

        return array;
    }

    internal void FromByteArray(byte[] bytes)
    {
        if (bytes.Length % 4 != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), "The length of the bytes must be a multiple of 4");
        }

        Clear();

        Capacity = bytes.Length / 4;

        for (var sI = 0; sI < bytes.Length; sI += 4)
        {
            var a = bytes[sI];
            var r = bytes[sI + 1];
            var g = bytes[sI + 2];
            var b = bytes[sI + 3];

            Add(a, r, g, b);
        }
    }
}