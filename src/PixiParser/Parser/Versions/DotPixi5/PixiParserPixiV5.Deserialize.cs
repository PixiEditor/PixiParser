using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;

namespace PixiEditor.Parser.Versions.DotPixi5;

internal partial class PixiParserPixiV5
{
    public override async Task<Document> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default)
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

        return document;
    }

    public override Document Deserialize(Stream stream, CancellationToken cancellationToken = default)
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

        return document;
    }

    private void ThrowInvalidMessagePack((Version version, Version minVersion) version, Exception e) =>
        throw new InvalidFileException("Failed to deserialize message pack of document", e)
        {
            Document = new Document { Version = version.version, MinVersion = version.minVersion },
            Parser = this
        };
}