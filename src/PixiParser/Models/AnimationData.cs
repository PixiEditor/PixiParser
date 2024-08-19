using System.Collections.Generic;
using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public sealed class AnimationData
{
    [Key(0)]
    public List<KeyFrameGroup> KeyFrameGroups { get; set; }
    
    [Key(1)]
    public int FrameRate { get; set; }
    
    [Key(2)]
    public int OnionFrames { get; set; }
}