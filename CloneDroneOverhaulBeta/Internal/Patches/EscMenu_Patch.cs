using CDOverhaul.HUD;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ErrorWindow))]
    internal static class ErrorWindow_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Show")]
        private static bool Show_Prefix()
        {
            return !OverhaulCrashScreen.Instance;
        }
    }
}
