using System;
using System.Linq;
using System.Runtime.Serialization;

namespace PixiEditor.Parser
{
    [DataContract]
    public class SerializableLayer
    {
        [DataMember(Order = 4)]
        public string Name { get; set; }

        [DataMember(Order = 0)]
        public int Width { get; set; }

        [DataMember(Order = 1)]
        public int Height { get; set; }

        [IgnoreDataMember]
        public int MaxWidth { get; set; }

        [IgnoreDataMember]
        public int MaxHeight { get; set; }

        [DataMember(Order = 8)]
        public byte[] PngBytes { get; set; }

        [DataMember(Order = 5)]
        public bool IsVisible { get; set; }

        [DataMember(Order = 2)]
        public int OffsetX { get; set; }

        [DataMember(Order = 3)]
        public int OffsetY { get; set; }

        [DataMember(Order = 6)]
        public float Opacity { get; set; }

        [DataMember(Order = 7)]
        public Guid LayerGuid { get; set; }

        public SerializableLayer()
        {
            IsVisible = true;
            Opacity = 1;
        }

        public SerializableLayer(int width, int height)
            : this()
        {
            Width = width;
            Height = height;
        }

        public SerializableLayer(int width, int height, int offsetX, int offsetY)
            : this(width, height)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public override bool Equals(object obj)
        {
            if (obj is not SerializableLayer layer)
            {
                return false;
            }

            return Equals(layer);
        }

        public override int GetHashCode()
        {
            HashCode hashCode = default;
            hashCode.Add(Name);
            hashCode.Add(Width);
            hashCode.Add(Height);
            hashCode.Add(MaxWidth);
            hashCode.Add(MaxHeight);
            hashCode.Add(PngBytes);
            hashCode.Add(IsVisible);
            hashCode.Add(OffsetX);
            hashCode.Add(OffsetY);
            hashCode.Add(Opacity);
            return hashCode.ToHashCode();
        }

        protected bool Equals(SerializableLayer other)
        {
            return Name == other.Name && Width == other.Width && Height == other.Height && MaxWidth == other.MaxWidth && MaxHeight == other.MaxHeight &&
                   PngBytes.SequenceEqual(other.PngBytes) && IsVisible == other.IsVisible && OffsetX == other.OffsetX && OffsetY == other.OffsetY && Opacity.Equals(other.Opacity);
        }
    }
}