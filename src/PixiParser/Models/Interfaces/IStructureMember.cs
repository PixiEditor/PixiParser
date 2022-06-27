namespace PixiEditor.Parser;

public interface IStructureMember
{
    /// <summary>
    /// Is this structure member enabled/visible
    /// </summary>
    public bool Enabled { get; set; }
    
    /// <summary>
    /// The opacity of this structure member. Must be a value between 0 and 1. Initial value is 1
    /// </summary>
    public float Opacity { get; set; }
}