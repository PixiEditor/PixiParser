using System;
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
    public static byte[] Serialize(Document document, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        Serialize(stream, document, cancellationToken);
        return stream.ToArray();
    }

    public static async Task SerializeAsync(string path, Document document, CancellationToken cancellationToken = default)
    {
        #if NET5_0_OR_GREATER
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        #else
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        #endif
        await SerializeAsync(stream, document, cancellationToken).ConfigureAwait(false);
    }
    
    public static async Task SerializeAsync(Stream stream, Document document, CancellationToken cancellationToken = default)
    {
        document.Version = FileVersion;
        document.MinVersion = MinSupportedVersion;
        
        byte[] header = GetHeader();
        
        #if NET5_0_OR_GREATER
        await stream.WriteAsync(header, cancellationToken).ConfigureAwait(false);
        #else
        await stream.WriteAsync(header, 0, header.Length, cancellationToken).ConfigureAwait(false);
        #endif
        
        cancellationToken.ThrowIfCancellationRequested();

        if (stream.Position != HeaderLength)
        {
            throw new ArgumentException($"Header length did not match the constant '{nameof(HeaderLength)}'");
        }

        var members = document.RootFolder.GetChildrenRecursive().ToList();
        
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

    public static void Serialize(Stream stream, Document document, CancellationToken cancellationToken = default)
    {
        document.Version = FileVersion;
        document.MinVersion = MinSupportedVersion;
        
        byte[] header = GetHeader();
        stream.Write(header, 0, header.Length);
        
        cancellationToken.ThrowIfCancellationRequested();

        if (stream.Position != HeaderLength)
        {
            throw new ArgumentException($"Header length did not match the constant '{nameof(HeaderLength)}'");
        }

        var members = document.RootFolder.GetChildrenRecursive().ToList();
        
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

    private static void WriteVersion(byte[] buffer, Version version, int offset)
    {
        BitConverter.GetBytes(version.Major).CopyTo(buffer, offset);
        BitConverter.GetBytes(version.Minor).CopyTo(buffer, offset + 4);
    }
}