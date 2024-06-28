using System.Collections.Generic;

namespace PixiEditor.Parser;

/// <summary>
/// A structure member containing other structure members
/// </summary>
public interface IStructureChildrenContainer : IStructureMember
{
    public List<IStructureMember> Children { get; }
}