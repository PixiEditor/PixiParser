using PixiEditor.Parser.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using PixiEditor.Parser.Deprecated;
using PixiEditor.Parser.Graph;

namespace PixiEditor.Parser.Benchmarks;

public static class Helper
{
    public static Document CreateDocument(int size, int layers, ImageEncoder? encoder) 
    {
        var benchmarkDocument = new Document()
        {
            Width = size,
            Height = size
        };

        benchmarkDocument.Swatches.Add(255, 255, 255, 255);
        benchmarkDocument.Graph = new NodeGraph();
        benchmarkDocument.Graph.AllNodes = new List<Node>();

        for (int i = 0; i < layers; i++)
        {
            var layer = CreateImageNode(encoder != null ? CreateSKBitmap(size) : null, encoder); 
            
            benchmarkDocument.Graph.AllNodes.Add(layer);
        }

        return benchmarkDocument;
    }

    public static SKBitmap CreateSKBitmap(int size)
    {
        Random random = new(2);
        SKBitmap bitmap = new(size, size);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bitmap.SetPixel(x, y, new SKColor((uint)random.Next()));
            }
        }

        return bitmap;
    }

    private static Node CreateImageNode(SKBitmap? bitmap, ImageEncoder? encoder)
    {
        Node node = new()
        {
            Name = "ImageNode",
            UniqueNodeName = "ImageLayer",
            Position = new Vector2() { X = 2, Y = 3 },
            Id = 0,
            AdditionalData = new Dictionary<string, object>()
        };
        
        if (bitmap != null && encoder != null)
        {
            byte[] encoded = encoder.Encode(bitmap.Bytes, bitmap.Width, bitmap.Height);
            node.AdditionalData["Images"] = new List<List<byte>>()
            {
                new(encoded)
            };
        }

        return node;
    }
}