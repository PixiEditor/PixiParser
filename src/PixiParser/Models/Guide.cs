using System;
using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public class Guide
{
    [Key(0)]
    public bool IsVertical { get; set; }

    [Key(1)]
    public double Offset { get; set; }
}
