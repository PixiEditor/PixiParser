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
        
        writer.Write(value.ToByteArray());
    }

    public ColorCollection Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var data = reader.ReadRaw();
        
        return ColorCollection.FromByteSequence(data.Slice(2));
    }
}