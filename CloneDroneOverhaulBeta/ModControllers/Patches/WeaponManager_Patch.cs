using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(WeaponManager))]
    internal static class WeaponManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetWeaponModelReplacementPrefab")]
        private static void GetWeaponModelReplacementPrefab_Prefix(ref Transform __result, WeaponType weaponType, bool isOnFire, bool isMultiplayer, bool isEMP)
        {
            if (weaponType == WeaponType.Sword)
            {
                __result = null;
            }
        }
    }
}
