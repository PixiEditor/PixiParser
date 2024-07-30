using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PixiEditor.Parser;

#if NETSTANDARD
public static class IPixiParserExtensions
{
    public static void Serialize(this IPixiParser parser, IPixiDocument document, string path, CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        parser.Serialize(document, stream, cancellationToken);
    }

    public static byte[] Serialize(this IPixiParser parser, IPixiDocument document, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        parser.Serialize(document, stream, cancellationToken);
        return stream.ToArray();
    }

    public static async Task SerializeAsync(this IPixiParser parser, IPixiDocument document, string path,
        CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        await parser.SerializeAsync(document, stream, cancellationToken).ConfigureAwait(false);
    }
    
    public static IPixiDocument Deserialize(this IPixiParser parser, byte[] buffer, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(buffer);
        return parser.Deserialize(stream, cancellationToken);
    }

    public static IPixiDocument Deserialize(this IPixiParser parser, string path, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(path);
        return parser.Deserialize(stream, cancellationToken);
    }

    public static async Task<IPixiDocument> DeserializeAsync(this IPixiParser parser, string path, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(path);
        return await parser.DeserializeAsync(stream, cancellationToken).ConfigureAwait(false);
    }
}
#endif