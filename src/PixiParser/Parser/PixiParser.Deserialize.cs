using MessagePack;
using System;
using System.IO;
using System.Linq;

namespace PixiEditor.Parser;

public static partial class PixiParser
{
    static readonly byte[] oldFormatIdentifier =
        new byte[] { 0x41, 0x50, 0x69, 0x78, 0x69, 0x45, 0x64, 0x69 };

    /// <summary>
    /// Deserializes a stream containing a .pixi file to a <see cref="SerializableDocument"/>.
    /// </summary>
    /// <returns>The deserialized document.</returns>
    public static SerializableDocument Deserialize(Stream stream) => Deserialize(stream, out _);

    // Version when the layer bitmap were not stored in the message pack
    private static readonly Version parseLayerVersion = new(2, 0);

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

        if (document.FileVersion < parseLayerVersion)
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
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read);

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

        ReadBytesFromStream(stream, 8, buffer);

        stream.Position -= 22 + 8;

        if (buffer.SequenceEqual(oldFormatIdentifier))
        {
            throw new OldFileFormatException();
        }
    }

    private static byte[] GetMessagePack(Stream stream, out int bytesRead)
    {
        byte[] lengthBytes = new byte[4];

        ReadBytesFromStream(stream, 4, lengthBytes);
        int length = BitConverter.ToInt32(lengthBytes, 0);

        byte[] buffer = new byte[length];
        try
        {
            ReadBytesFromStream(stream, length, buffer);
        }
        catch (Exception e)
        {
            throw new InvalidFileException("Document size doesn't match stream size", e);
        }

        bytesRead = length + 4;

        return buffer;
    }

    private static void ReadBytesFromStream(Stream stream, int count, byte[] buffer)
    {
        if (buffer.Length < count)
            throw new ArgumentException($"The buffer is not big enough to read {count} bytes");
        int read = 0;
        while (read < count)
        {
            int newRead = stream.Read(buffer, read, count - read);
            if (newRead == 0)
                throw new Exception($"The stream doesn't contain {count} bytes");
            read += newRead;
        }
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
            ReadBytesFromStream(stream, 4, layerLength);
            int layerLengthI = BitConverter.ToInt32(layerLength, 0);

            if (layerLengthI == 0)
            {
                continue;
            }

            try
            {
                layer.PngBytes = new byte[layerLengthI];
                ReadBytesFromStream(stream, layerLengthI, layer.PngBytes);
            }
            catch (InvalidFileException)
            {
                throw new InvalidFileException($"Parsing png bytes from layer ('{layer.Name}') failed");
            }
        }
    }
}
