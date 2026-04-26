using HarmonyLib;
using ModLibrary;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(UpgradeDescription))]
    internal static class UpgradeDescription_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(UpgradeDescription.IsUpgradeCurrentlyVisible))]
        private static void IsUpgradeCurrentlyVisible_Postfix(UpgradeDescription __instance, ref bool __result)
        {
            if (!ModCore.IsActive()) return;

            AutoBuildManager autoBuildManager = AutoBuildManager.Instance;
            if (autoBuildManager && autoBuildManager.IsInAutoBuildConfigurationMode)
            {
                __result = __instance.IsAvailableInBattleRoyale && !__instance.IsModdedUpgradeType();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(UpgradeDescription.GetSkillPointCost))]
        private static void GetSkillPointCost_Postfix(UpgradeDescription __instance, ref int __result)
        {
            if (!ModCore.IsActive()) return;

            AutoBuildManager autoBuildManager = AutoBuildManager.Instance;
            if (autoBuildManager && autoBuildManager.IsInAutoBuildConfigurationMode)
            {
                if (__instance.SkillPointCostBattleRoyale > 0)
                    __result = __instance.SkillPointCostBattleRoyale;
                else if (__instance.SkillPointCostMultiplayer > 0)
                    __result = __instance.SkillPointCostMultiplayer;
                else
                    __result = __instance.SkillPointCostDefault;
            }
        }
    }
}
