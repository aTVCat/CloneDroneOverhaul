using HarmonyLib;
using OverhaulMod.Engine;
using OverhaulMod.Visuals;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(LevelEditorLightManager))]
    internal static class LevelEditorLightManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("AddLightSettingInScene")]
        private static bool AddLightSettingInScene_Prefix(LevelEditorLightManager __instance, LevelLightSettings lightSettings)
        {
            RealisticLightningManager.Instance.PatchLevelLightSettings(lightSettings);

            LightningTransitionManager manager = LightningTransitionManager.Instance;
            if (manager && manager.IsDoingTransition())
            {
                __instance._lightSettingsInScene.Add(lightSettings);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("RemoveLightSettingInScene")]
        private static bool RemoveLightSettingInScene_Prefix(LevelEditorLightManager __instance, LevelLightSettings lightSettings)
        {
            LightningTransitionManager manager = LightningTransitionManager.Instance;
            if (manager && manager.IsDoingTransition())
            {
                _ = __instance._lightSettingsInScene.Remove(lightSettings);
                return false;
            }
            return true;
        }

        
        [HarmonyPrefix]
        [HarmonyPatch("RefreshLightInScene")]
        private static bool RefreshLightInScene_Prefix(LevelEditorLightManager __instance, bool onlyRefreshForNewLightSettings = false)
        {
            LevelLightSettings oldLevelLightSettings = __instance._selectedLightSettings;
            LevelLightSettings levelLightSettings = __instance.refreshActiveLightSettings();
            if (!levelLightSettings)
                return false;

            if(oldLevelLightSettings != levelLightSettings)
            {
                AdvancedPhotoModeManager advancedPhotoModeManager = AdvancedPhotoModeManager.Instance;
                if (advancedPhotoModeManager && advancedPhotoModeManager.IsInPhotoMode())
                {
                    advancedPhotoModeManager.OnLevelLightSettingsChanged(levelLightSettings);
                }
            }

            if (!LightningTransitionManager.TransitionsEnabled)
                return true;

            levelLightSettings.RestrictLightSettingsVariables();
            __instance._selectedLightSettings = levelLightSettings;

            if (!onlyRefreshForNewLightSettings)
            {
                DirectionalLightManager.Instance.RefreshDirectionalLight(__instance._selectedLightSettings);
                SkyBoxManager.Instance.RefreshSkyboxAmbientLightAndFog(__instance._selectedLightSettings);
            }
            else if (oldLevelLightSettings != levelLightSettings)
            {
                LightningTransitionManager manager = LightningTransitionManager.Instance;
                if (manager)
                    manager.DoTransition(levelLightSettings);
            }

            GlobalEventManager.Instance.Dispatch(GlobalEvents.LightSettingsRefreshed);
            return false;
        }        
        
        [HarmonyPostfix]
        [HarmonyPatch("RefreshLightInScene")]
        private static void RefreshLightInScene_Postfix(LevelEditorLightManager __instance, bool onlyRefreshForNewLightSettings = false)
        {
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }
    }
}
