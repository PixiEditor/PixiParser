using System.Drawing;
using MessagePack;
using MessagePack.Formatters;
using PixiEditor.Parser.Collections;

namespace PixiEditor.Parser;

internal class ColorCollectionFormatter : IMessagePackFormatter<ColorCollection>
{
    public void Serialize(ref MessagePackWriter writer, ColorCollection value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }
        
        writer.WriteMapHeader(value.Count);
        
        foreach (var color in value)
        {
            writer.Write(color.A);
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
        }
    }

    public ColorCollection Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        int count = reader.ReadMapHeader();
        var colors = new ColorCollection(count);
        
        for (int i = 0; i < count; i++)
        {
            byte a = reader.ReadByte();
            byte r = reader.ReadByte();
            byte g = reader.ReadByte();
            byte b = reader.ReadByte();
            colors.Add(Color.FromArgb(a, r, g, b));
        }

        return colors;
    }
}