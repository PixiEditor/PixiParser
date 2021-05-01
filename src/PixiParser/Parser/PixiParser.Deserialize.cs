using MessagePack;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace PixiEditor.Parser
{
    public static partial class PixiParser
    {
        const ulong oldFormatIdentifier = 0x41_50_69_78_69_45_64_69;

        /// <summary>
        /// Deserializes to a <see cref="SerializableDocument"/>.
        /// </summary>
        /// <param name="path">The stream to deserialize from</param>
        /// <param name="streamPosition">From where to start reading the stream, use null to start from current position.</param>
        /// <returns>The deserialized Document.</returns>
        public static SerializableDocument Deserialize(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            ThrowIfOldFormat(reader);

            byte[] msgPack = GetMessagePack(reader);

            SerializableDocument document = ParseDocument(msgPack);

            ParseLayers(ref document, reader);

            return document;
        }

        /// <summary>
        /// Deserializes to a <see cref="SerializableDocument"/>.
        /// </summary>
        /// <returns>The deserialized Document.</returns>
        public static SerializableDocument Deserialize(byte[] bytes) => Deserialize(new MemoryStream(bytes));

        /// <summary>
        /// Deserializes to a <see cref="SerializableDocument"/>.
        /// </summary>
        /// <param name="path">The path to the .pixi file</param>
        /// <returns>The deserialized Document.</returns>
        public static SerializableDocument Deserialize(string path)
        {
            using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            return Deserialize(stream);
        }

        private static void ThrowIfOldFormat(BinaryReader reader)
        {
            if (reader.BaseStream.CanSeek && reader.BaseStream.Length > 44)
            {
                reader.BaseStream.Position += 22;

                var oldFile = reader.ReadUInt64();

                reader.BaseStream.Position -= 22 + 8;

                if (oldFile == oldFormatIdentifier)
                {
                    throw new OldFileFormatException();
                }
            }
        }

        private static byte[] GetMessagePack(BinaryReader reader)
        {
            int messagePackLenght = reader.ReadInt32();
            return reader.ReadBytes(messagePackLenght);
        }

        private static SerializableDocument ParseDocument(byte[] messagePack)
        {
            try
            {
                return MessagePackSerializer.Deserialize<SerializableDocument>(
                    messagePack,
                    MessagePack.Resolvers.StandardResolverAllowPrivate.Options
                        .WithSecurity(MessagePackSecurity.UntrustedData));
            }
            catch (MessagePackSerializationException)
            {
                throw new InvalidFileException("Message Pack could not be deserialize");
            }
        }

        private static void ParseLayers(ref SerializableDocument document, BinaryReader reader)
        {
            foreach (SerializableLayer layer in document)
            {
                int layerLenght = reader.ReadInt32();

                byte[] layerBytes = reader.ReadBytes(layerLenght);

                try
                {
                    layer.BitmapBytes = ParsePNGToRawBytes(layerBytes);
                }
                catch (InvalidFileException)
                {
                    throw new InvalidFileException($"Parsing layer ('{layer.Name}') failed");
                }
            }
        }

        private static byte[] ParsePNGToRawBytes(byte[] bytes)
        {
            byte[] rawLayerData;

            using MemoryStream pngStream = new MemoryStream(bytes);
            using Bitmap png = (Bitmap)Image.FromStream(pngStream);

            BitmapData data = png.LockBits(new Rectangle(0, 0, png.Width, png.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            rawLayerData = new byte[Math.Abs(data.Stride * data.Height)];
            Marshal.Copy(data.Scan0, rawLayerData, 0, rawLayerData.Length);

            return rawLayerData;
        }
    }
}