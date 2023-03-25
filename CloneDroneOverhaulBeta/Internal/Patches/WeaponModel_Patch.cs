using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(WeaponModel))]
    internal static class WeaponModel_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ReplaceModelWithVariantMatching")]
        private static bool ReplaceModelWithVariantMatching_Postfix(WeaponModel __instance, bool isOnFire, bool isMultiplayer, Color weaponGlowColor, bool isEMP)
        {            
            if (!OverhaulMod.IsCoreCreated)
            {
                return true;
            }

            if(isMultiplayer && __instance.WeaponType.Equals(WeaponType.Spear))
            {
                return false;
            }

            return true;
        }
    }
}
