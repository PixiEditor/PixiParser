using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PixiEditor.Parser.Deprecated;
using PixiEditor.Parser.Helpers;
using SkiaSharp;
using Xunit;

namespace PixiEditor.Parser.Tests;

public class ParserTests
{
    [Fact]
    public void SerializingAndDeserialzingWorks()
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
    public void DetectOldFile() => Assert.Throws<OldFileFormatException>(() => DepractedPixiParser.Deserialize("./Files/OldPixiFile.pixi"));

    [Fact]
    public void DetectCorruptedFile() => Assert.Throws<InvalidFileException>(() => PixiParser.Deserialize("./Files/CorruptedPixiFile.pixi"));

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

        var subFolder = new Folder
        {
            Name = "Folder1",
            Enabled = false,
            Mask = new Mask(),
            Opacity = 0.8f,
        };
        //
        // subFolder.Children.Add(new ImageLayer { Name = "Sublayer1", Enabled = false, Height = 8, Width = 32, ImageBytes = new byte[] { 32, 65, 12, 65, 255  }, OffsetX = 2, OffsetY = 1, Opacity = 0f });

        SKBitmap bitmap = new SKBitmap(40, 32);
        
        bitmap.SetPixel(30, 16, SKColors.Beige);
        
        document.RootFolder = new(new IStructureMember[]
        {
            subFolder,
            new ImageLayer
            {
                Name = "Layer1",
                Enabled = true,
                Height = 40,
                Width = 32,
                OffsetX = 0,
                OffsetY = 0,
                Opacity = 0.8f,
                ImageBytes = bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray()
            }
        });

        return document;
    }
    
    private static void AssertEqual(Document expectedDocument, Document actualDocument)
    {
        Assert.Equal(expectedDocument.Version, actualDocument.Version);
        Assert.Equal(expectedDocument.Height, actualDocument.Height);
        Assert.Equal(expectedDocument.Width, actualDocument.Width);
        Assert.Equal(expectedDocument.Swatches.Count, actualDocument.Swatches.Count);
        Assert.Equal(expectedDocument.RootFolder.GetChildrenRecursive().Count(), actualDocument.RootFolder.GetChildrenRecursive().Count());

        if (expectedDocument.PreviewImage == null)
        {
            Assert.Empty(actualDocument.PreviewImage);
        }
        else
        {
            Assert.True(expectedDocument.PreviewImage.SequenceEqual(actualDocument.PreviewImage));
        }

        AssertMember(expectedDocument.ReferenceLayer, actualDocument.ReferenceLayer);

        for (int i = 0; i < expectedDocument.Swatches.Count; i++)
        {
            Assert.Equal(expectedDocument.Swatches[i], actualDocument.Swatches[i]);
        }

        for (int i = 0; i < expectedDocument.Palette.Count; i++)
        {
            Assert.Equal(expectedDocument.Swatches[i], actualDocument.Swatches[i]);
        }

        foreach (var member in expectedDocument.RootFolder.GetChildrenRecursive()
                     .Zip(actualDocument.RootFolder.GetChildrenRecursive(), (expected, actual) => new { Expected = expected, Actual = actual }))
        {
            AssertMember(member.Expected, member.Actual);
        }
        
        void AssertMember(IStructureMember expected, IStructureMember actual)
        {
            if (expected is null)
            {
                Assert.Null(actual);
                return;
            }
            
            Assert.Equal(expected.Enabled, actual.Enabled);
            Assert.IsType(expected.GetType(), actual);

            Matcher matcher = new(expected, actual);

            if (matcher.Match<IChildrenContainer>(out var expectedContainer, out var actualContainer))
            {
                foreach (var member in expectedContainer.GetChildrenRecursive()
                             .Zip(actualContainer.GetChildrenRecursive(), (expected, actual) => new { Expected = expected, Actual = actual }))
                {
                    AssertMember(member.Expected, member.Actual);
                }
            }

            if (matcher.Match<IGuid>(out var expectedGuid, out var actualGuid))
            {
                Assert.Equal(expectedGuid.Guid, actualGuid.Guid);
            }
            
            if (matcher.Match<IImageContainer>(out var expectedImage, out var actualImage))
            {
                if (expectedImage.ImageBytes is null)
                {
                    Assert.Empty(actualImage.ImageBytes);
                }
                else
                {
                    Assert.True(expectedImage.ImageBytes.SequenceEqual(actualImage.ImageBytes),
                        "Actual image bytes did not match expected image bytes");
                }
            }
            
            if (matcher.Match<IMaskable>(out var expectedMaskable, out var actualMaskable))
            {
                AssertMember(expectedMaskable.Mask, actualMaskable.Mask);
            }
            
            if (matcher.Match<IName>(out var expectedName, out var actualName))
            {
                Assert.Equal(expectedName.Name, actualName.Name);
            }
            
            if (matcher.Match<IOpacity>(out var expectedOpacity, out var actualOpacity))
            {
                Assert.Equal(expectedOpacity.Opacity, actualOpacity.Opacity);
            }
            
            if (matcher.Match<ISize<int>>(out var expectedSize, out var actualSize))
            {
                Assert.Equal(expectedSize.Height, actualSize.Height);
                Assert.Equal(expectedSize.Width, actualSize.Width);
                Assert.Equal(expectedSize.OffsetX, actualSize.OffsetX);
                Assert.Equal(expectedSize.OffsetY, actualSize.OffsetY);
            }
            
            if (matcher.Match<ISize<float>>(out var expectedFloatSize, out var actualFloatSize))
            {
                Assert.Equal(expectedFloatSize.Height, actualFloatSize.Height);
                Assert.Equal(expectedFloatSize.Width, actualFloatSize.Width);
            }
        }
    }
    
#nullable enable

    private class Matcher
    {
        private IStructureMember Expected { get; }
        
        private IStructureMember Actual { get; }
        
        public Matcher(IStructureMember expected, IStructureMember actual)
        {
            Expected = expected;
            Actual = actual;
        }
        
        public bool Match<T>(
            [NotNullWhen(true)] out T? expected,
            [NotNullWhen(true)] out T? actual)
            where T : IStructureMember
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
