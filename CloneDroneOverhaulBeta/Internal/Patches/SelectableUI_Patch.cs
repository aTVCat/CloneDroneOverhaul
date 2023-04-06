using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(SelectableUI))]
    internal static class SelectableUI_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix(SelectableUI __instance)
        {
            if (__instance.GameThemeData != null)
            {
                __instance.GameThemeData.ButtonBackground[0].Color = new Color(0.19f, 0.37f, 0.88f, 1);
                __instance.GameThemeData.ButtonBackground[1].Color = new Color(0.3f, 0.5f, 1, 1f);
                __instance.GameThemeData.ButtonTextOutline[0].Color = new Color(0.1f, 0.1f, 0.1f, 0.7f);
                __instance.GameThemeData.ButtonTextOutline[1].Color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
            }
        }
    }
}
