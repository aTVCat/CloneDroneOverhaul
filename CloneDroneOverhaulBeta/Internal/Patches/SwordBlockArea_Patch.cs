using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Combat;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(SwordBlockArea))]
    internal static class SwordBlockArea_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnTriggerEnter")]
        private static void OnTriggerEnter_Prefix(SwordBlockArea __instance, Collider otherCollider)
        {
            if (__instance.SwordHitArea != null && __instance.SwordHitArea.IsDamageActive() && __instance.BounceWeaponOnEnvironmentImpact && !otherCollider.isTrigger && Tags.IsEnvironment(otherCollider.tag))
            {
                OverhaulCombatState.SwordBlockAreaEnvCollisionSkinItem = WeaponSkinsWearer.GetEquippedWeaponSkinItemDirectly(__instance.GetOwner());
                OverhaulCombatState.SwordBlockAreaEnvCollisionPosition = __instance.SwordHitArea.GetEdgePointCenter();
                OverhaulCombatState.SwordBlockAreaCollidedWithEnvironment = __instance;
            }
        }
    }
}
