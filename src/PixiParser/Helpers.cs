using System;
using System.Collections.Generic;
using System.Drawing;

namespace PixiEditor.Parser
{
    internal static class Helpers
    {
        public static List<Color> BytesToSwatches(byte[] bytes)
        {
            List<Color> swatches = new();

            if (bytes is null)
            {
                return swatches;
            }

            // Convert the swatch byte array to a tuple array
            for (int sI = 0; sI < bytes.Length; sI += 4)
            {
                byte a = bytes[sI];
                byte r = bytes[sI + 1];
                byte g = bytes[sI + 2];
                byte b = bytes[sI + 3];

                swatches.Add(Color.FromArgb(a, r, g, b));
            }

            return swatches;
        }

        public static byte[] SwatchesToBytes(IEnumerable<Color> swatches)
        {
            if (swatches is null)
            {
                return Array.Empty<byte>();
            }

            List<byte> tupleData = new();

            foreach (var color in swatches)
            {
                tupleData.Add(color.A);
                tupleData.Add(color.R);
                tupleData.Add(color.G);
                tupleData.Add(color.B);
            }

            return tupleData.ToArray();
        }
    }
}