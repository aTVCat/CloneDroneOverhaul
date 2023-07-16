using CDOverhaul.Workshop;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(TitleScreenUI))]
    internal static class TitleScreenUI_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("setLogoAndRootButtonsVisible")]
        private static void setLogoAndRootButtonsVisible_Postfix(TitleScreenUI __instance, bool visible)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            __instance.transform.GetChild(1).gameObject.SetActive(visible);
        }

        [HarmonyPrefix]
        [HarmonyPatch("OnWorkshopBrowserButtonClicked")]
        private static bool OnWorkshopBrowserButtonClicked_Prefix(TitleScreenUI __instance)
        {
            // Todo: Move this to TitlscreenUI replacement
            return !OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsNewWorkshopBrowserEnabled || !OverhaulMod.IsModInitialized || OverhaulWorkshopBrowserUI.BrowserIsNull || !OverhaulWorkshopBrowserUI.Instance.TryShow();
        }
    }
}
