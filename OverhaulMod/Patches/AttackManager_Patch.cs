﻿using HarmonyLib;
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

            ParticleManager.Instance.SpawnSparksParticles(position);
            return false;
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
