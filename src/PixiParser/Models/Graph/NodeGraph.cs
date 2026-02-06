using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace PixiEditor.Parser.Graph;

[MessagePackObject]
public class NodeGraph
{
    [Key(0)] public List<Node> AllNodes { get; set; }
    [Key(1)] public Blackboard Blackboard { get; set; }
}
