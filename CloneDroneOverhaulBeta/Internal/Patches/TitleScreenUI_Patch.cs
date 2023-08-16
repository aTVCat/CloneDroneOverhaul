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
    }
}
