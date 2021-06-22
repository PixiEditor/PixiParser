using System;
using System.Collections.Generic;
using Xunit;

namespace PixiEditor.Parser.Tests
{
    public class ParserTest
    {
        [Fact]
        public void SerializingAndDeserialzingWorks()
        {
            SerializableDocument document = new SerializableDocument
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
                        BitmapBytes = imageData,
                        IsVisible = true, Name = "Base Layer",
                        Opacity = 1
                }
            };

            var topGuid = Guid.NewGuid();
            var bottomGuid = Guid.NewGuid();

            document.Groups = new Models.SerializableGuidStructureItem[]
            {
                new Models.SerializableGuidStructureItem(Guid.NewGuid(), "Test name", bottomGuid, topGuid,
                new Models.SerializableGuidStructureItem[] { new Models.SerializableGuidStructureItem(Guid.NewGuid(), "Test name 1", bottomGuid, topGuid, null, false, 0.7f)}, true, 1f)
            };

            byte[] serialized = PixiParser.Serialize(document);

            SerializableDocument deserializedDocument = PixiParser.Deserialize(serialized);

            AssertEqual(document, deserializedDocument);
        }

        [Fact]
        public void SerializeAndDeserializeEmptyLayer()
        {
            SerializableDocument document = new SerializableDocument
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
            Assert.Throws<OldFileFormatException>(() => PixiParser.Deserialize("./OldPixiFile.pixi"));
        }

        [Fact]
        public void DetectCorruptedFile()
        {
            Assert.Throws<InvalidFileException>(() => PixiParser.Deserialize("./CorruptedPixiFile.pixi"));
        }

        [Fact]
        public void CanOpenExistingPixiFile()
        {
            PixiParser.Deserialize("./Room.pixi");
        }

        private void AssertEqual(SerializableDocument document, SerializableDocument otherDocument)
        {
            Assert.Equal(document.FileVersion, otherDocument.FileVersion);
            Assert.Equal(document.Height, otherDocument.Height);
            Assert.Equal(document.Width, otherDocument.Width);
            Assert.Equal(document.Swatches.Count, otherDocument.Swatches.Count);

            Assert.Equal(document.Layers[0].BitmapBytes, otherDocument.Layers[0].BitmapBytes);
        }
    }
}