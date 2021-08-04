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
            document.FileVersion = FileVersion;

            EnsureLayerPng(document);

            byte[] messagePack = MessagePackSerializer.Serialize(document, MessagePack.Resolvers.StandardResolverAllowPrivate.Options);

            stream.Write(BitConverter.GetBytes(messagePack.Length));
            stream.Write(messagePack);
        }

        /// <summary>
        /// Serializes a <see cref="SerializableDocument"/> into bytes.
        /// </summary>
        /// <param name="document">The document to serialize.</param>
        /// <returns>The serialized bytes.</returns>
        public static byte[] Serialize(SerializableDocument document)
        {
            MemoryStream stream = new();

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

        private static void EnsureLayerPng(SerializableDocument document)
        {
            foreach (SerializableLayer layer in document)
            {
                if (layer.PngBytes != null || layer.Width * layer.Height == 0)
                {
                    continue;
                }

                if (layer.BitmapBytes == null)
                {
                    layer.PngBytes = null;
                }

                using MemoryStream stream = new();
                using Bitmap bitmap = layer.ToBitmap();

                bitmap.Save(stream, ImageFormat.Png);

                layer.PngBytes = stream.ToArray();
            }
        }
    }
}