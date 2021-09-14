using System.Collections.Generic;
using static System.Math;

namespace PixiEditor.Parser.Helpers
{
    internal static class StructureHelpers
    {
        public static void ClearGroupLayerIndexes(this SerializableDocument document)
        {
            foreach (SerializableGroup group in document.Groups)
            {
                group.SerializedStartLayer = group.SerializedEndLayer = -1;
            }
        }

        public static void IncreaseGroupLayerIndexes(this SerializableDocument document, int start)
        {
            foreach (SerializableGroup group in document.Groups)
            {
                if (group.SerializedStartLayer >= start)
                {
                    group.SerializedStartLayer++;
                }

                if (group.SerializedEndLayer >= start)
                {
                    group.SerializedStartLayer++;
                }
            }
        }

        public static void DecreaseGroupLayerIndexes(this SerializableDocument document, int start)
        {
            foreach (SerializableGroup group in document.Groups)
            {
                if (group.SerializedStartLayer > start)
                {
                    group.SerializedStartLayer = Max(group.SerializedStartLayer - 1, -1);
                }

                if (group.SerializedEndLayer > start)
                {
                    group.SerializedEndLayer = Max(group.SerializedEndLayer - 1, -1);
                }
            }
        }

        public static void EnsureInside(this SerializableGroup group, int index)
        {
            if (group.SerializedStartLayer == -1)
            {
                group.SerializedStartLayer = index;
            }
            else
            {
                group.SerializedStartLayer = Min(group.SerializedStartLayer, index);
            }

            if (group.SerializedEndLayer == -1)
            {
                group.SerializedEndLayer = index;
            }
            else
            {
                group.SerializedEndLayer = Max(group.SerializedEndLayer, index);
            }
        }

        public static IEnumerable<SerializableGroup> GetStack(this SerializableGroup group, SerializableDocument document)
        {
            Stack<SerializableGroup> groups = new();

            if (ContainedIn(group, document.Groups, ref groups))
            {
                return groups;
            }

            return null;
        }

        private static bool ContainedIn(SerializableGroup searchingFor, IEnumerable<SerializableGroup> searchingIn, ref Stack<SerializableGroup> groups)
        {
            foreach (SerializableGroup subgroup in searchingIn)
            {
                groups.Push(subgroup);

                if (subgroup == searchingFor)
                {
                    return true;
                }

                if (subgroup.Subgroups != null && ContainedIn(searchingFor, subgroup.Subgroups, ref groups))
                {
                    return true;
                }

                groups.Pop();
            }

            return false;
        }
    }
}
