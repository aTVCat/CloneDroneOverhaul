using CDOverhaul.Gameplay;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(LevelManager))]
    internal static class LevelManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("parseLevelSpreadSheets")]
        private static void parseLevelSpreadSheets_Postfix()
        {
            if (OverhaulVersion.Upd2Hotfix)
            {
                return;
            }

            OverhaulLevelAdder.AddLevel("FUSRoom", "CombatTutorial", GameMode.Story, out _);
        }
    }
}
