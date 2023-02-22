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
            /*
            if (__instance.WeaponType == WeaponType.Sword)
            {
                WeaponSkinsController c = MainGameplayController.Instance.WeaponSkins;
                c.RefreshSkins(__instance.MeleeImpactArea.Owner);
            }*/
        }

        [HarmonyPrefix]
        [HarmonyPatch("DropWeapon")]
        private static bool DropWeapon_Prefix(WeaponModel __instance)
        {
            return true;
        }
    }
}
