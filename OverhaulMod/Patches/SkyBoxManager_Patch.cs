using HarmonyLib;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(SkyBoxManager))]
    internal static class SkyBoxManager_Patch
    {
        /*[HarmonyPrefix]
        [HarmonyPatch(nameof(SkyBoxManager.RefreshSkyboxAmbientLightAndFog))]
        private static void RefreshSkyboxAmbientLightAndFog_Prefix(SkyBoxManager __instance, LevelLightSettings lightSettings)
        {
            ModLevelManager modLevelManager = ModLevelManager.Instance;
            if (modLevelManager && lightSettings)
                modLevelManager.currentSkyBoxIndex = lightSettings.SkyboxIndex;
        }*/

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SkyBoxManager.RefreshSkyboxAmbientLightAndFog))]
        private static void RefreshSkyboxAmbientLightAndFog_Postfix(SkyBoxManager __instance, LevelLightSettings lightSettings)
        {
            if (GameModeManager.IsStoryChapter4())
                RenderSettings.skybox = __instance.LevelConfigurableSkyboxes[7];

            AdditionalSkyboxSettings realisticLightSettings = lightSettings.GetComponent<AdditionalSkyboxSettings>();
            if (!realisticLightSettings || realisticLightSettings.GetSkybox().IsNullOrEmpty())
                return;

            AdditionalSkyboxesManager.Instance.SetSkybox(realisticLightSettings.GetSkybox());
        }
    }
}
