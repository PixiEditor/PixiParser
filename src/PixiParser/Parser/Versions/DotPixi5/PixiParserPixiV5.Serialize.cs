using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;

namespace PixiEditor.Parser.Versions.DotPixi5;

internal partial class PixiParserPixiV5
{
    public override async Task SerializeAsync(Document document, Stream stream,
        CancellationToken cancellationToken = default)
    {
        document.Version = FileVersion;
        document.MinVersion = MinSupportedVersion;

        byte[] header = GetHeaderForSerialization();

        await stream.WriteAsync(header, 0, header.Length, cancellationToken).ConfigureAwait(false);

        if (stream.Position != HeaderLength)
        {
            throw new ArgumentException($"Expected header length of {HeaderLength} but got {stream.Position}");
        }

        if (document.PreviewImage != null && document.PreviewImage.Length != 0)
        {
            byte[] preview = document.PreviewImage;
            int previewLength = preview.Length;

            await stream.WriteAsync(BitConverter.GetBytes(previewLength), 0, sizeof(int), cancellationToken)
                .ConfigureAwait(false);
            await stream.WriteAsync(preview, 0, previewLength, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await stream.WriteAsync(new byte[4], 0, 4, cancellationToken).ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();

        await MessagePackSerializer.SerializeAsync(stream, document, MessagePackOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public override void Serialize(Document document, Stream stream, CancellationToken cancellationToken = default)
    {
        document.Version = FileVersion;
        document.MinVersion = MinSupportedVersion;

        byte[] header = GetHeaderForSerialization();
        stream.Write(header, 0, header.Length);

        if (stream.Position != HeaderLength)
        {
            throw new ArgumentException($"Expected header length of {HeaderLength} but got {stream.Position}");
        }

        if (document.PreviewImage != null && document.PreviewImage.Length != 0)
        {
            byte[] preview = document.PreviewImage;

            stream.Write(BitConverter.GetBytes(preview.Length), 0, sizeof(int));
            stream.Write(preview, 0, preview.Length);
        }
        else
        {
            stream.Write(new byte[4], 0, 4);
        }

        var msg = MessagePackSerializer.Serialize(document, MessagePackOptions, cancellationToken);
        stream.Write(msg, 0, msg.Length);
    }
}