namespace PixiEditor.Parser.Deprecated.Interfaces;

/// <summary>
/// A maskable structure member
/// </summary>
public interface IMaskable : IStructureMember
{
    public Mask Mask { get; set; }
}