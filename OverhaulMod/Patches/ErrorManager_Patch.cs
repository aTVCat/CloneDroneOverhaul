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
        [HarmonyPatch(nameof(ErrorManager.sendExceptionDetailsToLoggly))]
        private static bool sendExceptionDetailsToLoggly_Prefix(ErrorManager __instance)
        {
            bool value = CrashManager.HasCrashedThisSession;
            if(!value)
                CrashManager.HasCrashedThisSession = true;

            return !value;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(ErrorManager.HandleLog))]
        private static bool HandleLog_Prefix(ErrorManager __instance, string logString, string stackTrace, ref LogType type)
        {
            /*ModCore.TempStringBuilder.Append(logString);
            ModCore.TempStringBuilder.Append(' ');
            ModCore.TempStringBuilder.Append(stackTrace);
            ModCore.TempStringBuilder.Append('\n');*/

            if (CrashManager.IgnoreCrashes)
                return false;

            if (!__instance._hasCrashed && (type == LogType.Error || type == LogType.Exception))
            {
                StringBuilder stringBuilder = new StringBuilder();
                _ = stringBuilder.Append(logString);
                _ = stringBuilder.Append(' ');
                _ = stringBuilder.Append(stackTrace);
                string fullString = stringBuilder.ToString();

                if (CrashManager.OnGameCrashed() || fullString.Contains("UpdateMe"))
                    return false;

                return true;
            }
            return true;
        }
    }
}
