using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
[DebuggerDisplay("{Resources.Count} resources")]
public class ResourceStorage
{
    [Key(0)] public List<EmbeddedResource> Resources { get; set; } = new();

    public int AddFromFilePath(string filePath)
    {
        var existing = Resources.FirstOrDefault(r => r.SourcePath == filePath);
        if (existing != null)
        {
            return existing.Handle;
        }

        try
        {
            int handle = Resources.Count;
            Resources.Add(new EmbeddedResource
            {
                Handle = handle,
                FileName = Path.GetFileName(filePath),
                Data = System.IO.File.ReadAllBytes(filePath),
                SourcePath = filePath,
            });

            return handle;
        }
        catch (IOException e)
        {
            throw new IOException($"Failed to read file {filePath}", e);
        }
    }

    public int AddFromBytes(string fileName, byte[] data)
    {
        var existing = Resources.FirstOrDefault(r => r.FileName == fileName);
        if (existing != null)
        {
            return existing.Handle;
        }

        int handle = Resources.Count;
        Resources.Add(new EmbeddedResource { Handle = handle, FileName = fileName, Data = data });

        return handle;
    }
}

[MessagePackObject]
public class EmbeddedResource
{
    [Key(0)] public int Handle { get; set; }
    [Key(1)] public string FileName { get; set; }
    [Key(2)] public byte[] Data { get; set; }
    [Key(3)] public Guid CacheId { get; set; } = Guid.NewGuid();

    [IgnoreMember]
    internal string? SourcePath { get; set; }
}
