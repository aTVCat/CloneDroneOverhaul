using HarmonyLib;
using OverhaulMod.Utils;
using System;
using System.Reflection;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ErrorWindow))]
    internal static class ErrorWindow_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ErrorWindow.Show))]
        private static void Show_Postfix(ErrorWindow __instance, string errorMessage)
        {
            PostmanManager.ErrorReportText = errorMessage;
            if (ModUIManager.Instance)
            {
                try
                {
                    _ = ModUIConstants.ShowCrashScreen(errorMessage);
                    __instance.gameObject.SetActive(false);
                }
                catch (Exception) { }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(ErrorWindow.Hide))]
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
