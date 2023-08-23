using HarmonyLib;
using UnityEngine;
using System.Collections;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(WeaponModel))]
    internal static class WeaponModel_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ReplaceModelWithVariantMatching")]
        private static bool ReplaceModelWithVariantMatching_Postfix(WeaponModel __instance, bool isOnFire, bool isMultiplayer, Color weaponGlowColor, bool isEMP)
        {
            if (!OverhaulMod.IsModInitialized || !__instance.MeleeImpactArea)
                return true;

            FirstPersonMover owner = __instance.MeleeImpactArea.Owner;
            if (!WeaponSkinsController.IsFirstPersonMoverSupported(owner) || !OverhaulController.Get<WeaponSkinsController>())
                return true;

            OverhaulPlayerInfo info = OverhaulPlayerInfo.GetOverhaulPlayerInfo(owner);
            if (info == null)
                return true;

            Hashtable t = GameModeManager.IsMultiplayer() ? info.Hashtable : OverhaulPlayerInfo.CreateNewHashtable();
            if (t != null &&
                t.Contains("Skin." + __instance.WeaponType) &&
                !Equals(t["Skin." + __instance.WeaponType], "Default") &&
                ((!owner.IsPlayer() &&
                WeaponSkinsController.AllowEnemiesWearSkins) ||
                owner.IsPlayer()))
                return false;
            return true;
        }
    }
}
