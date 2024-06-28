using System;
using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public class RasterKeyFrame : ITimeSpan, IGuid, ILayerGuid, IImageContainer, IKeyFrame
{
    [Key(0)]
    public int StartFrame { get; set; }
    [Key(1)]
    public int Duration { get; set; }
    [Key(2)]
    public Guid Guid { get; set; }
    [Key(3)]
    public Guid LayerGuid { get; set; }

    [IgnoreMember]
    public byte[] ImageBytes { get; set; }
    [Key(5)] int IImageContainer.ResourceOffset { get; set; }
    [Key(6)] int IImageContainer.ResourceSize { get; set; }
}