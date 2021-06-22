using MessagePack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PixiEditor.Parser.Models
{
    [DataContract]
    public class SerializableGuidStructureItem
    {
        [DataMember(Order = 1)]
        public Guid GroupGuid { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public Guid StartLayerGuid { get; set; }

        [DataMember(Order = 4)]
        public Guid EndLayerGuid { get; set; }

        [DataMember(Order = 6)]
        public SerializableGuidStructureItem[] Subgroups { get; set; }

        [DataMember(Order = 7)]
        public bool IsVisible { get; set; }

        [DataMember(Order = 8)]
        public float Opacity { get; set; }

        public SerializableGuidStructureItem()
        {
            IsVisible = true;
            Opacity = 1;
        }

        public SerializableGuidStructureItem(Guid groupGuid, string name, Guid startLayerGuid, Guid endLayerGuid, SerializableGuidStructureItem[] subgroups, bool isVisible, float opacity)
        {
            GroupGuid = groupGuid;
            Name = name;
            StartLayerGuid = startLayerGuid;
            EndLayerGuid = endLayerGuid;
            Subgroups = subgroups;
            IsVisible = isVisible;
            Opacity = opacity;
        }
    }
}
