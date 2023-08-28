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
            OverhaulTransitionController.GoToMainMenu();
            return false;
        }
    }
}