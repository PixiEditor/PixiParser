using System;

namespace PixiEditor.Parser;

public interface IGuid : IStructureMember
{
    public Guid Guid { get; set; }
}