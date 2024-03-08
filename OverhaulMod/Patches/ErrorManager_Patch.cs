using HarmonyLib;
using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ErrorManager))]
    internal static class ErrorManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("sendExceptionDetailsToLoggly")]
        private static bool sendExceptionDetailsToLoggly_Prefix(ErrorManager __instance)
        {
            return false;
        }

        [HarmonyPriority(1)]
        [HarmonyPrefix]
        [HarmonyPatch("HandleLog")]
        private static bool HandleLog_Prefix(ErrorManager __instance, string logString, string stackTrace, LogType type)
        {
            if (!__instance._hasCrashed && (type == LogType.Error || type == LogType.Exception))
            {
                return !CrashPreventionManager.OnGameCrashed();
            }
            return true;
        }
    }
}
