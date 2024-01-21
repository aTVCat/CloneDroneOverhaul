using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(MultiplayerButtonController))]
    internal static class MultiplayerButtonController_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("tryStartMultiplayer")]
        private static bool tryStartMultiplayer_Prefix()
        {
            GameUIRoot.Instance.TitleScreenUI.OnMultiplayerButtonClicked();
            return false;
        }
    }
}
