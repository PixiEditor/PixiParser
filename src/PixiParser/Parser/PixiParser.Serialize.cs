using MessagePack;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

            byte[] messagePack = MessagePackSerializer.Serialize(document, MessagePack.Resolvers.StandardResolverAllowPrivate.Options);

            writer.Write(messagePack.Length);
            writer.Write(messagePack);

            WriteLayers(document, writer);
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

                // Layer PNG Data Length
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