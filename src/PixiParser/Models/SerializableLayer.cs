using MessagePack;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace PixiEditor.Parser
{
    [Serializable]
    [MessagePackObject]
    public class SerializableLayer
    {
        [Key(4)]
        public string Name { get; set; }

        [Key(0)]
        public int Width { get; set; }

        [Key(1)]
        public int Height { get; set; }

        [IgnoreMember]
        public int MaxWidth { get; set; }

        [IgnoreMember]
        public int MaxHeight { get; set; }

        [IgnoreMember]
        public byte[] BitmapBytes { get; set; }

        [Key(5)]
        public bool IsVisible { get; set; }

        [Key(2)]
        public int OffsetX { get; set; }

        [Key(3)]
        public int OffsetY { get; set; }

        [Key(6)]
        public float Opacity { get; set; }

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
            HashCode hashCode = default(HashCode);
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