using HarmonyLib;
using OverhaulMod.Visuals;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(SeveredVolumeGenerator))]
    internal static class SeveredVolumeGenerator_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SeveredVolumeGenerator.CreateVolumeCopy))]
        private static void CreateVolumeCopy_Postfix(SeveredBodyPart __result)
        {
            if (!ModCore.IsActive()) return;

            if (SeveredBodyPartSparks.EnableGarbageParticles && Random.value <= 0.7f)
                _ = __result.gameObject.AddComponent<SeveredBodyPartSparks>();

            if (!GameModeManager.TimeScaleChangesAllowed())
                return;

            Rigidbody rigidBody = __result.gameObject.GetComponent<Rigidbody>();
            if (rigidBody)
            {
                rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
                DisableRigidBodyInterpolation disableInterpolation = __result.gameObject.AddComponent<DisableRigidBodyInterpolation>();
                disableInterpolation.Initialize(rigidBody, 1f);
            }
        }
    }
}