using HarmonyLib;
using UnityEngine;
using CDOverhaul.Gameplay;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(WeaponManager))]
    internal static class WeaponManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetWeaponModelReplacementPrefab")]
        private static void GetWeaponModelReplacementPrefab_Postfix(ref Transform __result, WeaponType weaponType, bool isOnFire, bool isMultiplayer, bool isEMP)
        {
            WeaponSkinsController c = MainGameplayController.Instance.WeaponSkins;
            WeaponSkinModels models = c.GetSkin(weaponType);
            if (models == null)
            {
                return;
            }

            if (isOnFire)
            {
                if (isMultiplayer && models.Fire.Item2 != null)
                {
                    __result = null;
                }
                else if (!isMultiplayer && models.Fire.Item1 != null)
                {
                    __result = null;
                }
            }
            else
            {
                if (isMultiplayer && models.Normal.Item2 != null)
                {
                    __result = null;
                }
                else if (!isMultiplayer && models.Normal.Item1 != null)
                {
                    __result = null;
                }
            }
        }
    }
}
