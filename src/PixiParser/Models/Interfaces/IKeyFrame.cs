using MessagePack;

namespace PixiEditor.Parser;

[Union(0, typeof(KeyFrameGroup))]
[Union(1, typeof(RasterKeyFrame))]
public interface IKeyFrame
{
    
}