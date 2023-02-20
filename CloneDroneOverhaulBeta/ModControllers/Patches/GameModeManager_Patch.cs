using CDOverhaul.Gameplay.Combat_Update;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(GameModeManager))]
    internal static class GameModeManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ShouldRestrictWeaponSpinningToWin")]
        private static void RefreshCursorEnabled_Postfix(ref bool __result)
        {
            if (OverhaulCombatController.AllowNewCombat)
            {
                __result = true;
            }
        }
    }
}
