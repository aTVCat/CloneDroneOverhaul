using HarmonyLib;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(UpgradeStation))]
    internal static class UpgradeStation_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(UpgradeStation.Start))]
        private static void Start_Prefix(UpgradeStation __instance)
        {
            RectTransform bgGlow = TransformUtils.FindChildRecursive(__instance.UpgradePressEPrefab.transform, "BGGlow") as RectTransform;
            if (bgGlow)
            {
                bgGlow.localPosition = Vector3.zero;
                bgGlow.sizeDelta = Vector3.one * 250f;

                Image image = bgGlow.GetComponent<Image>();
                if (image)
                {
                    image.sprite = ModResources.Sprite(AssetBundleConstants.UI, "Glow-2-256x256");
                    image.color = PressActionKeyObjectManager.BGGlowColor;
                }
            }
        }
    }
}
