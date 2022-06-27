using System;
using PixiEditor.Parser.Collections;

namespace PixiEditor.Parser;

public class Document
{
    private ColorCollection swatches;
    private ColorCollection palette;
    
    /// <summary>
    /// The .pixi version of this document
    /// </summary>
    public Version Version { get; internal set; }
    
    /// <summary>
    /// The width of the doucment
    /// </summary>
    public uint Width { get; set; }
    
    /// <summary>
    /// The height of the document
    /// </summary>
    public uint Height { get; set; }
    
    public ColorCollection Swatches
    {
        get => GetColorCollection(ref swatches);
#if NET5_0_OR_GREATER
        init => swatches = value;
#endif
    }

    public ColorCollection Palette
    {
        get => GetColorCollection(ref palette);
#if NET5_0_OR_GREATER
        init => palette = value;
#endif
    }
    
    public Folder RootFolder { get; }

    private ColorCollection GetColorCollection(ref ColorCollection variable)
    {
        if (variable is null)
        {
            return variable = new();
        }

        return palette;
    }
}