using HarmonyLib;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(LoadingScreen))]
    internal static class LoadingScreen_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Show")]
        private static void Show_Postfix()
        {
            ModUIConstants.ShowLoadingScreen();
        }

        [HarmonyPostfix]
        [HarmonyPatch("Hide")]
        private static void Hide_Postfix()
        {
            ModUIConstants.HideLoadingScreen();
        }
    }
}
