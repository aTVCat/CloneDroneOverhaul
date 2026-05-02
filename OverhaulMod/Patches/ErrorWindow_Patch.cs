using HarmonyLib;
using OverhaulMod.Utils;
using System;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ErrorWindow))]
    internal static class ErrorWindow_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ErrorWindow.Show))]
        private static bool Show_Prefix(ErrorWindow __instance, string errorMessage)
        {
            if (!ModCore.IsActive()) return true;

            PostmanManager.ErrorReportText = errorMessage;
            if (ModUIManager.Instance)
            {
                try
                {
                    _ = ModUIs.ShowCrashScreen(errorMessage);
                }
                catch (Exception)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(ErrorWindow.Hide))]
        private static bool Hide_Prefix(ErrorWindow __instance)
        {
            if (!ModCore.IsActive()) return true;

            if (Time.timeSinceLevelLoad < 5f)
                return false;

            ErrorManager errorManager = ErrorManager.Instance;
            if (!errorManager || errorManager.HasCrashed())
                return false;

            try
            {
                ModUIs.HideCrashScreen();
            }
            catch { }
            return true;
        }
    }
}
