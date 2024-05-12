using HarmonyLib;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ErrorWindow))]
    internal static class ErrorWindow_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Show")]
        private static bool Show_Prefix(ErrorWindow __instance, string errorMessage)
        {
            ModWebhookManager.ErrorReportText = errorMessage;
            if (ModUIManager.Instance)
            {
                try
                {
                    ModUIConstants.ShowCrashScreen(errorMessage);
                }
                catch
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Hide")]
        private static bool Hide_Prefix(ErrorWindow __instance)
        {
            if (Time.timeSinceLevelLoad < 5f)
                return false;

            ErrorManager errorManager = ErrorManager.Instance;
            if (!errorManager || errorManager.HasCrashed())
                return false;

            try
            {
                ModUIConstants.HideCrashScreen();
            }
            catch { }
            return true;
        }
    }
}
