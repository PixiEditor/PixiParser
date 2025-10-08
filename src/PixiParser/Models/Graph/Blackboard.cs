using System.Collections.Generic;
using MessagePack;

namespace PixiEditor.Parser.Graph;

[MessagePackObject]
public class Blackboard
{
    [Key(0)]
    public List<Variable> Variables { get; set; } = new();
}
