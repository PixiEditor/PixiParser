using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PixiEditor.Parser.Deprecated;
using PixiEditor.Parser.Deprecated.Helpers;
using PixiEditor.Parser.Deprecated.Interfaces;
using PixiEditor.Parser.Graph;
using PixiEditor.Parser.Skia;
using SkiaSharp;
using Xunit;

namespace PixiEditor.Parser.Tests;

public class ParserTests
{
    [Fact]
    public void SerializingAndDeserializingWorks()
    {
        var document = GetFullDocument();

        using FileStream stream = new("test.pixi", FileMode.Create);
        PixiParser.Serialize(document, stream);

        stream.Position = 0;

        var deserializedDocument = PixiParser.Deserialize(stream);

        AssertEqual(document, deserializedDocument);
    }

    [Fact]
    public async Task SerializingAndDeserialzingWorksAsync()
    {
        var document = GetFullDocument();

        await using FileStream stream = new("testAsync.pixi", FileMode.Create);
        await PixiParser.SerializeAsync(document, stream);

        stream.Position = 0;

        var deserializedDocument = await PixiParser.DeserializeAsync(stream);

        AssertEqual(document, deserializedDocument);
    }

    [Fact]
    public void DetectCorruptedFile() =>
        Assert.Throws<InvalidFileException>(() => PixiParser.Deserialize("./Files/CorruptedPixiFile.pixi"));

    private static Document GetFullDocument()
    {
        Document document = new()
        {
            Height = 32,
            Width = 40
        };

        document.Swatches.Add(234, 254, 153, 255);
        document.Swatches.Add(0, 254, 153, 80);
        document.Swatches.Add(254, 153, 80);

        document.Palette.Add(234, 254, 153, 255);
        document.Palette.Add(0, 254, 153, 80);
        document.Palette.Add(254, 153, 80);

        document.PreviewImage = new byte[]
        {
            0x42,
            0x21,
            0x64,
            0xAC
        };

        // document.ReferenceLayer = new ReferenceLayer()
        // {
        //     Width = 2,
        //     Height = 3,
        //     OffsetX = 5,
        //     OffsetY = 1,
        //     Enabled = false,
        //     Guid = Guid.NewGuid(),
        //     ImageBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 },
        //     Name = "Reference",
        //     Opacity = 0.45f,
        // };

        //
        // subFolder.Children.Add(new ImageLayer { Name = "Sublayer1", Enabled = false, Height = 8, Width = 32, ImageBytes = new byte[] { 32, 65, 12, 65, 255  }, OffsetX = 2, OffsetY = 1, Opacity = 0f });

        SKBitmap bitmap = new SKBitmap(40, 32);

        bitmap.SetPixel(30, 16, SKColors.Beige);

        document.Graph = new NodeGraph();
        document.Graph.AllNodes = new List<Node>();
        
        document.Graph.AllNodes.Add(CreateImageNode(bitmap, BuiltInEncoders.Encoders[document.ImageEncoderUsed]));
        
        Node outputNode = new Node()
        {
            Name = "OUTPUT_NODE",
            UniqueNodeName = "Output",
            Position = new Vector2 { X = 0, Y = 0 },
            Id = 1,
            InputConnections = new []
            {
                new PropertyConnection(){ NodeId = document.Graph.AllNodes[0].Id, PropertyName = "Output"}
            }
        };
        
        document.Graph.AllNodes.Add(outputNode);

        return document;
    }

    private static Node CreateImageNode(SKBitmap bitmap, ImageEncoder encoder)
    {
        Node node = new()
        {
            Name = "ImageNode",
            UniqueNodeName = "ImageLayer",
            Position = new Vector2() { X = 2, Y = 3 },
            Id = 0,
            AdditionalData = new()
            {
                {
                    "Images", new List<List<byte>>()
                    {
                        new(encoder.Encode(bitmap.Bytes, bitmap.Width, bitmap.Height))
                    }
                }
            }
        };

        return node;
    }

    private static void AssertEqual(Document expectedDocument, Document actualDocument)
    {
        Assert.Equal(expectedDocument.Version, actualDocument.Version);
        Assert.Equal(expectedDocument.Height, actualDocument.Height);
        Assert.Equal(expectedDocument.Width, actualDocument.Width);
        Assert.Equal(expectedDocument.Swatches.Count, actualDocument.Swatches.Count);
        Assert.Equal(expectedDocument.Graph.AllNodes?.Count, actualDocument.Graph.AllNodes?.Count);

        if (expectedDocument.PreviewImage == null)
        {
            Assert.Empty(actualDocument.PreviewImage);
        }
        else
        {
            Assert.True(expectedDocument.PreviewImage.SequenceEqual(actualDocument.PreviewImage));
        }

        //AssertMember(expectedDocument.ReferenceLayer, actualDocument.ReferenceLayer);

        for (int i = 0; i < expectedDocument.Swatches.Count; i++)
        {
            Assert.Equal(expectedDocument.Swatches[i], actualDocument.Swatches[i]);
        }

        for (int i = 0; i < expectedDocument.Palette.Count; i++)
        {
            Assert.Equal(expectedDocument.Swatches[i], actualDocument.Swatches[i]);
        }

        if (expectedDocument.Graph is { AllNodes: not null })
        {
            Assert.NotNull(actualDocument.Graph);
            Assert.NotNull(actualDocument.Graph.AllNodes);
            foreach (var member in expectedDocument.Graph.AllNodes
                         .Zip(actualDocument.Graph.AllNodes,
                             (expected, actual) => new { Expected = expected, Actual = actual }))
            {
                AssertNode(member.Expected, member.Actual);
            }
        }

        void AssertNode(Node expected, Node actual)
        {
            if (expected is null)
            {
                Assert.Null(actual);
                return;
            }

            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.UniqueNodeName, actual.UniqueNodeName);
            Assert.Equal(expected.Position, actual.Position);
            Assert.Equal(expected.AdditionalData?.Count, actual.AdditionalData?.Count);
            Assert.Equal(expected.InputConnections?.Length, actual.InputConnections?.Length);
            Assert.Equal(expected.Id, actual.Id);

            Assert.IsType(expected.GetType(), actual);

            if (expected.AdditionalData != null)
            {
                Assert.NotNull(actual.AdditionalData);
                foreach (var additionalData in expected.AdditionalData)
                {
                    Assert.True(actual.AdditionalData.TryGetValue(additionalData.Key, out object actualValue));
                    Assert.Equal(additionalData.Value, actualValue);
                }
            }
        }
    }

#nullable enable

    private class Matcher
    {
        private object Expected { get; }

        private object Actual { get; }

        public Matcher(object expected, object actual)
        {
            Expected = expected;
            Actual = actual;
        }

        public bool Match<T>(
            [NotNullWhen(true)] out T? expected,
            [NotNullWhen(true)] out T? actual)
        {
            expected = default;
            actual = default;

            if (Expected is not T ex || Actual is not T ac) return false;

            expected = ex;
            actual = ac;

            return true;
        }
    }
}