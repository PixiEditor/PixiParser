using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using MessagePack;

namespace PixiEditor.Parser
{
    public static partial class PixiParser
    {
        /// <summary>
        /// Serializes a <see cref="SerializableDocument"/> into bytes and saves them into a stream.
        /// </summary>
        /// <param name="document">The document to serialize.</param>
        public static void Serialize(SerializableDocument document, Stream stream)
        {
            document.SwatchesData = Helpers.SwatchesToBytes(document.Swatches);

            byte[] messagePack = MessagePackSerializer.Serialize(document);

            stream.Write(BitConverter.GetBytes(messagePack.Length));
            stream.Write(messagePack);

            foreach (SerializableLayer layer in document)
            {
                if (layer.Width * layer.Height == 0)
                {
                    stream.Write(BitConverter.GetBytes(0));
                    continue;
                }

                using Bitmap bitmap = layer.ToBitmap();
                using MemoryStream bitmapStream = new MemoryStream();

                bitmap.Save(bitmapStream, ImageFormat.Png);

                bitmapStream.Seek(0, SeekOrigin.Begin);

                // Layer PNG Data Lenght
                stream.Write(BitConverter.GetBytes((int)bitmapStream.Length));
                bitmapStream.CopyTo(stream);
            }

            if (document.Layers.Length == 0)
            {
                stream.Write(BitConverter.GetBytes(0));
            }
        }

        /// <summary>
        /// Serializes a <see cref="SerializableDocument"/> into bytes.
        /// </summary>
        /// <param name="document">The document to serialize.</param>
        /// <returns>The serialized bytes.</returns>
        public static byte[] Serialize(SerializableDocument document)
        {
            MemoryStream stream = new MemoryStream();

            Serialize(document, stream);

            stream.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);

            return buffer;
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