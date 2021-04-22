using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using MessagePack;

namespace PixiEditor.Parser
{
    public static partial class PixiParser
    {
        const ulong oldFormatIdentifier = 0x41_50_69_78_69_45_64_69;

        /// <summary>
        /// Deserializes to a <see cref="SerializableDocument"/>.
        /// </summary>
        /// <returns>The deserialized Document.</returns>
        public static SerializableDocument Deserialize(Span<byte> span)
        {
            Span<byte> oldFileFormat = span.Slice(22, 8);

            // The old format always begins with the same bytes
            if (BitConverter.ToUInt64(oldFileFormat) == oldFormatIdentifier)
            {
                throw new OldFileFormatException("This is a old .pixi file. Use DeserializeOld() to deserialize it");
            }

            int pos = 0;
            Span<byte> messagePackBytes;

            try
            {
                messagePackBytes = GetMessagePackBytes(span, ref pos);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidFileException("Invalid message pack lenght");
            }

            SerializableDocument document;

            try
            {
                document = ParseMessagePack(messagePackBytes);
            }
            catch (MessagePackSerializationException)
            {
                throw new InvalidFileException("Message Pack could not be deserialize");
            }

            ParseLayerPNGs(ref document, span, ref pos);

            return document;
        }

        /// <summary>
        /// Deserializes to a <see cref="SerializableDocument"/>.
        /// </summary>
        /// <returns>The deserialized Document.</returns>
        public static SerializableDocument Deserialize(byte[] bytes)
        {
            return Deserialize(new Span<byte>(bytes));
        }

        /// <summary>
        /// Deserializes to a <see cref="SerializableDocument"/>.
        /// </summary>
        /// <param name="path">The stream to deserialize from</param>
        /// <param name="streamPosition">From where to start reading the stream, use null to start from current position.</param>
        /// <returns>The deserialized Document.</returns>
        public static SerializableDocument Deserialize(Stream stream, int? streamPosition = 0)
        {
            Span<byte> span = new Span<byte>(new byte[stream.Length]);
            if (streamPosition.HasValue)
            {
                stream.Position = streamPosition.Value;
            }
            stream.Read(span);

            return Deserialize(span);
        }

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

        /// <summary>
        /// Deserializes the old pixi format. Note: Only use this if you know that its a old PixiFile.
        /// </summary>
        /// <param name="stream">The stream to deserialize.</param>
        /// <returns>The deserialized Document.</returns>
        public static SerializableDocument DeserializeOld(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            formatter.Binder = new CurrentAssemblyDeserializationBinder();

            return (SerializableDocument)formatter.Deserialize(stream);
        }

        private static Span<byte> GetMessagePackBytes(Span<byte> span, ref int pos)
        {
            // First four bytes are message pack lenght
            int messagePackLenght = BitConverter.ToInt32(span.Slice(0, 4));

            // The message pack lenght can't be 0
            if (messagePackLenght == 0)
            {
                throw new InvalidFileException("This does not seem to be a .pixi file");
            }

            // At the fith byte the message pack begins
            Span<byte> messagePackBytes = span.Slice(4, messagePackLenght);

            pos += messagePackLenght + 4;

            return messagePackBytes;
        }

        private static SerializableDocument ParseMessagePack(Span<byte> bytes)
        {
            var document = MessagePackSerializer.Deserialize<SerializableDocument>(bytes.ToArray());
            document.Swatches = Helpers.BytesToSwatches(document.SwatchesData);

            return document;
        }

        private static void ParseLayerPNGs(ref SerializableDocument document, Span<byte> span, ref int pos)
        {
            int i = 0;

            // Deserialize layer data
            while (pos < span.Length && document.Layers.Length > i)
            {
                SerializableLayer layer = document.Layers[i];
                layer.MaxWidth = document.Width;
                layer.MaxHeight = document.Height;

                // Layer data lenght
                int layerLenght = BitConverter.ToInt32(span.Slice(pos, 4));

                if (layerLenght == 0)
                {
                    pos += 4;
                    layer.BitmapBytes = new byte[0];
                    continue;
                }

                pos += 4;

                try
                {
                    layer.BitmapBytes = ParsePNGToRawBytes(span.Slice(pos, layerLenght));
                }
                catch (InvalidFileException)
                {
                    throw new InvalidFileException($"Parsing layer (Index: {i}) failed");
                }

                pos += layerLenght;
                i++;
            }
        }

        private static byte[] ParsePNGToRawBytes(Span<byte> bytes)
        {
            byte[] rawLayerData;

            using MemoryStream pngStream = new MemoryStream(bytes.ToArray());
            using Bitmap png = (Bitmap)Image.FromStream(pngStream);

            BitmapData data = png.LockBits(new Rectangle(0, 0, png.Width, png.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            rawLayerData = new byte[Math.Abs(data.Stride * data.Height)];
            Marshal.Copy(data.Scan0, rawLayerData, 0, rawLayerData.Length);

            return rawLayerData;
        }
    }
}