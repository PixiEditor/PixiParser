﻿using System;
using System.Diagnostics;
using System.Linq;
using MessagePack;
using PixiEditor.Parser.Collections;
using PixiEditor.Parser.Helpers;

namespace PixiEditor.Parser;

[MessagePackObject]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Document
{
    [IgnoreMember]
    private string DebuggerDisplay => $"{Width}x{Height}, {RootFolder.GetChildrenRecursive().Count()} members";
    
    [IgnoreMember]
    private ColorCollection swatches;
    [IgnoreMember]
    private ColorCollection palette;
    
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
    
    [IgnoreMember]
    public byte[] PreviewImage { get; set; }
    
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
    
    [Key(4)]
    public Folder RootFolder { get; set; }
    
    [Key(5)]
    public ReferenceLayer ReferenceLayer { get; set; }
    
    private ColorCollection GetColorCollection(ref ColorCollection variable)
    {
        if (variable is null)
        {
            return variable = new();
        }

        return variable;
    }
}