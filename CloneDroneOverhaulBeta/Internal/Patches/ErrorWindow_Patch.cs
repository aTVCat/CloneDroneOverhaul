using CDOverhaul.HUD;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(EscMenu))]
    internal static class EscMenu_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Show")]
        private static bool Show_Prefix()
        {
            if (!OverhaulMod.IsModInitialized)
                return true;

            OverhaulUIManager uiManager = OverhaulUIManager.reference;
            if (uiManager && UIPauseMenu.UseThisMenu && !UIPauseMenu.ForceUseOldMenu)
            {
                UIPauseMenu pauseMenu = uiManager.GetUI<UIPauseMenu>(UIConstants.UI_PAUSE_MENU);
                if (!pauseMenu)
                {
                    UIConstants.ShowPauseScreen();
                    pauseMenu = uiManager.GetUI<UIPauseMenu>(UIConstants.UI_PAUSE_MENU);
                    if (!pauseMenu)
                        return true;
                }
                else
                {
                    if (!pauseMenu.gameObject.activeSelf)
                    {
                        pauseMenu.Show();
                        return false;
                    }
                    pauseMenu.Hide();
                }
                return false;
            }
            return true;
        }
    }
}
