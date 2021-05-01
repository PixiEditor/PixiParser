using System;
using System.Collections.Generic;

namespace PixiEditor.Parser
{
    internal static class Helpers
    {
        public static List<(byte, byte, byte, byte)> BytesToSwatches(byte[] bytes)
        {
            List<(byte, byte, byte, byte)> swatches = new List<(byte, byte, byte, byte)>();

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

                swatches.Add((a, r, g, b));
            }

            return swatches;
        }

        public static byte[] SwatchesToBytes(IEnumerable<(byte, byte, byte, byte)> swatches)
        {
            if (swatches is null)
            {
                return Array.Empty<byte>();
            }

            List<byte> tupleData = new List<byte>();

            foreach (var tuple in swatches)
            {
                tupleData.Add(tuple.Item1);
                tupleData.Add(tuple.Item2);
                tupleData.Add(tuple.Item3);
                tupleData.Add(tuple.Item4);
            }

            return tupleData.ToArray();
        }
    }
}