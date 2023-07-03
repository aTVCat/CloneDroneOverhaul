using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.Graphics;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(UpgradeUIIcon))]
    internal static class UpgradeUIIcon_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnClickToUpgradeAbility")]
        private static bool OnClickToUpgradeAbility_Prefix(UpgradeUIIcon __instance, ref bool __result, bool isRandomSelectionInput)
        {
            if (!OverhaulMod.IsModInitialized || isRandomSelectionInput)
                return true;

            UpgradeModesController modesController = OverhaulController.GetController<UpgradeModesController>();
            if (!modesController || UpgradeModesController.Mode == UpgradeMode.Upgrade)
                return true;

            __result = UpgradeManager.Instance.RevertUpgrade(__instance.GetDescription());
            return false;
        }
    }
}