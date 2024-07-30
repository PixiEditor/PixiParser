using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PixiEditor.Parser;

public interface IPixiParser
{
    #if NET5_0_OR_GREATER
    public void Serialize(IPixiDocument document, string path, CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        Serialize(document, stream, cancellationToken);
    }

    public byte[] Serialize(IPixiDocument document, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        Serialize(document, stream, cancellationToken);
        return stream.ToArray();
    }

    public async Task SerializeAsync(IPixiDocument document, string path,
        CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        await SerializeAsync(document, stream, cancellationToken).ConfigureAwait(false);
    }
    
    public IPixiDocument Deserialize(byte[] buffer, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(buffer);
        return Deserialize(stream, cancellationToken);
    }

    public IPixiDocument Deserialize(string path, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(path);
        return Deserialize(stream, cancellationToken);
    }

    public async Task<IPixiDocument> DeserializeAsync(string path, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(path);
        return await DeserializeAsync(stream, cancellationToken).ConfigureAwait(false);
    }
    #endif

    public Task SerializeAsync(IPixiDocument document, Stream stream, CancellationToken cancellationToken = default);
    public void Serialize(IPixiDocument document, Stream stream, CancellationToken cancellationToken = default);
    public IPixiDocument Deserialize(Stream stream, CancellationToken cancellationToken = default);
    public Task<IPixiDocument> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default);
}