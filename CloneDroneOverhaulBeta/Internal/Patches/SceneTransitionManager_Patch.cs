using CDOverhaul.Graphics;
using HarmonyLib;
namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(SceneTransitionManager))]
    internal static class SceneTransitionManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("DisconnectAndExitToMainMenu")]
        private static bool DisconnectAndExitToMainMenu_Prefix()
        {
            if (!OverhaulTransitionController.IsNewTransitionEnabled)
                return true;

            OverhaulTransitionController.GoToMainMenu();
            return false;
        }
    }
}