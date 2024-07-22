using System.Collections.Generic;
using MessagePack;

namespace PixiEditor.Parser.Graph;

[MessagePackObject]
public class NodeGraph
{
    [Key(0)]
    public List<Node> AllNodes { get; set; }
}