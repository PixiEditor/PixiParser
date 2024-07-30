using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using PixiEditor.Parser.Old.PixiV4;
using PixiEditor.Parser.Old.PixiV4.Helpers;
using PixiEditor.Parser.Old.PixiV4.Interfaces;

namespace PixiEditor.Parser.Versions.DotPixi4;

internal partial class PixiParserPixiV4
{
    public override async Task SerializeAsync(DocumentV4 document, Stream stream, CancellationToken cancellationToken = default)
    {
        document.Version = FileVersion;
        document.MinVersion = MinSupportedVersion;
        
        byte[] header = GetHeader();
        
        #if NET5_0_OR_GREATER
        await stream.WriteAsync(header, cancellationToken).ConfigureAwait(false);
        #else
        await stream.WriteAsync(header, 0, header.Length, cancellationToken).ConfigureAwait(false);
        #endif

        if (stream.Position != HeaderLength)
        {
            throw new ArgumentException($"Expected header length of {HeaderLength} but got {stream.Position}");
        }

        if (document.PreviewImage != null && document.PreviewImage.Length != 0)
        {
            byte[] preview = document.PreviewImage;
            int previewLength = preview.Length;

            #if NET5_0_OR_GREATER
            await stream.WriteAsync(BitConverter.GetBytes(previewLength), cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(preview, cancellationToken).ConfigureAwait(false);
            #else
            await stream.WriteAsync(BitConverter.GetBytes(previewLength), 0, sizeof(int), cancellationToken)
                .ConfigureAwait(false);
            await stream.WriteAsync(preview, 0, previewLength, cancellationToken).ConfigureAwait(false);
            #endif
        }
        else
        {
            await stream.WriteAsync(new byte[4], 0, 4, cancellationToken).ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();


        var members = document.RootFolder?.GetChildrenRecursive().ToList() ?? new List<IStructureMember>();
        
        if (document.ReferenceLayer != null)
        {
            members.Add(document.ReferenceLayer);
        }

        var resources = GetResources(members, cancellationToken);

        await MessagePackSerializer.SerializeAsync(stream,document, MessagePackOptions, cancellationToken).ConfigureAwait(false);

        foreach (var resource in resources)
        {
            cancellationToken.ThrowIfCancellationRequested();
            #if NET5_0_OR_GREATER
            await stream.WriteAsync(resource.ImageBytes.AsMemory(0, resource.ImageBytes.Length), cancellationToken).ConfigureAwait(false);
            #else
            await stream.WriteAsync(resource.ImageBytes, 0, resource.ImageBytes.Length, cancellationToken).ConfigureAwait(false);
            #endif
        }
    }

    public override void Serialize(DocumentV4 document, Stream stream, CancellationToken cancellationToken = default)
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
            
            #if NET5_0_OR_GREATER
            stream.Write(BitConverter.GetBytes(preview.Length));
            stream.Write(preview);
            #else
            stream.Write(BitConverter.GetBytes(preview.Length), 0, sizeof(int));
            stream.Write(preview, 0, preview.Length);
            #endif
        }
        else
        {
            stream.Write(new byte[4], 0, 4);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var members = document.RootFolder?.GetChildrenRecursive().ToList() ?? new List<IStructureMember>();
        
        if (document.ReferenceLayer != null)
        {
            members.Add(document.ReferenceLayer);
        }

        var resources = GetResources(members, cancellationToken);

        var msg = MessagePackSerializer.Serialize(document, MessagePackOptions, cancellationToken);
        stream.Write(msg, 0, msg.Length);

        foreach (var resource in resources)
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

    private static List<IImageContainer> GetResources(IEnumerable<IStructureMember> members, CancellationToken cancellationToken = default)
    {
        int resourceOffset = 0;
        List<IImageContainer> resources = new(members.Count() + 1);
        
        foreach (var member in members)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (member is IImageContainer image)
            {
                image.ResourceOffset = resourceOffset;
                image.ResourceSize = image.ImageBytes?.Length ?? 0;
                resourceOffset += image.ResourceSize;
                if (image.ResourceSize > 0)
                {
                    resources.Add(image);
                }
            }

            if (member is not IMaskable { Mask: IImageContainer maskImage }) continue;
            
            maskImage.ResourceOffset = resourceOffset;
            maskImage.ResourceSize = maskImage.ImageBytes?.Length ?? 0;
            resourceOffset += maskImage.ResourceSize;
            if (maskImage.ResourceSize > 0)
            {
                resources.Add(maskImage);
            }
        }

        return resources;
    }
}