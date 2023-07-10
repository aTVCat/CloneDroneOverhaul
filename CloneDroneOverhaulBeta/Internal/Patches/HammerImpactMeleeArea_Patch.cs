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

            if (GameModeManager.IsMultiplayer() || !__instance.Owner)
                return;

            HammerImpactMeleeArea otherComponent = otherCollider.transform.GetComponent<HammerImpactMeleeArea>();
            if (!otherComponent)
                return;

            __instance.Owner.OnWeaponCollidedWithEnvironment();
            if (otherComponent.Owner) otherComponent.Owner.OnWeaponCollidedWithEnvironment();
        }
    }
}
