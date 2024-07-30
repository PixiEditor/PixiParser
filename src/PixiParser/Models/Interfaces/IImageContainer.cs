namespace PixiEditor.Parser;

/// <summary>
/// A member that has a bitmap
/// </summary>
public interface IImageContainer : IStructureMember
{
    /// <summary>
    /// A byte array containing the bytes for the png image
    /// </summary>
    public byte[] ImageBytes { get; set; }
    
    internal int ResourceOffset { get; set; }

    internal int ResourceSize { get; set; }
}