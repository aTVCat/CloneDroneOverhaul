using HarmonyLib;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(EndlessResultScreen))]
    internal static class EndlessResultScreen_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Show")]
        private static bool showShowLoss_Prefix(EndlessResultScreen __instance)
        {
            //ModUIConstants.ShowEndlessGameLossWindow();
            return true;
        }
    }
}
