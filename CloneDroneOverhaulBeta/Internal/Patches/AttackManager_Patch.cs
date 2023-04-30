using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Combat;
using CDOverhaul.Graphics;
using HarmonyLib;
using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(AttackManager))]
    internal static class AttackManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("CreateSwordBlockVFX")]
        private static bool CreateSwordBlockVFX_Prefix(AmplifyColorSwapper __instance)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return true;
            }

            WeaponSkinItemDefinitionV2 def = OverhaulCombatState.SwordBlockAreaEnvCollisionSkinItem;
            if (WeaponSkinsCustomVFXController.SkinHasCustomVFX(def))
            {
                WeaponSkinsCustomVFXController.SpawnVFX(OverhaulCombatState.SwordBlockAreaEnvCollisionPosition, Vector3.zero, def);
                return false;
            }
            return true;
        }
    }
}
