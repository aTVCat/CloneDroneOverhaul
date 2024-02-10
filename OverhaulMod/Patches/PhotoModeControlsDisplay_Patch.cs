using HarmonyLib;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(PhotoModeControlsDisplay))]
    internal static class PhotoModeControlsDisplay_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetVisibility")]
        private static void SetVisibility_Postfix(PhotoModeControlsDisplay __instance, bool value)
        {
            __instance.gameObject.SetActive(false);
            if (value)
            {
                ModUIConstants.ShowPhotoModeUIRework();
            }
            else
            {
                ModUIConstants.HidePhotoModeUIRework();
            }
        }
    }
}
