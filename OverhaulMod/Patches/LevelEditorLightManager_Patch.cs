using HarmonyLib;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(LevelEditorLightManager))]
    internal static class LevelEditorLightManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("AddLightSettingInScene")]
        private static bool AddLightSettingInScene_Prefix(LevelEditorLightManager __instance, LevelLightSettings lightSettings)
        {
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
            if (manager)
            {
                if (__instance.GetActiveLightSettings() == lightSettings)
                {
                    LightningTransitionManager.Instance.SetOldLightningInfo(lightSettings);
                }

                if (manager.IsDoingTransition())
                {
                    _ = __instance._lightSettingsInScene.Remove(lightSettings);
                    return false;
                }
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("RefreshLightInScene")]
        private static void RefreshLightInScene_Postfix(LevelEditorLightManager __instance, bool onlyRefreshForNewLightSettings = false)
        {
            LightningTransitionManager manager = LightningTransitionManager.Instance;
            if (manager && manager.ShouldDoTransition())
            {
                LevelLightSettings levelLightSettings = __instance.GetActiveLightSettings();
                if (levelLightSettings)
                {
                    LightningTransitionManager.Instance.DoTransition(levelLightSettings);
                }
            }
        }
    }
}
