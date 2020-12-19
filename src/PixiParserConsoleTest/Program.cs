using PixiEditor.Parser;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PixiParserConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            formatter.Binder = new CurrentAssemblyDeserializationBinder();

            using FileStream stream = new FileStream("./test.pixi", FileMode.Open);

            SerializableDocument document = (SerializableDocument)formatter.Deserialize(stream);

            byte[] bytes = PixiParser.Serialize(document);

            SerializableDocument document2 = PixiParser.Deserialize(bytes);

            using FileStream saveStream = new FileStream("./final.pixi", FileMode.OpenOrCreate);

            formatter.Serialize(saveStream, document2);

            Console.WriteLine(document.Equals(document2));

            Console.WriteLine(document.Layers[0].BitmapBytes.GetHashCode());
            Console.WriteLine(document2.Layers[0].BitmapBytes.GetHashCode());

            SerializableLayer layer0 = document.Layers[0];
            SerializableLayer layer1 = document2.Layers[0];

            for (int i = 0; i < document.Layers[0].BitmapBytes.Length; i++)
            {
                if (layer0.BitmapBytes[i] != layer1.BitmapBytes[i])
                {
                    Console.WriteLine(i);
                }
            }

            Console.WriteLine();
        }

        public sealed class CurrentAssemblyDeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                if (typeName.Contains("SerializableDocument"))
                {
                    return typeof(SerializableDocument);
                }
                else if (typeName.Contains("SerializableLayer"))
                {
                    return typeof(SerializableLayer);
                }

                return Type.GetType(string.Format("{0}, {1}", typeName, Assembly.GetAssembly(typeof(SerializableDocument)).FullName));
            }
        }
    }
}
