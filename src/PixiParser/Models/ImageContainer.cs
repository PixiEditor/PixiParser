using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public class ImageContainer : IImageContainer
{
    [Key(0)]
    public byte[] ImageBytes { get; set; }
    
    [Key(1)]
    public int ResourceOffset { get; set; }
    
    [Key(2)]
    public int ResourceSize { get; set; }
}