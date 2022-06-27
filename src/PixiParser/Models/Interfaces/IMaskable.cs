namespace PixiEditor.Parser;

/// <summary>
/// A maskable structure member
/// </summary>
public interface IMaskable : IStructureMember
{
    /// <summary>
    /// A byte array containing the bytes for the mask png
    /// </summary>
    public byte[] MaskBytes { get; set; }
}