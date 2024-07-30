namespace PixiEditor.Parser.Old.PixiV4.Interfaces;

/// <summary>
/// A maskable structure member
/// </summary>
public interface IMaskable : IStructureMember
{
    public Mask Mask { get; set; }
}