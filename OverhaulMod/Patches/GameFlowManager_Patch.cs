using HarmonyLib;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(GameFlowManager))]
    internal static class GameFlowManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameFlowManager.ShowTitleScreen))]
        private static void ShowTitleScreen_Postfix(GameFlowManager __instance)
        {
            if (!ModCore.IsActive()) return;

            if (ModManagers.ShowModSetupScreenOnStart)
            {
                ModActionUtils.DoInFrames(delegate
                {
                    _ = ModUIConstants.ShowSettingsMenuRework(true);
                }, 10);
            }
            else
                ModUIUtils.ShowNewUpdateMessageOrChangelog(2f);
        }
    }
}
