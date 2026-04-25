using HarmonyLib;
using OverhaulMod.Combat;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(GameModeManager))]
    internal static class GameModeManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameModeManager.UsesMultiplayerSpeedMultiplier))]
        private static void UsesMultiplayerSpeedMultiplier_Postfix(ref bool __result)
        {
            if (GameModeManager.Is(GameMode.Story) && LevelManager.Instance.GetCurrentLevelID() != "StoryC5_5")
            {
                __result = ModGameModifiersManager.Instance.ForceEnableGreatSwords;
            }
        }
    }
}