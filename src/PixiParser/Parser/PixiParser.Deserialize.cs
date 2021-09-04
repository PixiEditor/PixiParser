using MessagePack;
using System;
using System.IO;
using System.Linq;

namespace PixiEditor.Parser
{
    public static partial class PixiParser
    {
        static readonly byte[] oldFormatIdentifier =
            new byte[] { 0x41, 0x50, 0x69, 0x78, 0x69, 0x45, 0x64, 0x69 };

        /// <summary>
        /// Deserializes a stream containing a .pixi file to a <see cref="SerializableDocument"/>.
        /// </summary>
        /// <returns>The deserialized document.</returns>
        public static SerializableDocument Deserialize(Stream stream) => Deserialize(stream, out _);

        /// <summary>
        /// Deserializes a stream containing a .pixi file to a <see cref="SerializableDocument"/>.
        /// </summary>
        /// <param name="stream">The stream to deserialize from</param>
        /// <param name="bytesRead">The total number of bytes read from the stream</param>
        /// <returns>The deserialized document.</returns>
        public static SerializableDocument Deserialize(Stream stream, out int bytesRead)
        {
            ThrowIfOldFormat(stream);

            byte[] msgPack = GetMessagePack(stream, out bytesRead);

            SerializableDocument document = ParseDocument(msgPack);

            if (document.FileVersion < new Version(2, 0))
            {
                ParseLayers(ref document, stream);
            }

            return document;
        }

        /// <summary>
        /// Deserializes to a <see cref="SerializableDocument"/>.
        /// </summary>
        /// <returns>The deserialized Document.</returns>
        public static SerializableDocument Deserialize(byte[] bytes) => Deserialize(new MemoryStream(bytes));

        /// <summary>
        /// Deserializes a .pixi file to a <see cref="SerializableDocument"/>
        /// </summary>
        /// <param name="path">The path to the .pixi file</param>
        /// <returns>The deserialized document.</returns>
        public static SerializableDocument Deserialize(string path)
        {
            using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            return Deserialize(stream);
        }

        private static void ThrowIfOldFormat(Stream stream)
        {
            if (!stream.CanSeek || stream.Length <= 44)
            {
                return;
            }

            stream.Position += 22;

            byte[] buffer = new byte[8];

#if NET5_0_OR_GREATER
            stream.Read(buffer);
#else
            stream.Read(buffer, 0, 8);
#endif

            stream.Position -= 22 + 8;

            if (buffer.SequenceEqual(oldFormatIdentifier))
            {
                throw new OldFileFormatException();
            }
        }

        private static byte[] GetMessagePack(Stream stream, out int bytesRead)
        {
            byte[] lengthBytes = new byte[4];

#if NET5_0_OR_GREATER
            stream.Read(lengthBytes);
            int length = BitConverter.ToInt32(lengthBytes);
#else
            stream.Read(lengthBytes, 0, 4);
            int length = BitConverter.ToInt32(lengthBytes, 0);
#endif
            byte[] buffer = new byte[length];

#if NET5_0_OR_GREATER
            int read = stream.Read(buffer);
#else
            int read = stream.Read(buffer, 0, buffer.Length);
#endif

            bytesRead = read + 4;

            if (read != length)
            {
                throw new InvalidFileException("Stream size did not match document size");
            }

            return buffer;
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
            catch (MessagePackSerializationException e)
            {
                throw new InvalidFileException("Message Pack could not be deserialized", e);
            }
        }

        private static void ParseLayers(ref SerializableDocument document, Stream stream)
        {
            foreach (SerializableLayer layer in document)
            {
                byte[] layerLength = new byte[4];
                stream.Read(layerLength, 0, 4);
                int layerLengthI = BitConverter.ToInt32(layerLength, 0);

                if (layerLengthI == 0)
                {
                    continue;
                }

                try
                {
                    layer.PngBytes = new byte[layerLengthI];
                    stream.Read(layer.PngBytes, 0, layerLengthI);
                }
                catch (InvalidFileException)
                {
                    throw new InvalidFileException($"Parsing png bytes from layer ('{layer.Name}') failed");
                }
            }
        }
    }
}