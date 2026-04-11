using HarmonyLib;
using OverhaulMod.Visuals;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(MultiplayerSpawnPoint))]
    internal static class MultiplayerSpawnPoint_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MultiplayerSpawnPoint.OnEnable))]
        private static void OnEnable_Postfix(MultiplayerSpawnPoint __instance)
        {
            if (!ParticleManager.ReworkWeldingParticles) return;

            ConstructionPropsParticleReplacer constructionPropsParticleReplacer = __instance.GetComponent<ConstructionPropsParticleReplacer>();
            if (constructionPropsParticleReplacer)
            {
                constructionPropsParticleReplacer.RefreshParticles();
            }
            else
            {
                constructionPropsParticleReplacer = __instance.gameObject.AddComponent<ConstructionPropsParticleReplacer>();
                constructionPropsParticleReplacer.IsOutsideArena = true;
                constructionPropsParticleReplacer.IsMultiplayerSpawnPoint = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MultiplayerSpawnPoint.PlayConstructionSequenceForPlayer))]
        private static void PlayConstructionSequenceForPlayer_Postfix(MultiplayerSpawnPoint __instance)
        {
            if (!ParticleManager.ReworkWeldingParticles) return;

            ConstructionPropsParticleReplacer constructionPropsParticleReplacer = __instance.GetComponent<ConstructionPropsParticleReplacer>();
            if (constructionPropsParticleReplacer)
            {
                constructionPropsParticleReplacer.RefreshParticles();
            }
            else
            {
                constructionPropsParticleReplacer = __instance.gameObject.AddComponent<ConstructionPropsParticleReplacer>();
                constructionPropsParticleReplacer.IsOutsideArena = true;
                constructionPropsParticleReplacer.IsMultiplayerSpawnPoint = true;
            }
        }
    }
}