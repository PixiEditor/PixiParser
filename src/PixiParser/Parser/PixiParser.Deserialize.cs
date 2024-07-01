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
        using var stream = File.OpenRead(path);
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
                ReadImage(stream, cancellationToken, image, document);
            }

            if (member is IMaskable { Mask: IImageContainer mask })
            {
                ReadImage(stream, cancellationToken, mask, document);
            }
        }

        if (document.AnimationData != null)
        {
            List<IKeyFrame> keyFrames = document.AnimationData.KeyFrameGroups.Cast<IKeyFrame>().ToList();
            ReadKeyFrameImages(stream, cancellationToken, keyFrames, document);
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
            document = await MessagePackSerializer
                .DeserializeAsync<Document>(stream, MessagePackOptions, cancellationToken).ConfigureAwait(false);
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
                await ReadImageAsync(stream, cancellationToken, image, document);
            }

            if (member is IMaskable { Mask: IImageContainer mask })
            {
                await ReadImageAsync(stream, cancellationToken, mask, document);
            }
        }

        if (document.AnimationData != null)
        {
            List<IKeyFrame> keyFrames = document.AnimationData.KeyFrameGroups.Cast<IKeyFrame>().ToList();
            await ReadKeyFrameImagesAsync(stream, cancellationToken, keyFrames, document);
        }

        return document;
    }

    private static async Task ReadKeyFrameImagesAsync(Stream stream, CancellationToken cancellationToken,
        List<IKeyFrame> keyFrameGroups,
        Document document)
    {
        foreach (var keyFrame in keyFrameGroups)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (keyFrame is IKeyFrameChildrenContainer container && container.Children.Count != 0)
            {
                await ReadKeyFrameImagesAsync(stream, cancellationToken, container.Children, document);
            }

            if (keyFrame is IImageContainer image)
            {
                await ReadImageAsync(stream, cancellationToken, image, document);
            }
        }
    }
    
    private static void ReadKeyFrameImages(Stream stream, CancellationToken cancellationToken,
        List<IKeyFrame> keyFrameGroups,
        Document document)
    {
        foreach (var keyFrame in keyFrameGroups)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (keyFrame is IKeyFrameChildrenContainer container && container.Children.Count != 0)
            {
                ReadKeyFrameImages(stream, cancellationToken, container.Children, document);
            }

            if (keyFrame is IImageContainer image)
            {
                ReadImage(stream, cancellationToken, image, document);
            }
        }
    }

    private static async Task ReadImageAsync(Stream stream, CancellationToken cancellationToken, IImageContainer image,
        Document document)
    {
        int bytesRead;
        int totalRead = 0;
        image.ImageBytes = new byte[image.ResourceSize];

        bytesRead = 0;
        do
        {
            bytesRead = await stream
                .ReadAsync(image.ImageBytes, 0, image.ImageBytes.Length - bytesRead, cancellationToken)
                .ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            totalRead += bytesRead;
        } while (bytesRead > 0);

        if (totalRead != image.ResourceSize)
        {
            ThrowInvalidResourceSize(image, document, totalRead, stream);
        }
    }
    
    private static void ReadImage(Stream stream, CancellationToken cancellationToken, IImageContainer image,
        Document document)
    {
        int bytesRead;
        int totalRead = 0;
        image.ImageBytes = new byte[image.ResourceSize];

        bytesRead = 0;
        do
        {
            bytesRead = stream.Read(image.ImageBytes, 0, image.ImageBytes.Length - bytesRead);
            cancellationToken.ThrowIfCancellationRequested();
            totalRead += bytesRead;
        } while (bytesRead > 0);

        if (totalRead != image.ResourceSize)
        {
            ThrowInvalidResourceSize(image, document, totalRead, stream);
        }
    }

    private static (Version version, Version minVersion)? ValidateHeader(int bytesRead, ReadOnlySpan<byte> header,
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

    private static Version ReadVersion(ReadOnlySpan<byte> buffer)
    {
        byte[] byteBuffer = buffer.ToArray();
        int major = BitConverter.ToInt32(byteBuffer, 0);
        int minor = BitConverter.ToInt32(byteBuffer, 4);

        return new Version(major, minor);
    }

    private static void ThrowInvalidMessagePack((Version version, Version minVersion) version, Exception e) =>
        throw new InvalidFileException("Failed to deserialize message pack of document", e)
        {
            Document = new Document { Version = version.version, MinVersion = version.minVersion }
        };

    private static void
        ThrowInvalidResourceSize(IImageContainer member, Document document, int totalRead, Stream stream) =>
        throw new InvalidFileException(
            $"Expected to read {member.ResourceSize} bytes, but only read {totalRead} bytes. Expected at offset {member.ResourceSize} with the size {member.ResourceSize} (Current Stream Position: {stream.Position}).")
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

    public static Task<byte[]> ReadPreviewAsync(Stream stream, CancellationToken cancellationToken = default) =>
        ReadPreviewAsync(stream, cancellationToken, true);

    private static async Task<byte[]> ReadPreviewAsync(Stream stream, CancellationToken cancellationToken,
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