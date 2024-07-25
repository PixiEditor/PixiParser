using MessagePack;

namespace PixiEditor.Parser.Graph;

[MessagePackObject]
public class PropertyConnection
{
    [Key(0)]
    public int OutputNodeId { get; set; }

    [Key(1)] 
    public string InputPropertyName { get; set; }
    
    [Key(2)]
    public string OutputPropertyName { get; set; }
}