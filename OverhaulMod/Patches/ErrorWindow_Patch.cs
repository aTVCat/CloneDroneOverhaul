using HarmonyLib;
using OverhaulMod.Utils;

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
                ModUIConstants.ShowCrashScreen(errorMessage);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Hide")]
        private static bool Hide_Prefix(ErrorWindow __instance)
        {
            ErrorManager errorManager = ErrorManager.Instance;
            if (!errorManager || errorManager.HasCrashed())
                return false;

            ModUIConstants.HideCrashScreen();
            return true;
        }
    }
}
