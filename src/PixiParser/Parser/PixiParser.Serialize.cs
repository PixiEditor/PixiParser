﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;

namespace PixiEditor.Parser;

public partial class PixiParser
{
    public static void Serialize(Document document, string path, CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        Serialize(document, stream, cancellationToken);
    }

    public static byte[] Serialize(Document document, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        Serialize(document, stream, cancellationToken);
        return stream.ToArray();
    }

    public static async Task SerializeAsync(Document document, string path,
        CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        await SerializeAsync(document, stream, cancellationToken).ConfigureAwait(false);
    }

    public static async Task SerializeAsync(Document document, Stream stream,
        CancellationToken cancellationToken = default)
    {
        document.Version = FileVersion;
        document.MinVersion = MinSupportedVersion;

        byte[] header = GetHeader();

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


        List<IImageContainer> imageContainers = new();

        if (document.ReferenceLayer != null)
        {
            imageContainers.Add(document.ReferenceLayer);
        }
        
        if (document.Graph != null)
        {
            imageContainers.AddRange(document.Graph.CollectImageContainers());
        }

        await MessagePackSerializer.SerializeAsync(stream, document, MessagePackOptions, cancellationToken)
            .ConfigureAwait(false);

        foreach (var resource in imageContainers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await stream.WriteAsync(resource.ImageBytes, 0, resource.ImageBytes.Length, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public static void Serialize(Document document, Stream stream, CancellationToken cancellationToken = default)
    {
        document.Version = FileVersion;
        document.MinVersion = MinSupportedVersion;

        byte[] header = GetHeader();
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

        cancellationToken.ThrowIfCancellationRequested();

        List<IImageContainer> imageContainers = new();
        
        if (document.Graph != null)
        {
            imageContainers.AddRange(document.Graph.CollectImageContainers());
        }

        if (document.ReferenceLayer != null)
        {
            imageContainers.Add(document.ReferenceLayer);
        }

        var msg = MessagePackSerializer.Serialize(document, MessagePackOptions, cancellationToken);
        stream.Write(msg, 0, msg.Length);

        foreach (var resource in imageContainers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            stream.Write(resource.ImageBytes, 0, resource.ImageBytes.Length);
        }
    }

    private static byte[] GetHeader()
    {
        byte[] header = new byte[HeaderLength];
        Magic.CopyTo(header, 0);

        WriteVersion(header, FileVersion, MagicLength);
        WriteVersion(header, MinSupportedVersion, MagicLength + 8);

        return header;
    }

    private static void WriteVersion(byte[] buffer, Version version, int offset)
    {
        BitConverter.GetBytes(version.Major).CopyTo(buffer, offset);
        BitConverter.GetBytes(version.Minor).CopyTo(buffer, offset + 4);
    }
}