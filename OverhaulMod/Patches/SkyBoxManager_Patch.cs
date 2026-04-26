using HarmonyLib;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(SkyBoxManager))]
    internal static class SkyBoxManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SkyBoxManager.RefreshSkyboxAmbientLightAndFog))]
        private static void RefreshSkyboxAmbientLightAndFog_Postfix(SkyBoxManager __instance, LevelLightSettings lightSettings)
        {
            if (!ModCore.IsActive()) return;

            if (GameModeManager.IsStoryChapter4()) RenderSettings.skybox = __instance.LevelConfigurableSkyboxes[7];

            AdditionalSkyboxSettings realisticLightSettings = lightSettings.GetComponent<AdditionalSkyboxSettings>();
            if (!realisticLightSettings || realisticLightSettings.Skybox.IsNullOrEmpty()) return;

            AdditionalSkyboxesManager manager = AdditionalSkyboxesManager.Instance;
            manager.SetSkybox(realisticLightSettings.Skybox);
            manager.SetTint(realisticLightSettings.Tint);
            manager.SetRotation(realisticLightSettings.Rotation);
        }
    }
}