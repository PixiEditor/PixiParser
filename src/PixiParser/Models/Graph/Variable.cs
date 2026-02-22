using MessagePack;

namespace PixiEditor.Parser.Graph;

[MessagePackObject]
public class Variable
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public object Value { get; set; }

    [Key(2)]
    public string? Unit { get; set; }

    [Key(3)]
    public double? Min { get; set; }

    [Key(4)]
    public double? Max { get; set; }

    [Key(5)]
    public bool IsExposed { get; set; } = true;

    [Key(6)]
    public string Type { get; set; }
}
