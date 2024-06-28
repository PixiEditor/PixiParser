using MessagePack;

namespace PixiEditor.Parser;

[Union(0, typeof(Folder))]
[Union(1, typeof(ImageLayer))]
[Union(2, typeof(ReferenceLayer))]
public interface IStructureMember 
{
    /// <summary>
    /// Is this structure member enabled/visible
    /// </summary>
    public bool Enabled { get; set; }
}