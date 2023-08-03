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
            bool isEverythingFine = __instance.SwordHitArea && __instance.SwordHitArea.IsDamageActive();
            if (OverhaulMod.IsModInitialized && isEverythingFine && __instance.BounceWeaponOnEnvironmentImpact && !otherCollider.isTrigger && Tags.IsEnvironment(otherCollider.tag))
            {
                OverhaulCombatState.SwordBlockAreaEnvCollisionSkinItem = WeaponSkinsWearer.GetEquippedWeaponSkinItemDirectly(__instance.GetOwner());
                OverhaulCombatState.SwordBlockAreaEnvCollisionPosition = __instance.SwordHitArea.GetEdgePointCenter();
                OverhaulCombatState.SwordBlockAreaCollidedWithEnvironment = __instance;
            }

            if (!GameModeManager.IsMultiplayer())
                return;

            HammerImpactMeleeArea otherComponent = otherCollider.transform.GetComponent<HammerImpactMeleeArea>();
            if (isEverythingFine && otherComponent && otherComponent.DamageSourceType == DamageSourceType.Hammer)
            {
                FirstPersonMover owner = __instance.GetOwner();
                if (!owner)
                    return;

                Vector3 midPos = (otherComponent.transform.position + __instance.transform.position) / 2f;
                AttackManager.Instance.CreateSwordBlockVFX(midPos);
                _ = AudioManager.Instance.PlayClipAtPosition(AudioLibrary.Instance.SwordBlocks, midPos, 0f, false, 1f, Random.Range(0.95f, 1.05f));

                owner.OnWeaponCollidedWithEnvironment();
            }
        }
    }
}
