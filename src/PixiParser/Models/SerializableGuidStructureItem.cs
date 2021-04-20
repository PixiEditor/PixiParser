using MessagePack;
using System;

namespace PixiParser.Parser
{
    [Serializable]
    public class SerializableGuidStructureItem
    {
        [Key(1)]
        public Guid GroupGuid { get; set; }
        
        [Key(2)]
        public string Name { get; set; }

        [Key(3)]
        public Guid StartLayerGuid { get; set; }

        [Key(4)]
        public Guid EndLayerGuid { get; set; }

        [Key(5)]
        public SerializableGuidStructureItem Parent { get; set; }

        [Key(6)]
        public SerializableGuidStructureItem[] Subgroups { get; set; }

        public SerializableGuidStructureItem(Guid groupGuid, string name, Guid startLayerGuid, Guid endLayerGuid, SerializableGuidStructureItem[] subgroups, SerializableGuidStructureItem parent)
        {
            GroupGuid = groupGuid;
            Name = name;
            StartLayerGuid = startLayerGuid;
            EndLayerGuid = endLayerGuid;
            Subgroups = subgroups;
            Parent = parent;
        }
    }
}