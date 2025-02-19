﻿using HarmonyLib;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(LevelLightSettings))]
    internal static class LevelLightSettings_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(LevelLightSettings.onValueChangedFromAnimation))]
        private static void onValueChangedFromAnimation_Postfix(LevelLightSettings __instance)
        {
            if (GameModeManager.IsInLevelEditor())
                return;

            if (LevelEditorLightManager.Instance.GetActiveLightSettings() != __instance)
                return;

            AdvancedPhotoModeManager advancedPhotoModeManager = AdvancedPhotoModeManager.Instance;
            if (advancedPhotoModeManager)
            {
                advancedPhotoModeManager.OnLevelLightSettingsChanged(__instance);
            }
        }
    }
}
