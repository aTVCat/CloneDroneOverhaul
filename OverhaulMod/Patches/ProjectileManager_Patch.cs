using HarmonyLib;
using OverhaulMod.Visuals;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ProjectileManager))]
    internal static class ProjectileManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ProjectileManager.CreateArrowGroundImpactVFX))]
        private static bool CreateArrowGroundImpactVFX_Prefix(Vector3 position)
        {
            if (!ParticleManager.EnableParticles)
                return true;

            ParticleManager.Instance.SpawnSwordBlockParticles(position);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(ProjectileManager.CreateMortarExplodeVFX))]
        private static bool CreateMortarExplodeVFX_Prefix(Vector3 position)
        {
            if (!ParticleManager.EnableParticles)
                return true;

            ParticleManager.Instance.SpawnRedGrenadeExplosionParticles(position);
            return false;
        }
    }
}
