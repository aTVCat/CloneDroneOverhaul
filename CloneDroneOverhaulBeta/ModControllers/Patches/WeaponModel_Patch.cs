using CDOverhaul.Gameplay;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(WeaponModel))]
    internal static class WeaponModel_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("DropWeapon")]
        private static bool DropWeapon_Prefix(WeaponModel __instance)
        {
            return true;
        }
    }
}
