using MessagePack;
using PixiParser.Parser;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixiEditor.Parser
{
    [Serializable]
    [MessagePackObject]
    public class SerializableDocument : IEnumerable<SerializableLayer>
    {
        [Key(0)]
        public int Width { get; set; }

        [Key(1)]
        public int Height { get; set; }

        [Key(2)]
        public byte[] SwatchesData { get; set; }

        [IgnoreMember]
        public Tuple<byte, byte, byte, byte>[] Swatches { get; set; }

        [Key(3)]
        public SerializableLayer[] Layers { get; set; }

        [Key(4)]
        public SerializableGuidStructureItem[] Groups { get; set; }

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