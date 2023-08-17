using HarmonyLib;
using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ArrowProjectile))]
    internal static class ArrowProjectile_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        private static void FixedUpdate_Postfix(ArrowProjectile __instance)
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsCombatOverhaulEnabled || !OverhaulMod.IsModInitialized || GameModeManager.IsMultiplayer() || !__instance.IsFlying() || !__instance.GetOwner())
                return;

            Vector3 currentVelocity = __instance._velocity;
            currentVelocity.y -= 0.0175f;
            __instance._velocity = currentVelocity;
            __instance.transform.eulerAngles += new Vector3(0.04f, 0f, 0f);
        }

        [HarmonyPrefix]
        [HarmonyPatch("StartFlying")]
        private static void StartFlying_Prefix(ArrowProjectile __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            __instance.VelocityMagnitude = !OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsCombatOverhaulEnabled || GameModeManager.IsMultiplayer() ? 40f : 75f;
        }
    }
}
