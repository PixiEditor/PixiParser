using System.Collections.Generic;
using MessagePack;

namespace PixiEditor.Parser.Graph;

[MessagePackObject]
public class Node
{
    [Key(0)]
    public int Id { get; set; }
    
    [Key(1)]
    public string Name { get; set; }
    
    [Key(2)]
    public string UniqueNodeName { get; set; }
    
    [Key(3)]
    public Vector2 Position { get; set; }
    
    [Key(4)]
    public NodePropertyValue[] InputPropertyValues { get; set; } 
    
    [Key(5)]
    public PropertyConnection[] InputConnections { get; set; }
    
    [Key(6)]
    public KeyFrameData[] KeyFrames { get; set; }
    
    [Key(7)]
    public Dictionary<string, object> AdditionalData { get; set; }
}