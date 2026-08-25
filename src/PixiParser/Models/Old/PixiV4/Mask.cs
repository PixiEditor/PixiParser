using System.Diagnostics;
using MessagePack;

namespace PixiEditor.Parser.Old.PixiV4;

[MessagePackObject(AllowPrivate = true)]
[DebuggerDisplay("{Width}x{Height}")]
public partial class Mask : IImageContainer, ISize<int>, IStructureMember
{
    [Key(0)]
    public bool Enabled { get; set; }

    [Key(1)]
    public int Width { get; set; }
    
    [Key(2)]
    public int Height { get; set; }

    [Key(3)]
    public int OffsetX { get; set; }
    
    [Key(4)]
    public int OffsetY { get; set; }
    
    [IgnoreMember]
    public byte[] ImageBytes { get; set; }

    [Key(5)]
    public BlendMode BlendMode { get; set; }

    [Key(6)] public int ResourceOffset { get; set; }
    
    [Key(7)] public int ResourceSize { get; set; }
}
