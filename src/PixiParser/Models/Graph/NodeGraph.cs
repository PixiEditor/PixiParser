using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace PixiEditor.Parser.Graph;

[MessagePackObject]
public class NodeGraph
{
    [Key(0)] public List<Node> AllNodes { get; set; }

    public IImageContainer[] CollectImageContainers()
    {
        List<IImageContainer> imageContainers = new();
        foreach (Node node in AllNodes)
        {
            if (node.InputPropertyValues != null)
            {
                foreach (NodePropertyValue propertyValue in node.InputPropertyValues)
                {
                    if (propertyValue.Value is IImageContainer imageContainer)
                    {
                        imageContainers.Add(imageContainer);
                    }
                }
            }

            if (node.AdditionalData != null)
            {
                foreach (var additionalData in node.AdditionalData)
                {
                    if (additionalData.Value is IEnumerable enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            TryAddImageContainer(item, imageContainers);
                        }
                    }
                    else
                    {
                        TryAddImageContainer(additionalData.Value, imageContainers);
                    }
                }
            }
        }

        return imageContainers.ToArray();
    }

    private static void TryAddImageContainer(object value, List<IImageContainer> imageContainers)
    {
        if (value is IImageContainer imageContainer)
        {
            imageContainers.Add(imageContainer);
        }
    }
}