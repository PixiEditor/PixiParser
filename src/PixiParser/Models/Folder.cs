﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MessagePack;
using PixiEditor.Parser.Helpers;

namespace PixiEditor.Parser;

[MessagePackObject]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Folder : IName, IChildrenContainer, IMaskable
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
}