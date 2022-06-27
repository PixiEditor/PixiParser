using System.Collections.Generic;

namespace PixiEditor.Parser;

/// <summary>
/// A structure member containing other structure members
/// </summary>
public interface IChildrenContainer : IStructureMember, IList<IStructureMember>
{
    public void AddRange(IEnumerable<IStructureMember> children);
}