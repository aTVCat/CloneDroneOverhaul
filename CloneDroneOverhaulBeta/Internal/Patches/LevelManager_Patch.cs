using CDOverhaul.Gameplay.Overmodes;
using HarmonyLib;
using System.Collections.Generic;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(LevelManager))]
    internal static class LevelManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("getLevelDescriptions")]
        private static bool getLevelDescriptions_Postfix(ref List<LevelDescription> __result)
        {
            if (OvermodesManager.IsOvermode())
            {
                __result = OvermodesManager.Instance.CurrentOvermode.GetLevelDescriptions();
                return false;
            }
            if (OverhaulFeaturesSystem.IsFeatureImplemented(EBuildFeatures.TitleScreen_Overhaul) && TitleScreenOverhaulManager.reference.customization.OverridesLevelWithWorkshop())
            {
                __result = WorkshopLevelManager.Instance._endlessWorkshopLevels;
            }
            return true;
        }
    }
}
