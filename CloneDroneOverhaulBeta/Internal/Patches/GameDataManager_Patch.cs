using CDOverhaul.Gameplay.Overmodes;
using CDOverhaul.HUD;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(GameDataManager))]
    internal static class GameDataManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetCurrentLevelID")]
        private static void GetCurrentLevelID_Postfix(GameDataManager __instance, ref string __result)
        {
            if (OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsFirstUseSetupUIEnabled && !ModSetupWindow.HasSetTheModUp && GameModeManager.IsOnTitleScreen())
                __result = "U6Bronze2";

            if (OvermodesController.IsOvermode())
                __result = OvermodesController.Instance.CurrentOvermode.GetCurrentLevelID();
        }

        [HarmonyPostfix]
        [HarmonyPatch("getCurrentGameData")]
        private static void getCurrentGameData_Postfix(GameDataManager __instance, ref GameData __result)
        {
            if (OvermodesController.IsOvermode())
                __result = OvermodesController.Instance.CurrentOvermode.GameModeData;
        }
    }
}
