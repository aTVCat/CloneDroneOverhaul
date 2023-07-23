using CDOverhaul.Graphics;
using CDOverhaul.HUD;
using HarmonyLib;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(UpgradeUI))]
    internal static class UpgradeUI_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Show")]
        private static void Show_Postfix(UpgradeUI __instance, bool challengeUpgradeConfigMode = false, bool isStoryCutsceneMode = false, bool isMultiplayerSelection = false)
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

        
        [HarmonyPostfix]
        [HarmonyPatch("PopulateIcons")]
        private static void PopulateIcons_Postfix(UpgradeUI __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            Transform transform = __instance.transform.FindChildRecurisve("IconContainer");
            if (transform)
            {
                OverhaulUIPanelScaler scaler = transform.GetComponent<OverhaulUIPanelScaler>();
                if (scaler)
                {
                    scaler.OnEnable();
                }
            }

            /*
            foreach (UpgradeUIIcon icon in __instance.GetPrivateField<List<UpgradeUIIcon>>("_icons"))
            {
                if (icon.GetComponent<OverhaulUIAnchoredPanelSlider>())
                    continue;

                RectTransform rectTransform = icon.GetComponent<RectTransform>();
                Vector3 position = rectTransform.anchoredPosition;

                icon.gameObject.AddComponent<OverhaulUIAnchoredPanelSlider>().Initialize(Vector2.zero, position, 20f, 2);
                rectTransform.anchoredPosition = Vector2.zero;
            }*/
        }
    }
}
