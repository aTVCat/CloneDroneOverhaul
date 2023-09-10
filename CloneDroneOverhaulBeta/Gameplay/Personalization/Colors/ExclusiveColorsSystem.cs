using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CDOverhaul
{
    internal static class ExclusiveColorsSystem
    {
        private const string A_TVCAT_ID = "883CC7F4CA3155A3";
        private const string ZOLOR_ID = "193564D7A14F9C33";
        private const string ELECTRIFIED_CYBERKICK_ID = "F08DA308234126FB";
        private const string LAKI_ID = "ADB93DF0BAD2B594";

        private static readonly ReadOnlyCollection<ExclusiveColorDefinition> s_Colors = new ReadOnlyCollection<ExclusiveColorDefinition>(new List<ExclusiveColorDefinition>()
        {
            new ExclusiveColorDefinition (A_TVCAT_ID, 4, new Color(0.76f, 0.85f, 1, 0.58f)),
            new ExclusiveColorDefinition (ZOLOR_ID, 10, new Color(0.45f, 0.04f, 0.65f, 1f)),
            new ExclusiveColorDefinition (ELECTRIFIED_CYBERKICK_ID, 10, "#630330", 0.13f),
            new ExclusiveColorDefinition (ELECTRIFIED_CYBERKICK_ID, 16, "#76ff7a", 0.13f),
            new ExclusiveColorDefinition (LAKI_ID, 0, new Color(1f, 1f, 1f, 0.85f)),
        });

        public static void FindAndApplyExclusiveColor(FirstPersonMover firstPersonMover, Color currentColor, out Color newColor)
        {
            newColor = currentColor;
            foreach (ExclusiveColorDefinition exclusiveColor in s_Colors)
            {
                if (exclusiveColor.TryApplyColorOnRobot(firstPersonMover, currentColor, out newColor))
                    return;
            }
        }
    }
}
