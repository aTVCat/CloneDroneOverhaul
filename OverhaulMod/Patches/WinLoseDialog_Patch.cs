using HarmonyLib;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(WinLoseDialog))]
    internal static class WinLoseDialog_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("showLoss")]
        private static bool showLoss_Prefix(WinLoseDialog __instance)
        {
            ModUIConstants.ShowGameLossWindow();
            return false;
        }
    }
}
