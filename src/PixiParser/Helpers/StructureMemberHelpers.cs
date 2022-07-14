using System;
using System.Collections.Generic;
using System.Linq;

namespace PixiEditor.Parser.Helpers;

public static class StructureMemberHelpers
{
    internal static void SetOpacity(this IStructureMember member, ref float variable, float value)
    {
        if (value < 0 || variable > 1)
        {
            throw new ArgumentException("Opacity value must be between 0 and 1", nameof(value));
        }

        variable = value;
    }

    public static IEnumerable<IStructureMember> GetChildrenRecursive(this IChildrenContainer container)
    {
        foreach (var member in container.Children)
        {
            yield return member;

            if (member is not Folder childContainer) continue;
            
            foreach (var child in GetChildrenRecursive(childContainer))
            {
                yield return child;
            }
        }
    }

    internal static string GetDebugName(this IStructureMember member, Document document) =>
        GetDebugName(member, document.RootFolder.GetChildrenRecursive().Append(document.ReferenceLayer));

    internal static string GetDebugName(this IStructureMember member, IEnumerable<IStructureMember> rootFolder)
    {
        int i = 0;

        var structureMembers = rootFolder as IStructureMember[] ?? rootFolder.ToArray();
        foreach (var children in structureMembers)
        {
            if (member == children && member is ImageLayer image)
            {
                return $"Image '{image.Name}' [{i}]";
            }
            
            if (member == children && member is Folder folder)
            {
                return $"Folder '{folder.Name}' [{i}]";
            }
            
            if (member == children && member is ReferenceLayer reference)
            {
                return $"Reference '{reference.Name}' [{i}]";
            }
            
            if (member is Mask mask && children is IMaskable maskable && maskable.Mask == mask)
            {
                return $"Mask of {maskable.GetDebugName(structureMembers)}";
            }
                
            i++;
        }

        return null;
    }
}