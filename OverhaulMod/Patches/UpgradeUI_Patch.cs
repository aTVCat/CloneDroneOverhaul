using HarmonyLib;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(UpgradeUI))]
    internal static class UpgradeUI_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(UpgradeUI.OnResetButtonClicked))]
        private static bool OnResetButtonClicked_Prefix(UpgradeUI __instance)
        {
            AutoBuildManager autoBuildManager = AutoBuildManager.Instance;
            if (autoBuildManager && autoBuildManager.isInAutoBuildConfigurationMode)
            {
                autoBuildManager.ResetUpgrades();
                return false;
            }
            return true;
        }
    }
}
