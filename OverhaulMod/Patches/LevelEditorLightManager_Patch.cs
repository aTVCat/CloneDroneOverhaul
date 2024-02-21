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

        /*
        [HarmonyPrefix]
        [HarmonyPatch("RefreshLightInScene")]
        private static void RefreshLightInScene_Prefix(LevelEditorLightManager __instance, bool onlyRefreshForNewLightSettings = false)
        {
        }*/

        [HarmonyPostfix]
        [HarmonyPatch("RefreshLightInScene")]
        private static void RefreshLightInScene_Postfix(LevelEditorLightManager __instance, bool onlyRefreshForNewLightSettings = false)
        {
            LevelLightSettings levelLightSettings = __instance.GetActiveLightSettings();
            if (levelLightSettings)
            {
                RealisticLightningManager.Instance.PatchLightning(false, levelLightSettings);
                LightningTransitionManager manager = LightningTransitionManager.Instance;
                if (manager && manager.ShouldDoTransition())
                    LightningTransitionManager.Instance.DoTransition(levelLightSettings);
            }
        }
    }
}
