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
            if (logString.Contains("WeaponModel.ReplaceModel"))
            {
                return false;
            }
            return !OverhaulMod.IsCoreCreated;
        }
    }
}
