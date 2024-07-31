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
        var data = reader.ReadBytes();

        if (data == null)
        {
            return null;
        }
        
        return ColorCollection.FromByteSequence(data.Value);
    }
}