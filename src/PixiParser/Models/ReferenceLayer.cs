using System.Diagnostics;
using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
[DebuggerDisplay("{ImageWidth}x{ImageHeight}")]
public class ReferenceLayer
{
    [Key(0)]
    public bool Enabled { get; set; }
    
    [Key(1)]
    public bool Topmost { get; set; }

    [Key(2)]
    public Corners Corners { get; set; }
    
    [Key(3)]
    public int ImageWidth { get; set; }
    
    [Key(4)]
    public int ImageHeight { get; set; }
    
    [Key(5)]
    public byte[] ImageBytes { get; set; }
}