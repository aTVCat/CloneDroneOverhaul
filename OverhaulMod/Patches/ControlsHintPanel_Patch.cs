using HarmonyLib;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ControlsHintPanel))]
    internal static class ControlsHintPanel_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ControlsHintPanel.Update))]
        private static bool Hide_Prefix(ControlsHintPanel __instance)
        {
            if (PhotoManager.Instance.IsInPhotoMode())
            {
                __instance.DefaultMountControlInstructions.SetActive(false);
                __instance.LaserInstructions.SetActive(false);
                return false;
            }
            return true;
        }
    }
}