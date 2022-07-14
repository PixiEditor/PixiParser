using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MessagePack;
using MessagePack.Resolvers;
using PixiEditor.Parser.Helpers;

namespace PixiEditor.Parser;

public partial class PixiParser
{
    public static void Serialize(Stream stream, Document document, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        stream.Write(Magic, 0, Magic.Length);
        
        cancellationToken.ThrowIfCancellationRequested();
        
        byte[] version = new byte[HeaderLength - MagicLength];
        
        WriteVersion(version, FileVersion, 0);
        WriteVersion(version, MinSupportedVersion, 8);
        
        stream.Write(version, 0, version.Length);
        
        cancellationToken.ThrowIfCancellationRequested();

        if (stream.Position != HeaderLength)
        {
            throw new ArgumentException($"Header length did not match the constant '{nameof(HeaderLength)}'");
        }
        
        int resourceOffset = 0;

        List<IImageContainer> resources = new(document.RootFolder.Children.Count + 1);
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
        
        var msg = MessagePackSerializer.Serialize(document, MessagePackOptions, cancellationToken);
        stream.Write(msg, 0, msg.Length);

        foreach (var resource in resources)
        {
            stream.Write(resource.ImageBytes, 0, resource.ImageBytes.Length);
        }
    }

    private static void WriteVersion(byte[] buffer, Version version, int offset)
    {
        BitConverter.GetBytes(version.Major).CopyTo(buffer, offset);
        BitConverter.GetBytes(version.Minor).CopyTo(buffer, offset + 4);
    }
}