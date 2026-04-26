using HarmonyLib;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ObjectPlacedInLevel))]
    internal static class ObjectPlacedInLevel_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ObjectPlacedInLevel.Initialize))]
        private static void Initialize_Prefix(ObjectPlacedInLevel __instance)
        {
            if (!ModCore.IsActive()) return;

            LevelObjectEntry levelObjectEntry = __instance.LevelObjectEntry;
            if (levelObjectEntry != null && (levelObjectEntry.PathUnderResources == RealisticLightingManager.LightSettingsObjectResourcePath || levelObjectEntry.PathUnderResources == RealisticLightingManager.LightSettingsOverrideObjectResourcePath) && !__instance.GetComponent<AdditionalSkyboxSettings>())
            {
                __instance.gameObject.AddComponent<AdditionalSkyboxSettings>();
            }
        }
    }
}