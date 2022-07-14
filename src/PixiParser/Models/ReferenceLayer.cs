using System;
using System.Diagnostics;
using MessagePack;
using PixiEditor.Parser.Helpers;

namespace PixiEditor.Parser;

[MessagePackObject]
[DebuggerDisplay("'{Name,nq}' {Width}x{Height}")]
public class ReferenceLayer : IImageContainer, IName, ISize<float>, IOpacity, IGuid
{
    [IgnoreMember]
    private float _opacity = 1;
    
    [Key(0)]
    public string Name { get; set; }
    
    [Key(1)]
    public bool Enabled { get; set; }
    
    [IgnoreMember]
    public byte[] ImageBytes { get; set; }

    [Key(2)]
    public float Width { get; set; }
    
    [Key(3)]
    public float Height { get; set; }

    [Key(4)]
    public float OffsetX { get; set; }
    
    [Key(5)]
    public float OffsetY { get; set; }

    [Key(6)]
    public float Opacity
    {
        get => _opacity;
        set => this.SetOpacity(ref _opacity, value);
    }
    
    [Key(7)]
    public Guid Guid { get; set; }
    
    [Key(8)]
    int IImageContainer.ResourceOffset { get; set; }
    
    [Key(9)]
    int IImageContainer.ResourceSize { get; set; }
}