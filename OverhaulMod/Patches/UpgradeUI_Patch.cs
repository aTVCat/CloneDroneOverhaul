using HarmonyLib;
using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(UpgradeUI))]
    internal static class UpgradeUI_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(UpgradeUI.Show))]
        private static void Show_Postfix(UpgradeUI __instance)
        {
            bool isBattleRoyale = GameModeManager.IsBattleRoyale();

            RectTransform button = __instance.ExitButton.transform as RectTransform;
            button.anchoredPosition = isBattleRoyale ? new Vector2(275f, -230f) : new Vector2(283.7f, 137.8961f);
            if (!button.gameObject.activeSelf && isBattleRoyale)
            {
                button.gameObject.SetActive(true);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(UpgradeUI.closeUpgradeUIIfLocalPlayerHasNoSkillPoints))]
        private static bool closeUpgradeUIIfLocalPlayerHasNoSkillPoints_Prefix(UpgradeUI __instance)
        {
            if (AutoBuildManager.Instance && AutoBuildManager.Instance.isInAutoBuildConfigurationMode)
                return false;

            return true;
        }
    }
}
