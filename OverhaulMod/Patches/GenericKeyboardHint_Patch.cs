using HarmonyLib;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(GenericKeyboardHint))]
    internal static class GenericKeyboardHint_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GenericKeyboardHint.Show))]
        private static bool Show_Prefix(GenericKeyboardHint __instance)
        {
            Text t = __instance.DescriptionLabel;
            if (t && !t.text.IsNullOrEmpty())
                t.text = null;

            RectTransform bgGlow = TransformUtils.FindChildRecursive(__instance.transform, "BGGlow") as RectTransform;
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

            return false;
        }
    }
}
