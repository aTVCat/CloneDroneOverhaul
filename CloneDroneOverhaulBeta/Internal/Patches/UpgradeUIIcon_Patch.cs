using CDOverhaul.Gameplay.QualityOfLife;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(UpgradeUIIcon))]
    internal static class UpgradeUIIcon_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnClickToUpgradeAbility")]
        private static bool OnClickToUpgradeAbility_Prefix(UpgradeUIIcon __instance, ref bool __result, bool isRandomSelectionInput)
        {
            if (!OverhaulMod.IsModInitialized || isRandomSelectionInput)
                return true;

            UpgradeModesController modesController = OverhaulController.GetController<UpgradeModesController>();
            if (!modesController || UpgradeModesController.Mode == UpgradeMode.Upgrade)
                return true;

            __result = UpgradeManager.Instance.RevertUpgrade(__instance.GetDescription());
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch("refreshDisplay")]
        private static void refreshDisplay_Postfix(UpgradeUIIcon __instance)
        {
            CanvasGroup canvasGroup = __instance.GetComponent<CanvasGroup>();
            if (!canvasGroup)
                canvasGroup = __instance.gameObject.AddComponent<CanvasGroup>();

            bool isUpgradeMode = OverhaulMod.IsModInitialized && UpgradeModesController.Mode == UpgradeMode.Upgrade;
            if (!isUpgradeMode && !__instance.GetDescription().CanBeReverted())
            {
                canvasGroup.alpha = 0.3f;
                return;
            }
            canvasGroup.alpha = 1f;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        private static void Awake_Postfix(UpgradeUIIcon __instance)
        {
            Transform selectableFrame = __instance.transform.FindChildRecursive("SelectableFrame");
            if (selectableFrame)
            {
                selectableFrame.localPosition = new Vector3(1f, 2f, 0f);
                selectableFrame.localScale = new Vector3(1.2f, 1f, 1f);
            }
        }
    }
}