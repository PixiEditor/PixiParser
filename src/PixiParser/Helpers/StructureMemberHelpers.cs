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

    public static Stack<IChildrenContainer> GetParents(this IStructureMember member, Document document) =>
        GetParents(member, document.RootFolder);

    public static Stack<IChildrenContainer> GetParents(this IStructureMember member, IChildrenContainer root)
    {
        var parents = new Stack<IChildrenContainer>();

        GetParents(member, root, parents);
        
        return parents;
    }
    
    private static void GetParents(this IStructureMember member, IChildrenContainer root, Stack<IChildrenContainer> parents)
    {
        foreach (var children in root.Children)
        {
            if (children == member) return; // Found the member
            if (member is not IChildrenContainer container) continue; // Not the member nor a container, skip
            
            parents.Push(container);
            GetParents(member, container, parents);
        }
    }
    
    public static float GetFinalOpacity(this IOpacity member, Document document) =>
        member.Opacity == 0 ? 0 : member.GetParents(document).OfType<IOpacity>()
            .Aggregate(member.Opacity, (current, parent) => current * parent.Opacity);

    public static bool GetFinalVisibility(this IStructureMember member, Document document) =>
        member.Enabled && member.GetParents(document).All(parent => parent.Enabled);

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