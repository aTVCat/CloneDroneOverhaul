using CDOverhaul.HUD;
using CDOverhaul.Visuals;
using HarmonyLib;
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

            __instance.UpgradeUIBackground.gameObject.SetActive(challengeUpgradeConfigMode || ViewModesManager.IsFirstPersonModeEnabled);
            int i = 0;
            do
            {
                __instance.UpgradeUIBackground.transform.GetChild(i).gameObject.SetActive(challengeUpgradeConfigMode);
                i++;
            } while (i < __instance.UpgradeUIBackground.transform.childCount);

            Transform hexBG = __instance.transform.FindChildRecursive("HexBG");
            if (hexBG)
            {
                hexBG.localPosition = new Vector3(-5.5f, 9.5f, 0f);
                hexBG.localScale = Vector3.one * 1.2f;
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch("PopulateIcons")]
        private static void PopulateIcons_Postfix(UpgradeUI __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            Transform transform = __instance.transform.FindChildRecursive("IconContainer");
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
