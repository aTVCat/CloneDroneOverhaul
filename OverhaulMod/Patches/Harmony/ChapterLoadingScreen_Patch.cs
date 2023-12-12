using HarmonyLib;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(ChapterLoadingScreen))]
    internal static class ChapterLoadingScreen_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Show")]
        private static void Show_Postfix(ChapterLoadingScreen __instance)
        {
            ModUIConstants.ShowLoadingScreen();
        }

        [HarmonyPostfix]
        [HarmonyPatch("Hide")]
        private static void Hide_Prefix(ChapterLoadingScreen __instance)
        {
            ModUIConstants.HideLoadingScreen();
        }
    }
}
