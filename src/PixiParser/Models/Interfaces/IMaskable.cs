using PixiEditor.Parser.Deprecated;

namespace PixiEditor.Parser;

/// <summary>
/// A maskable structure member
/// </summary>
public interface IMaskable : IStructureMember
{
    public Mask Mask { get; set; }
}