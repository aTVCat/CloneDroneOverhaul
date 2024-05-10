using HarmonyLib;
using OverhaulMod.Engine;
using System.Text;
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
            if (CrashPreventionManager.IgnoreCrashes)
                return true;

            if (!__instance._hasCrashed && (type == LogType.Error || type == LogType.Exception))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(logString);
                stringBuilder.Append(' ');
                stringBuilder.Append(stackTrace);
                string fullString = stringBuilder.ToString();
                stringBuilder.Clear();

                if (fullString.Contains("UpdateMe"))
                    return false;

                return !CrashPreventionManager.OnGameCrashed();
            }
            return true;
        }
    }
}
