using CDOverhaul.Gameplay;
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
            if (!OverhaulMod.IsModInitialized)
            {
                return true;
            }

            if (__instance.MeleeImpactArea != null)
            {
                FirstPersonMover owner = __instance.MeleeImpactArea.Owner;
                if (owner != null)
                {
                    bool isSP = GameModeManager.IsSinglePlayer() && (owner.IsPlayer() || WeaponSkinsController.AllowEnemiesWearSkins);
                    OverhaulModdedPlayerInfo info = OverhaulModdedPlayerInfo.GetPlayerInfo(owner);
                    if (info != null || isSP)
                    {
                        Hashtable t = isSP ? OverhaulModdedPlayerInfo.GenerateNewHashtable() : info.GetHashtable();
                        if (t != null && t.Contains("Skin." + __instance.WeaponType) && !Equals(t["Skin." + __instance.WeaponType], "Default"))
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
