using System;
using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public class RasterKeyFrame : ITimeSpan, IGuid, ILayerGuid, IKeyFrame
{
    [Key(0)]
    public int StartFrame { get; set; }
    [Key(1)]
    public int Duration { get; set; }
    [Key(2)]
    public Guid Guid { get; set; }
    [Key(3)]
    public Guid NodeId { get; set; }
}