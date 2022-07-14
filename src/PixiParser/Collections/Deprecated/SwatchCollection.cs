using System;
using System.Collections.Generic;
using System.Drawing;

namespace PixiEditor.Parser.Collections.Deprecated;

public class SwatchCollection : List<Color>
{
    public SwatchCollection()
        : base()
    { }

    public SwatchCollection(int capacity)
        : base(capacity)
    { }

    public SwatchCollection(IEnumerable<Color> colors)
        : base(colors)
    { }

    public SwatchCollection(params Color[] colors)
        : base(colors)
    { }

    public Color Add(byte r, byte g, byte b) => AddReturn(Color.FromArgb(r, g, b));

    public Color Add(byte a, byte r, byte g, byte b) => AddReturn(Color.FromArgb(a, r, g, b));

    public Color Add(int argb) => AddReturn(Color.FromArgb(argb));

    private Color AddReturn(Color color)
    {
        Add(color);
        return color;
    }

    internal void FromByteArray(byte[] bytes)
    {
        if (bytes.Length % 4 != 0)
        {
            throw new ArgumentOutOfRangeException("The length of the bytes must be a multiple of 4", nameof(bytes));
        }

        Clear();

        Capacity = bytes.Length / 4;

        for (int sI = 0; sI < bytes.Length; sI += 4)
        {
            byte a = bytes[sI];
            byte r = bytes[sI + 1];
            byte g = bytes[sI + 2];
            byte b = bytes[sI + 3];

            Add(a, r, g, b);
        }
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
}
