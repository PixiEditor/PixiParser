using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MessagePack;
using PixiEditor.Parser.Old.PixiV4.Helpers;
using PixiEditor.Parser.Old.PixiV4.Interfaces;

namespace PixiEditor.Parser.Old.PixiV4;

[MessagePackObject]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Folder : IName, IStructureChildrenContainer, IBlendMode, IMaskable, IStructureOpacity, IClipToLayerBelow
{
    [IgnoreMember]
    private float _opacity = 1;
    
    [IgnoreMember]
    private string DebuggerDisplay => $"'{Name}', {this.GetChildrenRecursive().Count()} members";

    public Folder AddFolder(IEnumerable<IStructureMember> children)
    {
        var folder = new Folder(children);
        Children.Add(folder);
        return folder;
    }

    public Folder()
    {
        Children = new();
    }

    public Folder(IEnumerable<IStructureMember> children)
    {
        Children = new(children);
    }

    [SerializationConstructor]
    internal Folder(string name, bool enabled, float opacity, Mask mask, List<IStructureMember> children)
    {
        Name = name;
        Enabled = enabled;
        Opacity = opacity;
        Mask = mask;
        Children = children;
    }
    
    [Key(0)]
    public string Name { get; set; }
    
    [Key(1)]
    public bool Enabled { get; set; }
    
    [Key(2)]
    public float Opacity
    {
        get => _opacity;
        set => this.SetOpacity(ref _opacity, value);
    }

    [Key(3)]
    public Mask Mask { get; set; }

    [Key(4)]
    public List<IStructureMember> Children { get; }

    [Key(5)]
    public BlendMode BlendMode { get; set; }

    [Key(6)]
    public bool ClipToMemberBelow { get; set; }
}