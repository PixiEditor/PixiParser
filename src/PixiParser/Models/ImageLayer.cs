using System;
using System.Diagnostics;
using MessagePack;
using PixiEditor.Parser.Helpers;

namespace PixiEditor.Parser;

[MessagePackObject]
[DebuggerDisplay("'{Name, nq}' {Width}x{Height}")]
public sealed class ImageLayer : IImageContainer, IBlendMode, IName, IMaskable, IStructureOpacity, ISize<int>, IClipToLayerBelow, IStructureMember
{
    [IgnoreMember]
    private float _opacity = 1;

    [Key(0)]
    public string Name { get; set; }
    
    [Key(1)]
    public bool Enabled { get; set; }

    [Key(2)]
    public int Width { get; set; }
    
    [Key(3)]
    public int Height { get; set; }
    
    [Key(4)]
    public int OffsetX { get; set; }
    
    [Key(5)]
    public int OffsetY { get; set; }

    [IgnoreMember]
    public byte[] ImageBytes { get; set; }

    [Key(6)]
    public BlendMode BlendMode { get; set; }

    [Key(7)]
    public Mask Mask { get; set; }

    [Key(8)]
    public float Opacity
    {
        get => _opacity;
        set => this.SetOpacity(ref _opacity, value);
    }
    
    [Key(9)]
    int IImageContainer.ResourceOffset { get; set; }

    [Key(10)]
    int IImageContainer.ResourceSize { get; set; }
    
    [Key(11)]
    public bool ClipToMemberBelow { get; set; }
    
    [Key(12)]
    public bool LockAlpha { get; set; }
    
    [Key(13)]
    public Guid Guid { get; set; }
}