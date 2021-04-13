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
            BinaryWriter writeStream = new BinaryWriter(stream);

            document.SwatchesData = Helpers.SwatchesToBytes(document.Swatches);

            byte[] messagePack = MessagePackSerializer.Serialize(document);

            writeStream.Write(messagePack.Length);
            writeStream.Write(messagePack);

            foreach (SerializableLayer layer in document)
            {
                if (layer.Width * layer.Height == 0)
                {
                    writeStream.Write(0);
                    continue;
                }

                using Bitmap bitmap = layer.ToBitmap();
                using MemoryStream bitmapStream = new MemoryStream();

                bitmap.Save(bitmapStream, ImageFormat.Png);

                // Layer PNG Data Lenght
                writeStream.Write(bitmapStream.Length);
                bitmapStream.CopyTo(bitmapStream);
            }

            if (document.Layers.Length == 0)
            {
                writeStream.Write(0);
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