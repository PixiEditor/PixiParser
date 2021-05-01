using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PixiEditor.Parser
{
    [Serializable]
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
        private byte[] SwatchesData { get => Helpers.SwatchesToBytes(Swatches); set => Swatches = new List<(byte, byte, byte, byte)>(Helpers.BytesToSwatches(value)); }

        [IgnoreDataMember]
        public List<(byte, byte, byte, byte)> Swatches { get; set; } = new List<(byte, byte, byte, byte)>();

        [DataMember(Order = 3)]
        public SerializableLayer[] Layers { get; set; }

        public void AddSwatch(byte a, byte r, byte g, byte b)
        {
            Swatches.Add((a, r, g, b));
        }

        public void AddSwatch(byte r, byte g, byte b) => AddSwatch(255, r, g, b);

        public IEnumerator<SerializableLayer> GetEnumerator()
        {
            foreach (SerializableLayer layer in Layers)
            {
                yield return layer;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Layers.GetEnumerator();
        }
    }
}