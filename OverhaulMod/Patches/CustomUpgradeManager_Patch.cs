using HarmonyLib;
using ModLibrary;
using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(CustomUpgradesUIManager))]
    internal static class CustomUpgradesUIManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(CustomUpgradesUIManager.Update))]
        private static void Update_Postfix(CustomUpgradesUIManager __instance)
        {
            if (!ModCore.IsActive()) return;

            bool hide = AutoBuildManager.Instance.IsInAutoBuildConfigurationMode;
            if (hide)
            {
                GameObject backButton = __instance._backButton;
                if (backButton)
                    backButton.SetActive(false);

                GameObject nextButton = __instance._nextButton;
                if (nextButton)
                    nextButton.SetActive(false);
            }
        }
    }
}
