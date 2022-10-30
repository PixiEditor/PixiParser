using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace PixiEditor.Parser.Deprecated;

[DataContract]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SerializableLayer
{
    private float opacity;

    /// <summary>
    /// The width of the layer
    /// </summary>
    [DataMember(Order = 0)]
    public int Width { get; set; }

    /// <summary>
    /// The height of the layer
    /// </summary>
    [DataMember(Order = 1)]
    public int Height { get; set; }

    /// <summary>
    /// The X offset of the layer
    /// </summary>
    [DataMember(Order = 2)]
    public int OffsetX { get; set; }

    /// <summary>
    /// The Y offset of the layer
    /// </summary>
    [DataMember(Order = 3)]
    public int OffsetY { get; set; }

    /// <summary>
    /// The name of the layer
    /// </summary>
    [DataMember(Order = 4)]
    public string Name { get; set; }

    /// <summary>
    /// Shoud the layer be visible
    /// </summary>
    [DataMember(Order = 5)]
    public bool IsVisible { get; set; }

    /// <summary>
    /// The opacity of the layer
    /// </summary>
    [DataMember(Order = 6)]
    public float Opacity
    {
        get => opacity;
        set
        {
            opacity = value switch
            {
                > 1f => 1f,
                < 0f => 0f,
                _ => value,
            };
        }
    }

    /// <summary>
    /// The png data of the layer
    /// </summary>
    [DataMember(Order = 7)]
    public byte[] PngBytes { get; set; }

    /// <summary>
    /// Creates a new <see cref="SerializableLayer"/> that is set to visible and has a <see cref="Opacity"/> of 1
    /// </summary>
    public SerializableLayer()
    {
        IsVisible = true;
        Opacity = 1;
    }

    /// <summary>
    /// Creates a new <see cref="SerializableLayer"/> that is set to visible and has a <see cref="Opacity"/> of 1
    /// </summary>
    public SerializableLayer(int width, int height)
        : this()
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Creates a new <see cref="SerializableLayer"/> that is set to visible and has a <see cref="Opacity"/> of 1
    /// </summary>
    public SerializableLayer(int width, int height, int offsetX, int offsetY)
        : this(width, height)
    {
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    public override bool Equals(object obj)
    {
        if (obj is not SerializableLayer layer)
        {
            return false;
        }

        return Equals(layer);
    }

    public override int GetHashCode()
    {
        HashCode hashCode = default;
        hashCode.Add(Name);
        hashCode.Add(Width);
        hashCode.Add(Height);
        hashCode.Add(PngBytes);
        hashCode.Add(IsVisible);
        hashCode.Add(OffsetX);
        hashCode.Add(OffsetY);
        hashCode.Add(Opacity);
        return hashCode.ToHashCode();
    }

    protected bool Equals(SerializableLayer other)
    {
        return Name == other.Name && Width == other.Width && Height == other.Height && (PngBytes == null && other.PngBytes == null || PngBytes.SequenceEqual(other.PngBytes)) &&
               IsVisible == other.IsVisible && OffsetX == other.OffsetX && OffsetY == other.OffsetY && Opacity.Equals(other.Opacity);
    }

    private string DebuggerDisplay => $"'{Name}' {Width}x{Height}";
}
