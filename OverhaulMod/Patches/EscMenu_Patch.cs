using HarmonyLib;
using OverhaulMod.UI;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(EscMenu))]
    internal static class EscMenu_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Show")]
        private static bool Show_Prefix()
        {
            if (GameModeManager.Is((GameMode)2500))
                return false;

            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.PauseMenuRework))
                return true;

            if (UIPauseMenuRework.disableOverhauledVersion)
                return true;

            ModUIConstants.ShowPauseMenuRework();
            return false;
        }
    }
}
