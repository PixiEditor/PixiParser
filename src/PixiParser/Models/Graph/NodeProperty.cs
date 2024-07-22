using MessagePack;

namespace PixiEditor.Parser.Graph;

[MessagePackObject]
public class NodePropertyValue
{
    [Key(0)]
    public string PropertyName { get; set; }
    
    [Key(1)]
    public object Value { get; set; }
}