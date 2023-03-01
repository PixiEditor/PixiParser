using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public class Corners
{
    [Key(0)]
    public Vector2 TopLeft { get; set; }
    [Key(1)]
    public Vector2 TopRight { get; set; }
    [Key(2)]
    public Vector2 BottomLeft { get; set; }
    [Key(3)]
    public Vector2 BottomRight { get; set; }
}