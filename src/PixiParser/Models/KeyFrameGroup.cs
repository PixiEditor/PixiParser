using System;
using System.Collections.Generic;
using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public class KeyFrameGroup : ILayerGuid, IKeyFrameChildrenContainer, IKeyFrame
{
    [Key(0)]
    public bool Enabled { get; set; }
    
    [Key(1)]
    public List<IKeyFrame> Children { get; }
    
    [Key(2)]
    public Guid LayerGuid { get; set; }
    
    public KeyFrameGroup()
    {
        Children = new List<IKeyFrame>();
    }
    
    [SerializationConstructor]
    internal KeyFrameGroup(bool enabled, List<IKeyFrame> children, Guid layerGuid)
    {
        Enabled = enabled;
        Children = children;
        LayerGuid = layerGuid;
    }
}