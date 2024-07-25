using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public class KeyFrameData
{
    [Key(0)] 
    public int StartFrame { get; set; }
    
    [Key(1)] 
    public int Duration { get; set; }
    
    [Key(2)] 
    public int Id { get; set; }
    
    [Key(3)]
    public object Data { get; set; }
    
    [Key(4)]
    public string AffectedElement { get; set; }
    
    [Key(5)]
    public bool IsVisible { get; set; }
}