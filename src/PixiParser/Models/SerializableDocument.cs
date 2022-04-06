using PixiEditor.Parser.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace PixiEditor.Parser;

[DataContract]
public class SerializableDocument : IEnumerable<SerializableLayer>
{
    private SwatchCollection swatchCollection;
    private SwatchCollection palette;
    private LayerCollection layerCollection;

    /// <summary>
    /// Set to the <see cref="PixiParser.FileVersion"/> when serializing
    /// </summary>
    [DataMember(Order = 4)]
    public Version FileVersion { get; internal set; }

    /// <summary>
    /// The width of the document
    /// </summary>
    [DataMember(Order = 0)]
    public int Width { get; set; }

    /// <summary>
    /// The height of the document
    /// </summary>
    [DataMember(Order = 1)]
    public int Height { get; set; }

    /// <summary>
    /// A byte array containing all the swatches
    /// </summary>
    [DataMember(Order = 2)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used in message pack")]
    private byte[] SwatchesData { get => GetSwatchesBytes(swatchCollection); set => Swatches.FromByteArray(value); }

    /// <summary>
    /// A collection of swatches used in the document
    /// </summary>
    [IgnoreDataMember]
    public SwatchCollection Swatches
    {
        get
        {
            if (swatchCollection == null)
            {
                return swatchCollection = new SwatchCollection();
            }

            return swatchCollection;
        }
        internal set => swatchCollection = value;
    }

    [DataMember(Order = 3)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used in message pack")]
    private List<SerializableLayer> SerializedLayers
    {
        get
        {
            if (layerCollection == null)
            {
                return new List<SerializableLayer>(0);
            }

            return layerCollection.GetList();
        }
        set
        {
            Layers = new LayerCollection(this, value);
        }
    }

    /// <summary>
    /// The layers contained in the document
    /// </summary>
    [IgnoreDataMember]
    public LayerCollection Layers
    {
        get
        {
            if (layerCollection == null)
            {
                return layerCollection = new LayerCollection(this);
            }

            return layerCollection;
        }
        private set => layerCollection = value;
    }

    /// <summary>
    /// The layer groups
    /// </summary>
    [DataMember(Order = 5)]
    public List<SerializableGroup> Groups { get; set; }

    /// <summary>
    /// A byte array containing all the swatches
    /// </summary>
    [DataMember(Order = 6)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used in message pack")]
    private byte[] PaletteData { get => GetSwatchesBytes(Palette); set => Palette.FromByteArray(value); }

    /// <summary>
    /// A collection of colors used in the document palette
    /// </summary>
    [IgnoreDataMember]
    public SwatchCollection Palette
    {
        get
        {
            if (palette == null)
            {
                return palette = new SwatchCollection();
            }

            return palette;
        }
        internal set => palette = value;
    }

    /// <summary>
    /// Creates a new empty document
    /// </summary>
    public SerializableDocument()
    {
        Groups = new List<SerializableGroup>();
    }

    /// <summary>
    /// Creates a new empty document
    /// </summary>
    public SerializableDocument(int width, int height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Creates a new document, with the <paramref name="layers"/> as it's layers
    /// </summary>
    public SerializableDocument(int width, int height, params SerializableLayer[] layers)
        : this(width, height)
    {
        Layers = new LayerCollection(this, layers);
    }

    /// <summary>
    /// Creates a new document, with the <paramref name="groups"/> as it's groups and the <paramref name="layers"/> as it's layers
    /// </summary>
    public SerializableDocument(int width, int height, IEnumerable<SerializableGroup> groups, params SerializableLayer[] layers)
        : this(width, height)
    {
        Layers = new LayerCollection(this, layers);
        Groups = new List<SerializableGroup>(groups);
    }

    /// <summary>
    /// Creates a new document, with the <paramref name="layers"/> as it's layers
    /// </summary>
    public SerializableDocument(int width, int height, IEnumerable<SerializableLayer> layers)
        : this(width, height)
    {
        Layers = new LayerCollection(this, layers);
    }

    /// <summary>
    /// Creates a new document, with the <paramref name="groups"/> as it's groups and the <paramref name="layers"/> as it's layers
    /// </summary>
    public SerializableDocument(int width, int height, IEnumerable<SerializableGroup> groups, IEnumerable<SerializableLayer> layers)
        : this(width, height)
    {
        Layers = new LayerCollection(this, layers);
        Groups = new List<SerializableGroup>(groups);
    }

    public IEnumerator<SerializableLayer> GetEnumerator() => Layers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Layers.GetEnumerator();

    public override int GetHashCode()
    {
        return HashCode.Combine(FileVersion, Width, Height, swatchCollection, palette, layerCollection, Groups);
    }

    public override bool Equals(object obj)
    {
        if (obj is not SerializableDocument document)
        {
            return false;
        }

        return Equals(document);
    }

    protected virtual bool Equals(SerializableDocument document)
    {
        return FileVersion == document.FileVersion && Width == document.Width && Height == document.Height &&
               (swatchCollection == null && document.swatchCollection == null || swatchCollection.SequenceEqual(document.swatchCollection)) &&
               (layerCollection == null && document.layerCollection == null || layerCollection.SequenceEqual(document.layerCollection)) &&
               (Groups == null && document.Groups == null || Groups.SequenceEqual(document.Groups)
               && (palette == null && document.palette == null || palette.SequenceEqual(document.palette)));
    }

    private byte[] GetSwatchesBytes(SwatchCollection collection)
    {
        if (collection is null)
        {
            return Array.Empty<byte>();
        }

        return collection.ToByteArray();
    }
}
