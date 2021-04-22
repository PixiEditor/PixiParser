using MessagePack;
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
        public Version FileVersion { get; set; } = new Version(1, 1);

        [DataMember(Order = 0)]
        public int Width { get; set; }

        [DataMember(Order = 1)]
        public int Height { get; set; }

        [DataMember(Order = 2)]
        private byte[] SwatchesData { get; set; }

        [IgnoreDataMember]
        public Tuple<byte, byte, byte, byte>[] Swatches { get => Helpers.BytesToSwatches(SwatchesData); set => SwatchesData = Helpers.SwatchesToBytes(value); }

        [DataMember(Order = 3)]
        public SerializableLayer[] Layers { get; set; }

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