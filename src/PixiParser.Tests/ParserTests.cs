using System;
using System.Collections.Generic;
using Xunit;

namespace PixiEditor.Parser.Tests
{
    public class ParserTests
    {
        [Fact]
        public void SerializingAndDeserialzingWorks()
        {
            SerializableDocument document = new()
            {
                Height = 1,
                Width = 1
            };

            document.AddSwatch(255, 255, 255);

            byte[] imageData = new byte[] { 255, 255, 255, 255 };

            document.Layers = new List<SerializableLayer>
            {
                new SerializableLayer(1, 1)
                    {
                        MaxHeight = 1, MaxWidth = 1,
                        PngBytes = imageData,
                        IsVisible = true, Name = "Base Layer",
                        Opacity = 1
                }
            };

            var topGuid = Guid.NewGuid();
            var bottomGuid = Guid.NewGuid();

            document.Groups = new SerializableGuidStructureItem[]
            {
                new SerializableGuidStructureItem(Guid.NewGuid(), "Test name", bottomGuid, topGuid,
                new SerializableGuidStructureItem[] { new SerializableGuidStructureItem(Guid.NewGuid(), "Test name 1", bottomGuid, topGuid, null, false, 0.7f)}, true, 1f)
            };

            byte[] serialized = PixiParser.Serialize(document);

            SerializableDocument deserializedDocument = PixiParser.Deserialize(serialized);

            AssertEqual(document, deserializedDocument);
        }

        [Fact]
        public void SerializeAndDeserializeEmptyLayer()
        {
            SerializableDocument document = new()
            {
                Width = 1,
                Height = 1
            };

            document.Layers = new List<SerializableLayer>()
            {
                new SerializableLayer()
                {
                    Width = 0, Height = 0,
                    MaxHeight = 1, MaxWidth = 1,
                    IsVisible = true,
                    Name = "Base Layer", Opacity = 1,
                    OffsetX = 0, OffsetY = 0,
                }
            };

            var serialized = PixiParser.Serialize(document);

            SerializableDocument deserialized = PixiParser.Deserialize(serialized);

            AssertEqual(document, deserialized);
        }

        [Fact]
        public void DetectOldFile()
        {
            Assert.Throws<OldFileFormatException>(() => PixiParser.Deserialize("./Files/OldPixiFile.pixi"));
        }

        [Fact]
        public void DetectCorruptedFile()
        {
            Assert.Throws<InvalidFileException>(() => PixiParser.Deserialize("./Files/CorruptedPixiFile.pixi"));
        }

        [Fact]
        public void CanOpenExistingPixiFile()
        {
            PixiParser.Deserialize("./Files/Room.pixi");
        }

        private void AssertEqual(SerializableDocument document, SerializableDocument otherDocument)
        {
            Assert.Equal(document.FileVersion, otherDocument.FileVersion);
            Assert.Equal(document.Height, otherDocument.Height);
            Assert.Equal(document.Width, otherDocument.Width);
            Assert.Equal(document.Swatches.Count, otherDocument.Swatches.Count);

            Assert.Equal(document.Layers[0].PngBytes, otherDocument.Layers[0].PngBytes);
        }
    }
}