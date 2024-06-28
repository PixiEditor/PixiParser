using System.Collections.Generic;

namespace PixiEditor.Parser;

public interface IKeyFrameChildrenContainer 
{
    public List<IKeyFrame> Children { get; }
}