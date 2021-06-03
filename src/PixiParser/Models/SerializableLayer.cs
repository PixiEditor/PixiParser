using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace PixiEditor.Parser
{
    [Serializable]
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

        [IgnoreDataMember]
        public byte[] BitmapBytes { get; set; }

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

        public Bitmap ToBitmap()
        {
            return new Bitmap(Width, Height, Width * 4,
                     PixelFormat.Format32bppArgb,
                     Marshal.UnsafeAddrOfPinnedArrayElement(BitmapBytes, 0));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(SerializableLayer))
            {
                return false;
            }

            SerializableLayer layer = (SerializableLayer)obj;

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
            hashCode.Add(BitmapBytes);
            hashCode.Add(IsVisible);
            hashCode.Add(OffsetX);
            hashCode.Add(OffsetY);
            hashCode.Add(Opacity);
            return hashCode.ToHashCode();
        }

        protected bool Equals(SerializableLayer other)
        {
            return Name == other.Name && Width == other.Width && Height == other.Height && MaxWidth == other.MaxWidth && MaxHeight == other.MaxHeight &&
                   BitmapBytes.SequenceEqual(other.BitmapBytes) && IsVisible == other.IsVisible && OffsetX == other.OffsetX && OffsetY == other.OffsetY && Opacity.Equals(other.Opacity);
        }
    }
}