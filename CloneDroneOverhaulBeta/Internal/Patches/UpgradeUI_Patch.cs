using CDOverhaul.Graphics;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(UpgradeUI))]
    internal static class UpgradeUI_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Show")]
        private static void refreshTrophies_Postfix(UpgradeUI __instance, bool challengeUpgradeConfigMode = false, bool isStoryCutsceneMode = false, bool isMultiplayerSelection = false)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            __instance.UpgradeUIBackground.gameObject.SetActive(challengeUpgradeConfigMode || ViewModesController.IsFirstPersonModeEnabled);
            int i = 0;
            do
            {
                __instance.UpgradeUIBackground.transform.GetChild(i).gameObject.SetActive(challengeUpgradeConfigMode);
               i++;
            } while (i < __instance.UpgradeUIBackground.transform.childCount);
        }
    }
}
