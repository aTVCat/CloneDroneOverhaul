using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Combat;
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
            if (!OverhaulFeatureAvailabilitySystem.BuildImplements.IsCombatOverhaulEnabled || !OverhaulMod.IsModInitialized || GameModeManager.IsMultiplayer() || !__instance.IsFlying() || !__instance.GetOwner())
                return;

            Vector3 currentVelocity = __instance.GetPrivateField<Vector3>("_velocity");
            Vector3 newVelocity = new Vector3(currentVelocity.x, currentVelocity.y - 0.0175f, currentVelocity.z);
            __instance.SetPrivateField<Vector3>("_velocity", newVelocity);
            __instance.transform.eulerAngles += new Vector3(0.04f, 0f, 0f);
        }

        [HarmonyPrefix]
        [HarmonyPatch("StartFlying")]
        private static void StartFlying_Prefix(ArrowProjectile __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            __instance.VelocityMagnitude = !OverhaulFeatureAvailabilitySystem.BuildImplements.IsCombatOverhaulEnabled || GameModeManager.IsMultiplayer() ? 40f : 75f;
        }
    }
}
