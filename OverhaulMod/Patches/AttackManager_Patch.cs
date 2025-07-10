using HarmonyLib;
using OverhaulMod.Visuals;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(AttackManager))]
    internal static class AttackManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AttackManager.CreateSwordBlockVFX))]
        private static bool CreateSwordBlockVFX_Prefix(Vector3 position)
        {
            if (!ParticleManager.EnableParticles)
                return true;

            ParticleManager.Instance.SpawnSwordBlockParticles(position);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(AttackManager.CreateEmperorWeaponEnvironmentImpactVFX))]
        private static bool CreateEmperorWeaponEnvironmentImpactVFX_Prefix(Vector3 position)
        {
            if (!ParticleManager.EnableParticles)
                return true;

            ParticleManager.Instance.SpawnBlueGrenadeExplosionParticles(position);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(AttackManager.CreateHammerHitEffectVFX))]
        private static void CreateHammerHitEffectVFX_Prefix(Vector3 position)
        {
            if (!ParticleManager.EnableParticles)
                return;

            ParticleManager.Instance.SpawnHammerHitParticles(position);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(AttackManager.CreateEmperorHeadExplosionVFX))]
        private static void CreateEmperorHeadExplosionVFX_Postfix(Vector3 position)
        {
            if (!ParticleManager.EnableParticles || !ParticleManager.NewExplosionParticles)
                return;

            ParticleManager.Instance.SpawnLogoExplosionParticles(position);
        }
    }
}
