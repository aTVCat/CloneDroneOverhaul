using HarmonyLib;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ArenaCameraManager))]
    internal static class ArenaCameraManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ArenaCameraManager.SetTitleScreenLogoVisible))]
        private static void SetTitleScreenLogoVisible_Postfix(ArenaCameraManager __instance, bool visible)
        {
            __instance.TitleScreenLogoCamera.enabled = visible;
        }

        /*[HarmonyPrefix]
        [HarmonyPatch(nameof(ArenaCameraManager.updateLogoCameraRect))]
        private static bool updateLogoCameraRect_Prefix(ArenaCameraManager __instance)
        {
            if (__instance.TitleScreenLogoCamera && __instance.TitleScreenLogoCamera.gameObject && __instance.TitleScreenLogoCamera.gameObject.activeInHierarchy)
            {
                bool leftSide = TitleScreenCustomizationManager.PanelPosition == TitleScreenPanelPosition.LeftSide;

                RectTransform rootButtonsContainer = ModCache.TitleScreenUI.RootButtonsContainer;
                float num = rootButtonsContainer.anchoredPosition.x + rootButtonsContainer.rect.width / 2f;
                float width = leftSide ? (2f * num / UIManager.Instance.UIRoot.rect.width) : 1f;
                __instance.TitleScreenLogoCamera.rect = new Rect(0f, __instance.TitleScreenLogoCamera.rect.y, width, __instance.TitleScreenLogoCamera.rect.height);
            }
            return false;
        }*/
    }
}
