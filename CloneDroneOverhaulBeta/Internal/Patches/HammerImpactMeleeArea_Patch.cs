using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(HammerImpactMeleeArea))]
    internal static class HammerImpactMeleeArea_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnTriggerEnter")]
        private static void OnTriggerEnterPostfix(HammerImpactMeleeArea __instance, Collider otherCollider)
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsCombatOverhaulEnabled || !OverhaulMod.IsModInitialized)
                return;

            if (GameModeManager.IsMultiplayer() || !__instance.Owner || !__instance.IsDamageActive())
                return;

            HammerImpactMeleeArea otherComponent = otherCollider.transform?.GetComponent<HammerImpactMeleeArea>();
            if (otherComponent && otherComponent.DamageSourceType == DamageSourceType.Hammer)
            {
                Vector3 midPos = (otherComponent.transform.position + __instance.transform.position) / 2f;
                AttackManager.Instance.CreateSwordBlockVFX(midPos);
                _ = AudioManager.Instance.PlayClipAtPosition(AudioLibrary.Instance.HammerImpacts, midPos, 0f, false, 1f, Random.Range(1.300f, 1.450f));

                __instance.Owner.OnWeaponCollidedWithEnvironment();
                if (otherComponent.Owner) 
                    otherComponent.Owner.OnWeaponCollidedWithEnvironment();
            }

            SwordBlockArea swordBlockArea = otherCollider?.transform?.GetComponent<SwordBlockArea>();
            if (swordBlockArea)
            {
                FirstPersonMover owner = swordBlockArea.GetOwner();
                if (owner != __instance.Owner && swordBlockArea.SwordHitArea)
                {
                    owner.OnSwordHitSword(__instance.Owner, null, false);

                    Vector3 midPos = (swordBlockArea.transform.position + __instance.transform.position) / 2f;
                    AttackManager.Instance.CreateSwordBlockVFX(midPos);
                    _ = AudioManager.Instance.PlayClipAtPosition(AudioLibrary.Instance.SwordHitShield, midPos, 0f, false, 1f, Random.Range(0.925f, 1.075f));
                }
            }
        }
    }
}
