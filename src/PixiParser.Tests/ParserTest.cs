using System;
using System.IO;
using Xunit;

namespace PixiEditor.Parser.Tests
{
    public class ParserTest
    {
        [Fact]
        public void SerializingAndDeserialzingWorks()
        {
            SerializableDocument document = new SerializableDocument();
            document.Height = 1;
            document.Width = 1;
            document.Swatches = new Tuple<byte, byte, byte, byte>[] { new Tuple<byte, byte, byte, byte>(255, 255, 255, 255) };

            byte[] imageData = new byte[] { 255, 255, 255, 255 };

            document.Layers = new SerializableLayer[] { new SerializableLayer() {
                Width = 1, Height = 1,
                MaxHeight = 1, MaxWidth = 1,
                BitmapBytes = imageData,
                IsVisible = true, Name = "Base Layer",
                OffsetX = 0, OffsetY = 0,
                Opacity = 1 } };

            byte[] serialized = PixiParser.Serialize(document);

            SerializableDocument deserializedDocument = PixiParser.Deserialize(serialized);

            Assert.Equal(document.Height, deserializedDocument.Height);
            Assert.Equal(document.Width, deserializedDocument.Width);
            Assert.Equal(document.Swatches.Length, deserializedDocument.Swatches.Length);

            Assert.Equal(document.Layers[0].BitmapBytes, deserializedDocument.Layers[0].BitmapBytes);
        }

        [Fact]
        public void DetectOldFile()
        {
            Assert.Throws<OldFileFormatException>(delegate { PixiParser.Deserialize("./OldPixiFile.pixi"); });
        }

        [Fact]
        public void ParsesOldFile()
        {
            using FileStream stream = new FileStream("./OldPixiFile.pixi", FileMode.Open, FileAccess.Read);

            PixiParser.DeserializeOld(stream);
        }

        [Fact]
        public void DetectCorruptedFile()
        {
            Assert.Throws<InvalidFileException>(delegate { PixiParser.Deserialize("./CorruptedPixiFile.pixi"); });
        }
    }
}