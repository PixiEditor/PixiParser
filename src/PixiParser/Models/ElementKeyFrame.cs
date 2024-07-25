using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public class ElementKeyFrame
{ 
    [Key(0)]
    public int KeyFrameId { get; set; } 
    
    [Key(1)]
    public int NodeId { get; set; }
}