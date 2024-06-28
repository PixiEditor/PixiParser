namespace PixiEditor.Parser;

public interface IStructureOpacity : IStructureMember
{
    /// <summary>
    /// The opacity of this structure member. Must be a value between 0 and 1. Initial value is 1
    /// </summary>
    public float Opacity { get; set; }
}