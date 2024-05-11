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

        [HarmonyPrefix]
        [HarmonyPatch("HandleLog")]
        private static bool HandleLog_Prefix(ErrorManager __instance, string logString, string stackTrace, ref LogType type)
        {
            if (CrashPreventionManager.IgnoreCrashes)
            {
                type = LogType.Warning;
                return true;
            }

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

                if (CrashPreventionManager.OnGameCrashed())
                {
                    type = LogType.Warning;
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}
