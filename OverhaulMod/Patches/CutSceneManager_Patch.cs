using HarmonyLib;
using OverhaulMod.Visuals;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(CutSceneManager))]
    internal static class CutSceneManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(CutSceneManager.StartConstructionSequence))]
        private static void StartConstructionSequence_Postfix(FirstPersonMover player, bool isBusinessConstruction = false)
        {
            CharacterModel characterModel = player ? player.GetCharacterModel() : null;
            ConstructionProps constructionProps = characterModel ? characterModel.ConstructionProps : null;
            if (!constructionProps) return;

            Transform transform = constructionProps.transform;
            if (!transform) return;

            ConstructionPropsParticleReplacer constructionPropsParticleReplacer = transform.GetComponent<ConstructionPropsParticleReplacer>();
            if (constructionPropsParticleReplacer)
            {
                constructionPropsParticleReplacer.RefreshParticles();
            }
            else
            {
                constructionPropsParticleReplacer = transform.gameObject.AddComponent<ConstructionPropsParticleReplacer>();
                constructionPropsParticleReplacer.IsOutsideArena = isBusinessConstruction;
            }
        }
    }
}