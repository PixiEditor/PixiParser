namespace PixiEditor.Parser;

/// <summary>
/// A structure member that has a bitmap
/// </summary>
public interface IImageContainer : IStructureMember
{
    /// <summary>
    /// A byte array containing the bytes for the png image
    /// </summary>
    public byte[] ImageBytes { get; set; }
}