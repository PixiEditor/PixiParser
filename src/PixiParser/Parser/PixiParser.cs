using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using PixiEditor.Parser.Old.PixiV4;
using PixiEditor.Parser.Versions.DotPixi4;
using PixiEditor.Parser.Versions.DotPixi5;

namespace PixiEditor.Parser;

public abstract class PixiParser
{
    internal PixiParser() { }
    
    public static Version FileVersion { get; } = new(5, 0);
    
    public static Version MinSupportedVersion { get; } = new(4, 0);

    protected static readonly byte[] Magic = [20, 50, 49, 58, 49];
    
    protected const int MagicLength = 5;
    
    protected const int HeaderLength = MagicLength + sizeof(int) * 4;

    public static PixiParser<Document> V5 { get; } = new PixiParserPixiV5();

    public static PixiParser<DocumentV4> V4 { get; } = new PixiParserPixiV4();

    public static IPixiDocument DeserializeUsingCompatible(Stream stream)
    {
        if (!TryGetCompatibleVersion(stream, out var parser))
        {
            throw new OldFileFormatException("This file was serialized pre .pixi 4.0 and is not supported");
        }

        return parser.Deserialize(stream);
    }

    public static async Task<IPixiDocument> DeserializeUsingCompatibleAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var parser = await GetCompatibleVersionAsync(stream, cancellationToken);

        if (parser == null)
        {
            throw new OldFileFormatException("This file was serialized pre .pixi 4.0 and is not supported");
        }

        return await parser.DeserializeAsync(stream, cancellationToken);
    }

    public static bool TryGetCompatibleVersion(Stream stream,
        #if NET5_0_OR_GREATER
        [NotNullWhen(true)]
        #endif
        out IPixiParser parser)
    {
        var header = new byte[HeaderLength];

        var bytesRead = stream.Read(header, 0, HeaderLength);
        var (version, _) = ValidateHeader(bytesRead, header)!.Value;
        stream.Seek(-bytesRead, SeekOrigin.Current);

        return TryGetCompatibleFromVersion(version, out parser);
    }
    
    public static async Task<IPixiParser> GetCompatibleVersionAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var header = new byte[HeaderLength];

        var bytesRead = await stream.ReadAsync(header, 0, HeaderLength, cancellationToken);
        var (version, _) = ValidateHeader(bytesRead, header)!.Value;
        stream.Seek(-bytesRead, SeekOrigin.Current);

        TryGetCompatibleFromVersion(version, out var parser);

        return parser;
    }

    private static bool TryGetCompatibleFromVersion(Version version, [NotNullWhen(true)] out IPixiParser parser)
    {
        parser = version.Major switch
        {
            4 => V4,
            5 => V5,
            _ => null
        };

        return parser != null;
    }

    public static bool TryGetCompatibleVersion(byte[] bytes,
    #if NET5_0_OR_GREATER
            [NotNullWhen(true)]
    #endif
        out IPixiParser parser)
    {
        var (version, _) = ValidateHeader(bytes.Length > HeaderLength ? HeaderLength : bytes.Length, bytes)!.Value;

        parser = version.Major switch
        {
            4 => V4,
            5 => V5,
            _ => null
        };

        return parser != null;
    }

    protected static void WriteVersion(byte[] buffer, Version version, int offset)
    {
        BitConverter.GetBytes(version.Major).CopyTo(buffer, offset);
        BitConverter.GetBytes(version.Minor).CopyTo(buffer, offset + 4);
    }
    
    protected static byte[] GetHeaderForSerialization()
    {
        byte[] header = new byte[HeaderLength];
        Magic.CopyTo(header, 0);

        WriteVersion(header, FileVersion, MagicLength);
        WriteVersion(header, MinSupportedVersion, MagicLength + 8);

        return header;
    }
    
    protected static Version ReadVersion(ReadOnlySpan<byte> buffer)
    {
        byte[] byteBuffer = buffer.ToArray();
        int major = BitConverter.ToInt32(byteBuffer, 0);
        int minor = BitConverter.ToInt32(byteBuffer, 4);

        return new Version(major, minor);
    }

    protected static (Version version, Version minVersion)? ValidateHeader(int bytesRead, ReadOnlySpan<byte> header,
        bool skipVersion = false)
    {
        if (bytesRead != HeaderLength)
        {
            throw new InvalidFileException(
                $"Header was not of expected length. Expected {HeaderLength} bytes, but got {bytesRead} bytes.");
        }

        if (!header.Slice(0, MagicLength).SequenceEqual(Magic))
        {
            throw new InvalidFileException("Header did not start with expected magic");
        }

        if (skipVersion)
        {
            return null;
        }

        return (ReadVersion(header.Slice(HeaderLength - 16, 8)), ReadVersion(header.Slice(HeaderLength - 8, 8)));
    }

    public static byte[] ReadPreview(Stream stream) => ReadPreview(stream, true);
    
    protected static byte[] ReadPreview(Stream stream, bool checkHeader)
    {
        int bytesRead;

        if (checkHeader)
        {
            var header = new byte[HeaderLength];

            bytesRead = stream.Read(header, 0, HeaderLength);

            _ = ValidateHeader(bytesRead, header);
        }

        byte[] previewLengthBytes = new byte[4];
        bytesRead = stream.Read(previewLengthBytes, 0, 4);

        if (bytesRead != 4)
        {
            throw new InvalidFileException();
        }

        byte[] previewData = new byte[BitConverter.ToInt32(previewLengthBytes, 0)];

        int left = previewData.Length;

        do
        {
            bytesRead = stream.Read(previewData, 0, previewData.Length);
            left -= bytesRead;
        } while (bytesRead != 0 && left != 0);

        if (left != 0)
        {
            throw new InvalidFileException("Reached end of stream while reading preview");
        }

        return previewData;
    }

    public static Task<byte[]> ReadPreviewAsync(Stream stream, CancellationToken cancellationToken = default) =>
        ReadPreviewAsync(stream, cancellationToken, true);

    public static async Task<byte[]> ReadPreviewAsync(Stream stream, CancellationToken cancellationToken,
        bool checkHeader)
    {
        int bytesRead;

        if (checkHeader)
        {
            var header = new byte[HeaderLength];

            bytesRead = await stream.ReadAsync(header, 0, HeaderLength, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            _ = ValidateHeader(bytesRead, header);
        }

        byte[] previewLengthBytes = new byte[4];
        bytesRead = await stream.ReadAsync(previewLengthBytes, 0, 4, cancellationToken).ConfigureAwait(false);

        if (bytesRead != 4)
        {
            throw new InvalidFileException("Reached end of stream while reading preview length");
        }

        byte[] previewData = new byte[BitConverter.ToInt32(previewLengthBytes, 0)];
        int left = previewData.Length;

        do
        {
            bytesRead = await stream.ReadAsync(previewData, 0, previewData.Length, cancellationToken)
                .ConfigureAwait(false);
            left -= bytesRead;
        } while (bytesRead != 0 && left != 0);

        if (left != 0)
        {
            throw new InvalidFileException("Reached end of stream while reading preview");
        }

        return previewData;
    }
}

