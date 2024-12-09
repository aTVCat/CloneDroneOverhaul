using HarmonyLib;
using ModLibrary;
using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(CustomUpgradeManager))]
    internal static class CustomUpgradeManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(CustomUpgradeManager.Update))]
        private static void Update_Postfix(CustomUpgradeManager __instance)
        {
            bool hide = AutoBuildManager.Instance.isInAutoBuildConfigurationMode;
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
