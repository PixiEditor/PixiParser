using System.Collections.Generic;
using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public class KeyFrameGroup
{
    [Key(0)]
    public bool Enabled { get; set; }
    
    [Key(1)]
    public List<ElementKeyFrame> Children { get; }
    
    [Key(2)]
    public int NodeId { get; set; }
    
    public KeyFrameGroup()
    {
        Children = new List<ElementKeyFrame>();
    }
    
    [SerializationConstructor]
    internal KeyFrameGroup(bool enabled, List<ElementKeyFrame> children, int nodeId)
    {
        Enabled = enabled;
        Children = children;
        NodeId = nodeId;
    }
}