public abstract class PixiParser<T> : PixiParser, IPixiParser where T : class, IPixiDocument
{
    internal PixiParser() { }

    Task IPixiParser.SerializeAsync(IPixiDocument document, Stream stream, CancellationToken cancellationToken) =>
        SerializeAsync((T)document, stream, cancellationToken);

    void IPixiParser.Serialize(IPixiDocument document, Stream stream, CancellationToken cancellationToken) =>
        Serialize((T)document, stream, cancellationToken);

    IPixiDocument IPixiParser.Deserialize(Stream stream, CancellationToken cancellationToken) =>
        Deserialize(stream, cancellationToken);

    async Task<IPixiDocument> IPixiParser.DeserializeAsync(Stream stream, CancellationToken cancellationToken)
    {
        var document = await DeserializeAsync(stream, cancellationToken);

        return document;
    }
    
    public void Serialize(T document, string path, CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        Serialize(document, stream, cancellationToken);
    }

    public byte[] Serialize(T document, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        Serialize(document, stream, cancellationToken);
        return stream.ToArray();
    }

    public async Task SerializeAsync(T document, string path,
        CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        await SerializeAsync(document, stream, cancellationToken).ConfigureAwait(false);
    }
    
    public T Deserialize(byte[] buffer, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(buffer);
        return Deserialize(stream, cancellationToken);
    }

    public T Deserialize(string path, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(path);
        return Deserialize(stream, cancellationToken);
    }

    public async Task<T> DeserializeAsync(string path, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(path);
        return await DeserializeAsync(stream, cancellationToken).ConfigureAwait(false);
    }
    
    public abstract Task SerializeAsync(T document, Stream stream, CancellationToken cancellationToken = default);
    public abstract void Serialize(T document, Stream stream, CancellationToken cancellationToken = default);
    public abstract T Deserialize(Stream stream, CancellationToken cancellationToken = default);
    public abstract Task<T> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default);
}