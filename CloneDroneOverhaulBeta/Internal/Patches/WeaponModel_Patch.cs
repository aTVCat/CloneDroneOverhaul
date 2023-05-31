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
            if (!OverhaulMod.IsModInitialized || __instance.MeleeImpactArea == null)
                return true;

            FirstPersonMover owner = __instance.MeleeImpactArea.Owner;
            if (!WeaponSkinsController.IsFirstPersonMoverSupported(owner))
                return true;

            OverhaulModdedPlayerInfo info = OverhaulModdedPlayerInfo.GetPlayerInfo(owner);
            if (info != null || !GameModeManager.IsMultiplayer())
            {
                Hashtable t = GameModeManager.IsMultiplayer() ? info.GetHashtable() : OverhaulModdedPlayerInfo.GenerateNewHashtable();
                if (t != null &&
                    t.Contains("Skin." + __instance.WeaponType) &&
                    !Equals(t["Skin." + __instance.WeaponType], "Default") &&
                    ((!owner.IsPlayer() &&
                    WeaponSkinsController.AllowEnemiesWearSkins) ||
                    owner.IsPlayer()))
                    return false;
            }
            return true;
        }
    }
}
