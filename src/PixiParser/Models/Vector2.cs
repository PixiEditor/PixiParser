using System;
using MessagePack;

namespace PixiEditor.Parser;

[MessagePackObject]
public struct Vector2 : IEquatable<Vector2>
{
    [Key(0)]
    public double X { set; get; }
    
    [Key(1)]
    public double Y { set; get; }

    public bool Equals(Vector2 other) => X.Equals(other.X) && Y.Equals(other.Y);

    public override bool Equals(object obj) => obj is Vector2 other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);
}