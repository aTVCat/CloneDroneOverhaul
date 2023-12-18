using HarmonyLib;
using OverhaulMod.Combat;

namespace OverhaulMod.Patches.Harmony
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
    }
}
