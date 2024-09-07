using HarmonyLib;
using OverhaulMod.Visuals;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ExplodeWhenCut))]
    internal static class ExplodeWhenCut_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ExplodeWhenCut.onBodyPartDamaged))]
        private static void CreateEmperorHeadExplosionVFX_Prefix(ExplodeWhenCut __instance)
        {
            if (!ParticleManager.EnableParticles || __instance._hasExploded)
                return;

            ParticleManager.Instance.SpawnLogoExplosionParticles(__instance.transform.position);
        }
    }
}
