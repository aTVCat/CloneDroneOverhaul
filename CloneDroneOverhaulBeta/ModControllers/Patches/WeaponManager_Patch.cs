using CDOverhaul.Gameplay;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(WeaponManager))]
    internal static class WeaponManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetWeaponModelReplacementPrefab")]
        private static void GetWeaponModelReplacementPrefab_Postfix(ref Transform __result, WeaponType weaponType, bool isOnFire, bool isMultiplayer, bool isEMP)
        {/*
            WeaponSkinModels models = MainGameplayController.Instance.WeaponSkins.GetSkin(weaponType);
            if (models == null)
            {
                return;
            }
            else if (models.GetModel(isOnFire, isMultiplayer) != null)
            {
                __result = null;
            }*/
        }
    }
}
