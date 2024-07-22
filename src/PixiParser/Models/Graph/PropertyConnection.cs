using MessagePack;

namespace PixiEditor.Parser.Graph;

[MessagePackObject]
public class PropertyConnection
{
    [Key(0)]
    public int NodeId { get; set; }

    [Key(1)] 
    public string PropertyName { get; set; }
}