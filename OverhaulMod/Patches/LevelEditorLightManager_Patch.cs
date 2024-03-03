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
            RealisticLightningManager.Instance.PatchLightning(false, lightSettings, false);

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

            levelLightSettings.RestrictLightSettingsVariables();
            __instance._selectedLightSettings = levelLightSettings;

            if (!onlyRefreshForNewLightSettings)
            {
                DirectionalLightManager.Instance.RefreshDirectionalLight(__instance._selectedLightSettings);
                SkyBoxManager.Instance.RefreshSkyboxAmbientLightAndFog(__instance._selectedLightSettings);
            }
            else if (oldLevelLightSettings != levelLightSettings)
            {
                RealisticLightningManager.Instance.PatchLightning(false, levelLightSettings);
                LightningTransitionManager manager = LightningTransitionManager.Instance;
                if (manager)
                    manager.DoTransition(levelLightSettings);
            }

            GlobalEventManager.Instance.Dispatch(GlobalEvents.LightSettingsRefreshed);
            PostEffectsManager.Instance.RefreshCameraPostEffects();
            return false;
        }

        /*
        [HarmonyPostfix]
        [HarmonyPatch("RefreshLightInScene")]
        private static void RefreshLightInScene_Postfix(LevelEditorLightManager __instance, bool onlyRefreshForNewLightSettings = false)
        {
            LevelLightSettings levelLightSettings = __instance.GetActiveLightSettings();
            if (levelLightSettings && !levelLightSettings.IsOverrideSettings)
            {
                RealisticLightningManager.Instance.PatchLightning(false, levelLightSettings);
                LightningTransitionManager manager = LightningTransitionManager.Instance;
                if (manager)
                    manager.DoTransition(levelLightSettings);
            }
        }*/
    }
}
