using System;
using System.Collections.Generic;

namespace PixiEditor.Parser
{
    internal static class Helpers
    {
        public static Tuple<byte, byte, byte, byte>[] BytesToSwatches(byte[] bytes)
        {
            if (bytes is null)
            {
                return Array.Empty<Tuple<byte, byte, byte, byte>>();
            }

            List<Tuple<byte, byte, byte, byte>> swatches = new List<Tuple<byte, byte, byte, byte>>();

            // Convert the swatch byte array to a tuple array
            for (int sI = 0; sI < bytes.Length; sI += 4)
            {
                byte a = bytes[sI];
                byte r = bytes[sI + 1];
                byte g = bytes[sI + 2];
                byte b = bytes[sI + 3];

                swatches.Add(new Tuple<byte, byte, byte, byte>(a, r, g, b));
            }

            return swatches.ToArray();
        }

        public static byte[] SwatchesToBytes(IEnumerable<Tuple<byte, byte, byte, byte>> swatches)
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