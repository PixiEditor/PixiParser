using MessagePack;
using System;
using System.IO;
using System.Linq;

namespace PixiEditor.Parser.Deprecated;

/// <summary>
/// Use this class to parse pre 4.0 files.
/// </summary>
public static class DeprecatedPixiParser
{
    public static DeprecatedDocument Deserialize(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        return MessagePackSerializer.Deserialize<DeprecatedDocument>(stream);
    }

    public static DeprecatedDocument Deserialize(string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        using FileStream stream = new(path, FileMode.Open);
        return Deserialize(stream);
    }

    public static DeprecatedDocument Deserialize(byte[] stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using MemoryStream memoryStream = new(stream);
        return Deserialize(memoryStream);
    }
}
