using CDOverhaul.Gameplay;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(SettingsManager))]
    internal static class SettingsManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetMultiplayerFavColorIndex")]
        private static void GetMultiplayerFavColorIndex_Postfix(ref int __result)
        {
            __result += ExclusivityController.ColorOffset;
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetMultiplayerFavColorIndex")]
        private static void SetMultiplayerFavColorIndex_Prefix(SettingsManager __instance, ref int index)
        {
            int orig = index;
            index = Mathf.Clamp(index, 0, HumanFactsManager.Instance.FavouriteColors.Length - 1);
            SettingInfo.SavePref(SettingsController.GetSetting("Player.VanillaAdditions.FavColorOffset", true), orig - index);
        }
    }
}
