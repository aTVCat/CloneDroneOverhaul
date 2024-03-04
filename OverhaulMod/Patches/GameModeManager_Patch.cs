using HarmonyLib;
using OverhaulMod.Combat;
using OverhaulMod.Engine;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(GameModeManager))]
    internal static class GameModeManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("UsesMultiplayerSpeedMultiplier")]
        private static void UsesMultiplayerSpeedMultiplier_Postfix(ref bool __result)
        {
            if (GameModeManager.Is(GameMode.Story))
            {
                __result = ModGameModifiersManager.Instance.forceEnableGreatSwords && LevelManager.Instance.GetCurrentLevelID() != "StoryC5_5";
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("CanLevelsModifyTimeScale")]
        private static void CanLevelsModifyTimeScale_Postfix(ref bool __result)
        {
            if (!__result && GameModeManager.IsLevelPlaytest())
            {
                __result = true;
            }
        }
    }
}
