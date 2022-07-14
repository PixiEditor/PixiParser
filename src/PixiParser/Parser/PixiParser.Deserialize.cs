using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;
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
    //
    // public static async ValueTask<Document> DeserializeAsync(string path, CancellationToken cancellationToken = default)
    // {
    //     using var stream = File.OpenRead(path);
    //     return await DeserializeAsync(stream, cancellationToken).ConfigureAwait(false);
    // }

    public static Document Deserialize(Stream stream, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var header = new byte[HeaderLength];
        
        var bytesRead = stream.Read(header, 0, HeaderLength);
        
        cancellationToken.ThrowIfCancellationRequested();

        ValidateHeader(bytesRead, header);
        
        var document = MessagePackSerializer.Deserialize<Document>(stream, MessagePackOptions, cancellationToken);
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
                    ThrowInvalidResourceSize(image, totalRead, members, stream);
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
                    ThrowInvalidResourceSize(mask, totalRead, members, stream);
                }
            }
        }

        return document;
        
    }
    
    private static void ValidateHeader(int bytesRead, byte[] header)
    {
        if (bytesRead != HeaderLength)
        {
            throw new InvalidFileException("Header was not of expected length");
        }
        
        if (!Magic.SequenceEqual(header.Take(MagicLength)))
        {
            throw new InvalidFileException("Magic did not match");
        }
    }
    
    private static void ThrowInvalidResourceSize(IImageContainer member, int totalRead, IEnumerable<IStructureMember> members, Stream stream) => throw new InvalidFileException($"Expected to read {member.ResourceSize} bytes, but only read {totalRead} bytes for layer {member.GetDebugName(members) ?? "{null}"}. Expected at offset {member.ResourceSize} with the size {member.ResourceSize} (Current Stream Position: {stream.Position}).");
}