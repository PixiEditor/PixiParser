using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace PixiEditor.Parser
{
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SerializableGroup
    {
        /// <summary>
        /// The name of the group
        /// </summary>
        [DataMember(Order = 0)]
        public string Name { get; set; }

        /// <summary>
        /// The subgroups of this group
        /// </summary>
        [DataMember(Order = 1)]
        public List<SerializableGroup> Subgroups { get; set; }

        /// <summary>
        /// Should all the layer in the group be visible
        /// </summary>
        [DataMember(Order = 2)]
        public bool IsVisible { get; set; }

        /// <summary>
        /// Opacity of the group
        /// </summary>
        [DataMember(Order = 3)]
        public float Opacity { get; set; }

        [DataMember(Order = 4)]
        internal int SerializedStartLayer { get; set; }

        [DataMember(Order = 5)]
        internal int SerializedEndLayer { get; set; }

        /// <summary>
        /// The index of the first layer in the group
        /// </summary>
        [IgnoreDataMember]
        public int StartLayer
        {
            get => SerializedStartLayer;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"value must be between 0 and {nameof(int.MaxValue)}");
                }

                SerializedEndLayer = Math.Max(SerializedEndLayer, value);
                SerializedStartLayer = value;
            }
        }

        /// <summary>
        /// The index of the last layer in the group
        /// </summary>
        [IgnoreDataMember]
        public int EndLayer
        {
            get => SerializedEndLayer;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"value must be between 0 and {nameof(int.MaxValue)}");
                }

                SerializedStartLayer = Math.Min(SerializedStartLayer, value);
                SerializedEndLayer = value;
            }
        }

        public SerializableGroup()
        {
            Subgroups = new List<SerializableGroup>();
            IsVisible = true;
            Opacity = 1;
            SerializedStartLayer = -1;
            SerializedEndLayer = -1;
        }

        public SerializableGroup(string name)
            : this()
        {
            Name = name;
        }

        public SerializableGroup(string name, IEnumerable<SerializableGroup> subgroups)
            : this()
        {
            Name = name;
            Subgroups = new List<SerializableGroup>(subgroups);
        }

        public SerializableGroup(string name, int startLayerIndex, int endLayerIndex, IEnumerable<SerializableGroup> subgroups, bool isVisible = true, float opacity = 1)
            : this(name)
        {
            if (startLayerIndex > endLayerIndex)
            {
                throw new ArgumentException("The start layer index can't be lower then the end layer index", nameof(startLayerIndex));
            }

            Name = name;
            SerializedStartLayer = startLayerIndex;
            SerializedEndLayer = endLayerIndex;
            Subgroups = new List<SerializableGroup>(subgroups);
            IsVisible = isVisible;
            Opacity = opacity;
        }

        /// <summary>
        /// Set's the values of <see cref="StartLayer"/> and <see cref="EndLayer"/> in this <em>group</em> and all <em>subgroups</em> to -1
        /// </summary>
        public void DetachLayers()
        {
            DetachLayers(this);
        }

        private static void DetachLayers(SerializableGroup group)
        {
            group.SerializedEndLayer = -1;
            group.SerializedStartLayer = -1;

            foreach (SerializableGroup subgroup in group.Subgroups)
            {
                DetachLayers(subgroup);
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Subgroups, IsVisible, Opacity, StartLayer, EndLayer);
        }

        public override bool Equals(object obj)
        {
            if (obj is not SerializableGroup group)
            {
                return false;
            }

            return Equals(group);
        }

        protected virtual bool Equals(SerializableGroup group)
        {
            return Name == group.Name && (Subgroups != null && group.Subgroups != null || Subgroups.SequenceEqual(group.Subgroups) &&
                   IsVisible == group.IsVisible && Opacity == group.Opacity && StartLayer == group.StartLayer && EndLayer == group.EndLayer);
        }

        private string DebuggerDisplay => $"'{Name}' Start: {StartLayer} End: {EndLayer}";
    }
}
