using MessagePack;

namespace PixiEditor.Parser.Graph;

[MessagePackObject]
public class Variable
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public object Value { get; set; }
}
