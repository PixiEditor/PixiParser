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
        /// Deserializes to a <see cref="SerializableDocument"/>.
        /// </summary>
        /// <param name="path">The stream to deserialize from</param>
        /// <param name="streamPosition">From where to start reading the stream, use null to start from current position.</param>
        /// <returns>The deserialized Document.</returns>
        public static SerializableDocument Deserialize(Stream stream)
        {
            ThrowIfOldFormat(stream);

            byte[] msgPack = GetMessagePack(stream);

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
        /// Deserializes to a <see cref="SerializableDocument"/>.
        /// </summary>
        /// <param name="path">The path to the .pixi file</param>
        /// <returns>The deserialized Document.</returns>
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
            stream.Read(buffer);

            stream.Position -= 22 + 8;

            if (buffer.SequenceEqual(oldFormatIdentifier))
            {
                throw new OldFileFormatException();
            }
        }

        private static byte[] GetMessagePack(Stream stream)
        {
            byte[] lenght = new byte[4];
            stream.Read(lenght);
            byte[] buffer = new byte[BitConverter.ToInt32(lenght)];
            stream.Read(buffer, 0, buffer.Length);
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
            catch (MessagePackSerializationException)
            {
                throw new InvalidFileException("Message Pack could not be deserialize");
            }
        }

        private static void ParseLayers(ref SerializableDocument document, Stream stream)
        {
            foreach (SerializableLayer layer in document)
            {
                byte[] layerLenght = new byte[4];
                stream.Read(layerLenght);
                int layerLenghtI = BitConverter.ToInt32(layerLenght);

                if (layerLenghtI == 0)
                {
                    continue;
                }

                try
                {
                    layer.PngBytes = new byte[layerLenghtI];
                    stream.Read(layer.PngBytes);
                }
                catch (InvalidFileException)
                {
                    throw new InvalidFileException($"Parsing layer ('{layer.Name}') failed");
                }
            }
        }
    }
}