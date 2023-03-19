using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ErrorManager))]
    internal static class ErrorManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HandleLog")]
        private static bool HandleLog_Prefix(string logString, string stackTrace, LogType type)
        {
            if (!OverhaulMod.IsCoreCreated)
            {
                return true;
            }
            if(type == LogType.Error || type == LogType.Exception) OverhaulDebugController.PrintError(logString + "\n" + stackTrace);
            return false;
        }
    }
}
