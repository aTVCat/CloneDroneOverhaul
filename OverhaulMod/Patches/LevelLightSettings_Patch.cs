using HarmonyLib;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(LevelLightSettings))]
    internal static class LevelLightSettings_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(LevelLightSettings.Start))]
        private static void Start_Prefix(LevelLightSettings __instance)
        {
            RealisticLightingInfo realisticLightingInfo = RealisticLightingManager.Instance.GetCurrentRealisticLightingInfo();
            if (realisticLightingInfo != null && realisticLightingInfo.Lighting != null)
            {
                realisticLightingInfo.Lighting.ApplyValues(__instance);
            }
        }

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
