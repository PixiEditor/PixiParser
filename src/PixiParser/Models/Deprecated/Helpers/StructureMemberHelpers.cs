using System;
using System.Collections.Generic;
using System.Linq;
using PixiEditor.Parser.Deprecated.Interfaces;

namespace PixiEditor.Parser.Deprecated.Helpers;

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

    public static Stack<IStructureChildrenContainer> GetParents(this IStructureMember member, DeprecatedDocument document) =>
        GetParents(member, document.RootFolder);

    public static Stack<IStructureChildrenContainer> GetParents(this IStructureMember member, IStructureChildrenContainer root)
    {
        var parents = new Stack<IStructureChildrenContainer>();

        GetParents(member, root, parents);
        
        return parents;
    }
    
    private static void GetParents(this IStructureMember member, IStructureChildrenContainer root, Stack<IStructureChildrenContainer> parents)
    {
        foreach (var children in root.Children)
        {
            if (children == member) return; // Found the member
            if (member is not IStructureChildrenContainer container) continue; // Not the member nor a container, skip
            
            parents.Push(container);
            GetParents(member, container, parents);
        }
    }
    
    public static float GetFinalOpacity(this IStructureOpacity member, DeprecatedDocument document) =>
        member.Opacity == 0 ? 0 : member.GetParents(document).OfType<IStructureOpacity>()
            .Aggregate(member.Opacity, (current, parent) => current * parent.Opacity);

    public static bool GetFinalVisibility(this IStructureMember member, DeprecatedDocument document) =>
        member.Enabled && member.GetParents(document).All(parent => parent.Enabled);

    public static IEnumerable<IStructureMember> GetChildrenRecursive(this IStructureChildrenContainer container)
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

    internal static string GetDebugName(this IStructureMember member, DeprecatedDocument document) =>
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