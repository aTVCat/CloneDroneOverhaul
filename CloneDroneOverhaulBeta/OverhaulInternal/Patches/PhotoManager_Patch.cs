using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(PhotoManager))]
    internal static class PhotoManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("TriggerPhotoModeOnOff")]
        private static bool TriggerPhotoModeOnOff_Prefix()
        {
            if (!OverhaulMod.IsCoreCreated)
            {
                return true;
            }

            if (EnableCursorController.HasToEnableCursor())
            {
                return false;
            }
            return true;
        }
    }
}
