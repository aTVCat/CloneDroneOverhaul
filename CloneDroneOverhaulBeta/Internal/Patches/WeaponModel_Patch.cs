using CDOverhaul.Gameplay.Multiplayer;
using HarmonyLib;
using System.Collections;
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

            if(isMultiplayer && __instance.WeaponType.Equals(WeaponType.Spear) && __instance.MeleeImpactArea != null)
            {
                FirstPersonMover owner = __instance.MeleeImpactArea.Owner;
                if(owner != null)
                {
                    OverhaulModdedPlayerInfo info = OverhaulModdedPlayerInfo.GetPlayerInfo(owner);
                    if(info != null)
                    {
                        Hashtable t = info.GetHashtable();
                        if(t != null && t.Contains("Skin.Spear") && !t["Skin.Spear"].Equals("Default"))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
