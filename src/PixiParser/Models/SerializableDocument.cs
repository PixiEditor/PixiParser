using PixiEditor.Parser.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace PixiEditor.Parser
{
    [DataContract]
    public class SerializableDocument : IEnumerable<SerializableLayer>
    {
        [DataMember(Order = 4)]
        public Version FileVersion { get; private set; } = new Version(1, 1);

        [DataMember(Order = 0)]
        public int Width { get; set; }

        [DataMember(Order = 1)]
        public int Height { get; set; }

        [DataMember(Order = 2)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Message Pack")]
        private byte[] SwatchesData { get => Helpers.SwatchesToBytes(Swatches); set => Swatches = Helpers.BytesToSwatches(value); }

        [IgnoreDataMember]
        public List<Color> Swatches { get; set; } = new List<Color>();

        [DataMember(Order = 3)]
        public List<SerializableLayer> Layers { get; set; } = new List<SerializableLayer>();

        [DataMember(Order = 5)]
        public SerializableGuidStructureItem[] Groups { get; set; }

        public SerializableDocument() { }

        public SerializableDocument(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public SerializableDocument(int width, int height, SerializableGuidStructureItem[] groups, params SerializableLayer[] layers)
            : this(width, height)
        {
            Layers = new List<SerializableLayer>(layers);
            Groups = groups;
        }

        public void AddSwatch(byte a, byte r, byte g, byte b) => Swatches.Add(Color.FromArgb(a, r, g, b));

        public void AddSwatch(byte r, byte g, byte b) => AddSwatch(255, r, g, b);

        public IEnumerator<SerializableLayer> GetEnumerator() => Layers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Layers.GetEnumerator();
    }
}