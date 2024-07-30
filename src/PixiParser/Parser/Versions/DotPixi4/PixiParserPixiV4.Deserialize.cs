using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PixiEditor.Parser.Old.PixiV4;
using PixiEditor.Parser.Old.PixiV4.Helpers;
using PixiEditor.Parser.Old.PixiV4.Interfaces;

namespace PixiEditor.Parser.Versions.DotPixi4;

/// <summary>
/// Use this class to parse pre 4.0 files.
/// </summary>
internal partial class PixiParserPixiV4
{
    public override DocumentV4 Deserialize(Stream stream, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var header = new byte[HeaderLength];

        var bytesRead = stream.Read(header, 0, HeaderLength);

        cancellationToken.ThrowIfCancellationRequested();

        var version = ValidateHeader(bytesRead, header)!.Value;

        byte[] preview = ReadPreview(stream, false);

        DocumentV4 document;

        try
        {
            document = MessagePackSerializer.Deserialize<DocumentV4>(stream, MessagePackOptions, cancellationToken);
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

    public override async Task<DocumentV4> DeserializeAsync(Stream stream,
        CancellationToken cancellationToken = default)
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

        DocumentV4 document;

        try
        {
            document = await MessagePackSerializer
                .DeserializeAsync<DocumentV4>(stream, MessagePackOptions, cancellationToken).ConfigureAwait(false);
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
                    bytesRead = await stream
                        .ReadAsync(image.ImageBytes.AsMemory(0, image.ImageBytes.Length - bytesRead), cancellationToken)
                        .ConfigureAwait(false);
#else
                    bytesRead =
 await stream.ReadAsync(image.ImageBytes, 0, image.ImageBytes.Length - bytesRead, cancellationToken).ConfigureAwait(false);
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
                    bytesRead = await stream.ReadAsync(mask.ImageBytes.AsMemory(0, mask.ImageBytes.Length - bytesRead),
                        cancellationToken).ConfigureAwait(false);
#else
                    bytesRead =
 await stream.ReadAsync(mask.ImageBytes, 0, mask.ImageBytes.Length - bytesRead, cancellationToken).ConfigureAwait(false);
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

    private void ThrowInvalidMessagePack((Version version, Version minVersion) version, Exception e) =>
        throw new InvalidFileException("Failed to deserialize message pack of document", e)
        {
            Document = new DocumentV4 { Version = version.version, MinVersion = version.minVersion },
            Parser = this
        };

    private void
        ThrowInvalidResourceSize(IImageContainer member, DocumentV4 document, int totalRead,
            IEnumerable<IStructureMember> members, Stream stream) => throw new InvalidFileException(
        $"Expected to read {member.ResourceSize} bytes, but only read {totalRead} bytes for layer {member.GetDebugName(members) ?? "{null}"}. Expected at offset {member.ResourceSize} with the size {member.ResourceSize} (Current Stream Position: {stream.Position}).")
    {
        Document = document,
        Parser = this
    };
}