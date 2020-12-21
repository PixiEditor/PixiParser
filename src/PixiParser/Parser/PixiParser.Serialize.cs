using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MessagePack;

namespace PixiEditor.Parser
{
    public static partial class PixiParser
    {
        /// <summary>
        /// Serializes a <see cref="SerializableDocument"/> into bytes.
        /// </summary>
        /// <param name="document">The document to serialize.</param>
        /// <returns>The serialized bytes.</returns>
        public static byte[] Serialize(SerializableDocument document)
        {
            List<byte> final = new List<byte>();

            document.SwatchesData = Helpers.SwatchesToBytes(document.Swatches);

            byte[] messagePack = MessagePackSerializer.Serialize(document);

            // Message Pack Lenght
            final.AddRange(BitConverter.GetBytes(messagePack.Length));
            // Message Pack Data
            final.AddRange(messagePack);

            foreach (SerializableLayer layer in document)
            {
                if (layer.Width * layer.Height == 0)
                {
                    final.AddRange(BitConverter.GetBytes(0));
                    continue;
                }

                byte[] pngData;

                using (Bitmap bitmap = layer.ToBitmap())
                {
                    using MemoryStream stream = new MemoryStream();

                    bitmap.Save(stream, ImageFormat.Png);
                    pngData = stream.ToArray();
                }

                // Layer PNG Data Lenght
                final.AddRange(BitConverter.GetBytes(pngData.Length));
                // Layer PNG Data
                final.AddRange(pngData);
            }

            if (document.Layers.Length == 0)
            {
                final.AddRange(BitConverter.GetBytes(0));
            }

            return final.ToArray();
        }

        /// <summary>
        /// Serializes a <see cref="SerializableDocument"/> into bytes and saves them into a stream.
        /// </summary>
        /// <param name="document">The document to serialize.</param>
        public static void Serialize(SerializableDocument document, Stream stream)
        {
            byte[] toWrite = Serialize(document);

            stream.Write(toWrite, 0, toWrite.Length);
        }

        /// <summary>
        /// Serializes a <see cref="SerializableDocument"/> into bytes and saves them into a file.
        /// </summary>
        /// <param name="document">The document to serialize.</param>
        public static void Serialize(SerializableDocument document, string path)
        {
            using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);

            Serialize(document, stream);
        }
    }
}