using HarmonyLib;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(UpgradeUIIcon))]
    internal static class UpgradeUIIcon_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnClickToUpgradeAbility")]
        private static bool OnClickToUpgradeAbility_Prefix(UpgradeUIIcon __instance, ref bool __result, bool isRandomSelectionInput)
        {
            if (isRandomSelectionInput)
                return true;

            if (UpgradeModeManager.Mode == UpgradeMode.Upgrade)
                return true;

            __result = UpgradeManager.Instance.RevertUpgrade(__instance.GetDescription());
            return false;
        }

        /*
        [HarmonyPostfix]
        [HarmonyPatch("SetScaleToMouseOver")]
        private static void SetScaleToMouseOver_Postfix(UpgradeUIIcon __instance)
        {
            __instance.transform.localScale = Vector3.one;
        }*/

        [HarmonyPostfix]
        [HarmonyPatch("refreshDisplay")]
        private static void refreshDisplay_Postfix(UpgradeUIIcon __instance)
        {
            CanvasGroup canvasGroup = __instance.GetComponent<CanvasGroup>();
            if (!canvasGroup)
                canvasGroup = __instance.gameObject.AddComponent<CanvasGroup>();

            bool isUpgradeMode = UpgradeModeManager.Mode == UpgradeMode.Upgrade;
            canvasGroup.alpha = !isUpgradeMode && !__instance.GetDescription().CanBeReverted() ? 0.3f : 1f;

            if (!__instance._upgradeDescription || !__instance._upgradeDescription.Icon)
            {
                RectTransform content = __instance.transform.FindChildRecursive("Content") as RectTransform;
                if (content)
                {
                    Image image = content.GetComponent<Image>();
                    if (image)
                    {
                        //image.sprite = OverhaulAssetsContainer.HQQuestionSprite;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        private static void Awake_Postfix(UpgradeUIIcon __instance)
        {
            SelectableUI selectableUI = __instance.GetComponent<SelectableUI>();
            if (selectableUI)
            {
                selectableUI.enabled = false;
            }

            Transform selectableFrame = __instance.transform.FindChildRecursive("SelectableFrame");
            if (selectableFrame)
            {
                selectableFrame.localPosition = new Vector3(1f, 2f, 0f);
                selectableFrame.localScale = new Vector3(1.2f, 1f, 1f);
            }

            Transform bgFillTransform = __instance.transform.FindChildRecursive("BGFill");
            if (bgFillTransform)
            {
                UIColorSwapper bgFill = bgFillTransform.GetComponent<UIColorSwapper>();
                if (bgFill)
                {
                    bgFill.ColorVariants[0] = new Color(0f, 0.33f, 0.475f, 0.7f);
                }
            }

            /*
            Transform repetitionsLabelTransform = __instance.transform.FindChildRecursive("repetitionsLabel");
            if (repetitionsLabelTransform && OverhaulAssetsContainer.TriggeringFanFaresFont)
            {
                Text repetitionsLabel = repetitionsLabelTransform.GetComponent<Text>();
                if (repetitionsLabel)
                {
                    repetitionsLabel.font = OverhaulAssetsContainer.TriggeringFanFaresFont;
                    repetitionsLabel.fontSize = 13;
                }
            }*/

            /*
            RectTransform additionalCostBGTransform = __instance.transform.FindRectChildRecursive("AdditionalCostBG");
            if (additionalCostBGTransform && OverhaulAssetsContainer.TriggeringFanFaresFont)
            {
                Canvas canvas = additionalCostBGTransform.gameObject.AddComponent<Canvas>();
                CanvasParametersUpdater updater = canvas.gameObject.AddComponent<CanvasParametersUpdater>();
                updater.OverrideSorting = true;
                updater.SortingOrder = 100;

                additionalCostBGTransform.SetSiblingIndex(additionalCostBGTransform.GetSiblingIndex() + 1);
                additionalCostBGTransform.sizeDelta = Vector2.one * 15f;
                additionalCostBGTransform.localPosition = Vector3.one * 13.5f;

                RectTransform label = additionalCostBGTransform.FindRectChildRecursive("CostIconLabel");
                if (label)
                {
                    label.anchorMax = new Vector2(1f, 1f);
                    label.anchorMin = new Vector2(0.15f, 0f);
                    label.sizeDelta = Vector2.zero;

                    label.GetComponent<Text>().font = OverhaulAssetsContainer.TriggeringFanFaresFont;
                    _ = label.gameObject.AddComponent<BetterOutline>();
                }
            }*/

            RectTransform content = __instance.transform.FindChildRecursive("Content") as RectTransform;
            if (content)
            {
                content.sizeDelta = Vector2.one * -16f;

                UIColorSwapper contentColors = content.GetComponent<UIColorSwapper>();
                if (contentColors)
                {
                    contentColors.ColorVariants[0] = new Color(0f, 1f, 1f, 1f);
                }

                Shadow shadow = content.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0f, 0f, 0f, 0.225f);
                shadow.effectDistance = Vector2.one * -1.5f;
            }
        }
    }
}
