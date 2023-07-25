using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CDOverhaul
{
    internal static class ExclusiveColorsControllerV2
    {
        // These are PlayFab IDs
        private const string A_TVCAT_ID = "883CC7F4CA3155A3";
        private const string ZOLOR_ID = "193564D7A14F9C33";
        private const string ELECTRIFIED_CYBERKICK_ID = "F08DA308234126FB";

        private static readonly ReadOnlyCollection<ExclusiveColor> s_Colors = new ReadOnlyCollection<ExclusiveColor>(new List<ExclusiveColor>()
        {
            new ExclusiveColor (A_TVCAT_ID, 4, new Color(0.76f, 0.85f, 1, 0.82f)),
            new ExclusiveColor (ZOLOR_ID, 10, new Color(0.45f, 0.04f, 0.65f, 1f)),
            new ExclusiveColor (ELECTRIFIED_CYBERKICK_ID, 10, "#630330", 0.13f),
            new ExclusiveColor (ELECTRIFIED_CYBERKICK_ID, 16, "#76ff7a", 0.13f),
        });

        public static void FindAndApplyExclusiveColor(FirstPersonMover firstPersonMover, Color currentColor, out Color newColor)
        {
            newColor = currentColor;
            foreach (ExclusiveColor exclusiveColor in s_Colors)
            {
                exclusiveColor.TryApplyColorOnRobot(firstPersonMover, currentColor, out newColor, out bool success);
                if (success)
                    return;
            }
        }
    }
}
