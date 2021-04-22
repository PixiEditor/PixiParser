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
            BinaryWriter writer = new BinaryWriter(stream);

            document.SwatchesData = Helpers.SwatchesToBytes(document.Swatches);

            byte[] messagePack = MessagePackSerializer.Serialize(document);

            writer.Write(messagePack.Length);
            writer.Write(messagePack);

            WriteLayers(document, writer);
        }

        /// <summary>
        /// Serializes a <see cref="SerializableDocument"/> into bytes.
        /// </summary>
        /// <param name="document">The document to serialize.</param>
        /// <returns>The serialized bytes.</returns>
        public static Span<byte> Serialize(SerializableDocument document)
        {
            MemoryStream stream = new MemoryStream();

            Serialize(document, stream);

            stream.Seek(0, SeekOrigin.Begin);

            Span<byte> span = new Span<byte>();

            stream.Read(span);

            return span;
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

        private static void WriteLayers(SerializableDocument document, BinaryWriter writer)
        {
            foreach (SerializableLayer layer in document)
            {
                if (layer.Width * layer.Height == 0)
                {
                    writer.Write(0);
                    continue;
                }

                using Bitmap bitmap = layer.ToBitmap();
                using MemoryStream bitmapStream = new MemoryStream();

                bitmap.Save(bitmapStream, ImageFormat.Png);

                bitmapStream.Seek(0, SeekOrigin.Begin);

                // Layer PNG Data Lenght
                writer.Write((int)bitmapStream.Length);
                bitmapStream.CopyTo(writer.BaseStream);
            }

            if (document.Layers.Length == 0)
            {
                writer.Write(0);
            }
        }
    }
}