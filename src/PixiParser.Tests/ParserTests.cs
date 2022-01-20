using Xunit;

namespace PixiEditor.Parser.Tests;

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

        document.Swatches.Add(255, 255, 255);

        SerializableLayer layer = document.Layers.Add("Base Layer", 1, 1, false, 0.5f, 1, 1);
        layer.PngBytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

        document.Groups.Add(new SerializableGroup("Base Group"));

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

        document.Layers.Add("Base Layer");

        var serialized = PixiParser.Serialize(document);

        SerializableDocument deserialized = PixiParser.Deserialize(serialized);

        AssertEqual(document, deserialized);
    }

    [Fact]
    public void DetectOldFile() => Assert.Throws<OldFileFormatException>(() => PixiParser.Deserialize("./Files/OldPixiFile.pixi"));

    [Fact]
    public void DetectCorruptedFile() => Assert.Throws<InvalidFileException>(() => PixiParser.Deserialize("./Files/CorruptedPixiFile.pixi"));

    [Fact]
    public void CanOpenExistingFile() => PixiParser.Deserialize("./Files/16x16,PPD-3.pixi");

    [Fact]
    public void IsBackwardsCompatible() => PixiParser.Deserialize("./Files/16x16,PE-0.6.pixi");

    [Fact]
    public void LayerStructureWorks()
    {
        SerializableDocument document = new();

        SerializableLayer layer1 = document.Layers.Add("Test Layer 1");
        SerializableLayer layer2 = document.Layers.Add("Test Layer 2");
        SerializableLayer layer3 = document.Layers.Add("Test Layer 3");
        SerializableLayer layer4 = document.Layers.Add("Test Layer 4");

        SerializableGroup group1 = new("Group 1");
        SerializableGroup group2 = new("Group 1|1");
        group1.Subgroups.Add(group2);
        SerializableGroup group3 = new("Group 2");

        document.Groups.Add(group1);
        document.Groups.Add(group2);
        document.Groups.Add(group3);

        document.Layers.AddToGroup(group1, layer1);
        document.Layers.AddToGroup(group2, layer2);
        document.Layers.AddToGroup(group1, layer3);

        document.Layers.AddToGroup(group3, layer4);

        Assert.Equal(0, document.Groups[0].StartLayer);
        Assert.Equal(2, document.Groups[0].EndLayer);

        Assert.Equal(1, document.Groups[1].StartLayer);
        Assert.Equal(1, document.Groups[1].EndLayer);

        Assert.Equal(3, document.Groups[2].StartLayer);
        Assert.Equal(3, document.Groups[2].EndLayer);

        Assert.True(document.Layers.ContainedIn(group1, layer1));
        Assert.True(document.Layers.ContainedIn(group1, layer2));
        Assert.True(document.Layers.ContainedIn(group1, layer3));
        Assert.True(document.Layers.ContainedIn(group2, layer2));
        Assert.False(document.Layers.ContainedIn(group1, layer4));
        Assert.True(document.Layers.ContainedIn(group3, layer4));
    }

    private static void AssertEqual(SerializableDocument document, SerializableDocument otherDocument)
    {
        Assert.Equal(document.FileVersion, otherDocument.FileVersion);
        Assert.Equal(document.Height, otherDocument.Height);
        Assert.Equal(document.Width, otherDocument.Width);
        Assert.Equal(document.Swatches.Count, otherDocument.Swatches.Count);
        Assert.Equal(document.Layers.Count, document.Layers.Count);
        Assert.Equal(document.Groups.Count, document.Groups.Count);

        for (int i = 0; i < document.Swatches.Count; i++)
        {
            Assert.Equal(document.Swatches[i], otherDocument.Swatches[i]);
        }

        for (int i = 0; i < document.Layers.Count; i++)
        {
            Assert.Equal(document.Layers[i], otherDocument.Layers[i]);
        }

        for (int i = 0; i < document.Groups.Count; i++)
        {
            Assert.Equal(document.Groups[i], otherDocument.Groups[i]);
        }
    }
}
