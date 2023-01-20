using CDOverhaul.Gameplay;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(WeaponModel))]
    internal static class WeaponModel_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ReplaceModelWithVariantMatching")]
        private static void ReplaceModelWithVariantMatching_Postfix(WeaponModel __instance, bool isOnFire, bool isMultiplayer, Color weaponGlowColor, bool isEMP)
        {
            if (__instance.WeaponType == WeaponType.Sword)
            {
                MainGameplayController.Instance.WeaponSkins.GetAndSpawnSkin(__instance, "DetailedOne", __instance.MeleeImpactArea.Owner, isMultiplayer, isOnFire);
            }
        }
    }
}
