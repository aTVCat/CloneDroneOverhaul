using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(GameModeManager))]
    internal static class GameModeManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("UsesWorkshopLevelsInEndlessMode")]
        private static void UsesWorkshopLevelsInEndlessMode_Postfix(ref bool __result)
        {
            if (OverhaulFeaturesSystem.IsFeatureImplemented(EBuildFeatures.TitleScreen_Overhaul))
            {
                __result = TitleScreenOverhaulManager.reference.customization.OverridesLevelWithWorkshop();
            }
        }
    }
}
