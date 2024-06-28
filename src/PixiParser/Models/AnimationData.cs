using System.Collections.Generic;
using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public sealed class AnimationData
{
    [Key(0)]
    public List<KeyFrameGroup> KeyFrameGroups { get; set; }
}