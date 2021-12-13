using MessagePack;
using System;
using System.IO;

namespace PixiEditor.Parser;

public static partial class PixiParser
{
    /// <summary>
    /// Serializes a <see cref="SerializableDocument"/> into bytes and saves them into a stream.
    /// </summary>
    /// <param name="document">The document to serialize.</param>
    /// <returns>The total number of bytes written to the stream.</returns>
    public static int Serialize(SerializableDocument document, Stream stream)
    {
        document.FileVersion = FileVersion;

        byte[] messagePack = MessagePackSerializer.Serialize(document, MessagePack.Resolvers.StandardResolverAllowPrivate.Options);

#if NET5_0_OR_GREATER
            stream.Write(BitConverter.GetBytes(messagePack.Length));
            stream.Write(messagePack);
#else
        stream.Write(BitConverter.GetBytes(messagePack.Length), 0, 4);
        stream.Write(messagePack, 0, messagePack.Length);
#endif

        return messagePack.Length + 4;
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

#if NET5_0_OR_GREATER
            stream.Read(buffer);
#else
        stream.Read(buffer, 0, buffer.Length);
#endif

        return buffer;
    }

    /// <summary>
    /// Serializes a <see cref="SerializableDocument"/> and saves it in a file.
    /// </summary>
    /// <param name="document">The document to serialize.</param>
    /// <returns>The total number of bytes written to the file.</returns>
    public static int Serialize(SerializableDocument document, string path)
    {
        using FileStream stream = new(path, FileMode.Create, FileAccess.Write);

        return Serialize(document, stream);
    }
}
