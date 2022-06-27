using System;

namespace PixiEditor.Parser.Helpers;

internal static class IStructureMemberHelpers
{
    internal static void SetOpacity(this IStructureMember member, ref float variable, float value)
    {
        if (value < 0 || variable > 1)
        {
            throw new ArgumentException("Opacity value must be between 0 and 1", nameof(value));
        }

        variable = value;
    }
}