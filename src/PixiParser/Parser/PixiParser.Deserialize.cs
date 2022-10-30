using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using PixiEditor.Parser.Helpers;

namespace PixiEditor.Parser;

public partial class PixiParser
{
    public static Document Deserialize(byte[] buffer, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(buffer);
        return Deserialize(stream, cancellationToken);
    }
    
    public static Document Deserialize(string path, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(path);
        return Deserialize(stream, cancellationToken);
    }
    
    public static async Task<Document> DeserializeAsync(string path, CancellationToken cancellationToken = default)
    {
        #if NET5_0_OR_GREATER
        await using var stream = File.OpenRead(path);
        #else
        using var stream = File.OpenRead(path);
        #endif
        return await DeserializeAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    public static Document Deserialize(Stream stream, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var header = new byte[HeaderLength];
        
        var bytesRead = stream.Read(header, 0, HeaderLength);
        
        cancellationToken.ThrowIfCancellationRequested();

        var version = ValidateHeader(bytesRead, header)!.Value;

        byte[] preview = ReadPreview(stream, false);
        
        Document document;

        try
        {
            document = MessagePackSerializer.Deserialize<Document>(stream, MessagePackOptions, cancellationToken);
        }
        catch (Exception e)
        {
            ThrowInvalidMessagePack(version, e);
            // Make compiler happy :]
            return null;
        }
        
        document.Version = version.version;
        document.MinVersion = version.minVersion;
        
        document.PreviewImage = preview;

        var members = document.RootFolder.GetChildrenRecursive().ToList();
        
        if (document.ReferenceLayer != null)
        {
            members.Add(document.ReferenceLayer);
        }
        
        foreach (var member in members)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (member is IImageContainer image)
            {
                int totalRead = 0;
                image.ImageBytes = new byte[image.ResourceSize];

                bytesRead = 0;
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    bytesRead = stream.Read(image.ImageBytes, 0, image.ImageBytes.Length - bytesRead);
                    totalRead += bytesRead;
                } while (bytesRead > 0);
                
                if (totalRead != image.ResourceSize)
                {
                    ThrowInvalidResourceSize(image, document, totalRead, members, stream);
                }
            }

            if (member is IMaskable { Mask: IImageContainer mask })
            {
                int totalRead = 0;
                mask.ImageBytes = new byte[mask.ResourceSize];

                bytesRead = 0;
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    bytesRead = stream.Read(mask.ImageBytes, 0, mask.ImageBytes.Length - bytesRead);
                    totalRead += bytesRead;
                } while (bytesRead > 0);
                
                if (totalRead != mask.ResourceSize)
                {
                    ThrowInvalidResourceSize(mask, document, totalRead, members, stream);
                }
            }
        }

        return document;
    }
    
    public static async Task<Document> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var header = new byte[HeaderLength];
        
        var bytesRead = await stream.ReadAsync(header, 0, HeaderLength, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        var version = ValidateHeader(bytesRead, header)!.Value;
        
        if (FileVersion < version.minVersion)
        {
            throw new InvalidFileException(
                $".pixi version {version.version} is not supported by this parser. Minimum supported version is {version.minVersion}. (Parser Version: {FileVersion})")
            {
                Document = new Document { Version = version.version, MinVersion = version.minVersion }
            };
        }

        byte[] preview = await ReadPreviewAsync(stream, cancellationToken, false).ConfigureAwait(false);

        Document document;

        try
        {
            document = await MessagePackSerializer.DeserializeAsync<Document>(stream, MessagePackOptions, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            ThrowInvalidMessagePack(version, e);
            // Makes compiler happy :]
            return null;
        }

        document.Version = version.version;
        document.MinVersion = version.minVersion;

        document.PreviewImage = preview;
        
        var members = document.RootFolder.GetChildrenRecursive().ToList();
        
        if (document.ReferenceLayer != null)
        {
            members.Add(document.ReferenceLayer);
        }
        
        foreach (var member in members)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (member is IImageContainer image)
            {
                int totalRead = 0;
                image.ImageBytes = new byte[image.ResourceSize];

                bytesRead = 0;
                do
                {
                    #if NET5_0_OR_GREATER
                    bytesRead = await stream.ReadAsync(image.ImageBytes.AsMemory(0, image.ImageBytes.Length - bytesRead), cancellationToken).ConfigureAwait(false);
                    #else
                    bytesRead = await stream.ReadAsync(image.ImageBytes, 0, image.ImageBytes.Length - bytesRead, cancellationToken).ConfigureAwait(false);
                    #endif
                    cancellationToken.ThrowIfCancellationRequested();
                    totalRead += bytesRead;
                } while (bytesRead > 0);
                
                if (totalRead != image.ResourceSize)
                {
                    ThrowInvalidResourceSize(image, document, totalRead, members, stream);
                }
            }

            if (member is IMaskable { Mask: IImageContainer mask })
            {
                int totalRead = 0;
                mask.ImageBytes = new byte[mask.ResourceSize];

                bytesRead = 0;
                do
                {
                    #if NET5_0_OR_GREATER
                    bytesRead = await stream.ReadAsync(mask.ImageBytes.AsMemory(0, mask.ImageBytes.Length - bytesRead), cancellationToken).ConfigureAwait(false);
                    #else
                    bytesRead = await stream.ReadAsync(mask.ImageBytes, 0, mask.ImageBytes.Length - bytesRead, cancellationToken).ConfigureAwait(false);
                    #endif
                    cancellationToken.ThrowIfCancellationRequested();
                    totalRead += bytesRead;
                } while (bytesRead > 0);
                
                if (totalRead != mask.ResourceSize)
                {
                    ThrowInvalidResourceSize(mask, document, totalRead, members, stream);
                }
            }
        }

        return document;
    }
    
    private static (Version version, Version minVersion)? ValidateHeader(int bytesRead, ReadOnlySpan<byte> header, bool skipVersion = false)
    {
        if (bytesRead != HeaderLength)
        {
            throw new InvalidFileException($"Header was not of expected length. Expected {HeaderLength} bytes, but got {bytesRead} bytes.");
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

    private static Version ReadVersion(ReadOnlySpan<byte> buffer)
    {
        #if NET5_0_OR_GREATER
        int major = BitConverter.ToInt32(buffer[..4]);
        int minor = BitConverter.ToInt32(buffer[4..8]);
        #else
        byte[] byteBuffer = buffer.ToArray();
        int major = BitConverter.ToInt32(byteBuffer, 0);
        int minor = BitConverter.ToInt32(byteBuffer, 4);
        #endif

        return new Version(major, minor);
    }

    private static void ThrowInvalidMessagePack((Version version, Version minVersion) version, Exception e) =>
        throw new InvalidFileException("Failed to deserialize message pack of document", e)
        {
            Document = new Document { Version = version.version, MinVersion = version.minVersion }
        };

    private static void
        ThrowInvalidResourceSize(IImageContainer member, Document document, int totalRead,
            IEnumerable<IStructureMember> members, Stream stream) => throw new InvalidFileException(
        $"Expected to read {member.ResourceSize} bytes, but only read {totalRead} bytes for layer {member.GetDebugName(members) ?? "{null}"}. Expected at offset {member.ResourceSize} with the size {member.ResourceSize} (Current Stream Position: {stream.Position}).")
    {
        Document = document
    };

    public static byte[] ReadPreview(Stream stream) => ReadPreview(stream, true);

    private static byte[] ReadPreview(Stream stream, bool checkHeader)
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

    public static Task<byte[]> ReadPreviewAsync(Stream stream, CancellationToken cancellationToken = default) => ReadPreviewAsync(stream, cancellationToken, true);
    
    private static async Task<byte[]> ReadPreviewAsync(Stream stream, CancellationToken cancellationToken, bool checkHeader)
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
            bytesRead = await stream.ReadAsync(previewData, 0, previewData.Length, cancellationToken).ConfigureAwait(false);
            left -= bytesRead;
        } while (bytesRead != 0 && left != 0);
        
        if (left != 0)
        {
            throw new InvalidFileException("Reached end of stream while reading preview");
        }

        return previewData;
    }
}