using HarmonyLib;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(SkyBoxManager))]
    internal static class SkyBoxManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("RefreshSkyboxAmbientLightAndFog")]
        private static void RefreshSkyboxAmbientLightAndFog_Postfix(SkyBoxManager __instance)
        {
            if (GameModeManager.IsStoryChapter4())
            {
                RenderSettings.skybox = __instance.LevelConfigurableSkyboxes[7];
            }
        }
    }
}
