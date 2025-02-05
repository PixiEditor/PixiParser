using System;
using System.Diagnostics;
using System.Linq;
using MessagePack;
using PixiEditor.Parser.Collections;
using PixiEditor.Parser.Graph;

namespace PixiEditor.Parser;

[MessagePackObject]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Document : IPixiDocument
{
    [IgnoreMember] private string DebuggerDisplay => $"{Width}x{Height}, {Graph.AllNodes.Count()} nodes";

    [IgnoreMember] private ColorCollection swatches;
    [IgnoreMember] private ColorCollection palette;

    /// <summary>
    /// The .pixi version of this document
    /// </summary>
    [IgnoreMember]
    public Version Version { get; internal set; }

    /// <summary>
    /// The minimum .pixi version required to parse this document
    /// </summary>
    [IgnoreMember]
    public Version MinVersion { get; internal set; }

    [IgnoreMember] public byte[] PreviewImage { get; set; }

    /// <summary>
    /// The width of the doucment
    /// </summary>
    [Key(0)]
    public int Width { get; set; }

    /// <summary>
    /// The height of the document
    /// </summary>
    [Key(1)]
    public int Height { get; set; }

    [Key(2)]
    internal ColorCollection SwatchesInternal
    {
        get => swatches;
        set => swatches = value;
    }

    [Key(3)]
    internal ColorCollection PaletteInternal
    {
        get => palette;
        set => palette = value;
    }

    [IgnoreMember]
    public ColorCollection Swatches
    {
        get => GetColorCollection(ref swatches);
#if NET5_0_OR_GREATER
        init => swatches = value;
#endif
    }

    [IgnoreMember]
    public ColorCollection Palette
    {
        get => GetColorCollection(ref palette);
#if NET5_0_OR_GREATER
        init => palette = value;
#endif
    }

    [Key(4)] public NodeGraph Graph { get; set; }

    [Key(5)] public ReferenceLayer ReferenceLayer { get; set; }

    [Key(6)] public AnimationData AnimationData { get; set; }

    [Key(7)] public string ImageEncoderUsed { get; set; } = "PNG";

    [Key(8)] public string SerializerName { get; set; }

    [Key(9)] public string SerializerVersion { get; set; }
    
    [Key(10)] public bool SrgbColorBlending { get; set; }

    [Key(11)] public ResourceStorage Resources { get; set; }

    private ColorCollection GetColorCollection(ref ColorCollection variable)
    {
        if (variable is null)
        {
            return variable = new();
        }

        return variable;
    }

    public IPixiParser GetParser() => PixiParser.V5;
}
