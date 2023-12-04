using OverhaulMod.UI;
using UnityEngine;

namespace OverhaulMod.Utils
{
    public static class UIConstants
    {
        public static void ShowOtherModsMenu()
        {
            _ = ModUIManager.Instance.Show<UIOtherMods>(AssetBundleConstants.UI, "UI_OtherMods", ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowEndlessModeMenu()
        {
            _ = ModUIManager.Instance.Show<UIEndlessModeMenu>(AssetBundleConstants.UI, "UI_EndlessModeMenu", ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowLeaderboard(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIEndlessModeLeaderboard>(AssetBundleConstants.UI, "UI_EndlessModeLeaderboard", parent);
        }
    }
}
