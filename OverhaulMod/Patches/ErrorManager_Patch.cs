using HarmonyLib;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ErrorManager))]
    internal static class ErrorManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("sendExceptionDetailsToLoggly")]
        private static bool sendExceptionDetailsToLoggly_Prefix(ErrorWindow __instance)
        {
            return false;
        }
    }
}
