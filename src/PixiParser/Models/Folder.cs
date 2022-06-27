using System.Collections;
using System.Collections.Generic;
using PixiEditor.Parser.Helpers;

namespace PixiEditor.Parser;

public class Folder : List<IStructureMember>, IChildrenContainer, IMaskable
{
    private float _opacity = 1;
    
    public Folder AddFolder(IEnumerable<IStructureMember> children)
    {
        var folder = new Folder(children);
        Add(folder);
        return folder;
    }

    public Folder() { }

    public Folder(IEnumerable<IStructureMember> children)
        : base(children)
    {
    }

    public byte[] MaskBytes { get; set; }
    
    public bool Enabled { get; set; }
    

    public float Opacity
    {
        get => _opacity;
        set => this.SetOpacity(ref _opacity, value);
    }
}