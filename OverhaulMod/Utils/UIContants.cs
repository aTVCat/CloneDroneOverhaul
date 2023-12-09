using OverhaulMod.UI;
using UnityEngine;

namespace OverhaulMod.Utils
{
    public static class UIConstants
    {
        public const string UI_OTHER_MODS = "UI_OtherMods";
        public const string UI_ENDLESS_MODE = "UI_EndlessModeMenu";
        public const string UI_ENDLESS_MODE_LEADERBOARD = "UI_EndlessModeLeaderboard";
        public const string UI_SETTINGS_MENU = "UI_SettingsMenuRework";
        public const string UI_TITLE_SCREEN = "UI_TitleScreenRework";
        public const string UI_ADVANCEMENT_PROGRESS = "UI_AdvancementProgress";
        public const string UI_ADVANCEMENTS_MENU = "UI_AdvancementsMenuRework";

        public static void ShowOtherModsMenu()
        {
            _ = ModUIManager.Instance.Show<UIOtherMods>(AssetBundleConstants.UI, UI_OTHER_MODS, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowEndlessModeMenu()
        {
            _ = ModUIManager.Instance.Show<UIEndlessModeMenu>(AssetBundleConstants.UI, UI_ENDLESS_MODE, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowLeaderboard(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIEndlessModeLeaderboard>(AssetBundleConstants.UI, UI_ENDLESS_MODE_LEADERBOARD, parent);
        }

        public static void ShowSettingsMenuRework()
        {
            _ = ModUIManager.Instance.Show<UISettingsMenuRework>(AssetBundleConstants.UI, UI_SETTINGS_MENU, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowTitleScreenRework()
        {
            _ = ModUIManager.Instance.Show<UITitleScreenRework>(AssetBundleConstants.UI, UI_TITLE_SCREEN, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowAdvancementProgress(GameplayAchievement gameplayAchievement)
        {
            UIAdvancementProgress panel = ModUIManager.Instance.Show<UIAdvancementProgress>(AssetBundleConstants.UI, UI_ADVANCEMENT_PROGRESS, ModUIManager.EUILayer.AfterTitleScreen);
            if (panel)
            {
                panel.ShowProgress(gameplayAchievement);
            }
        }

        public static void ShowAdvancementsMenuRework()
        {
            _ = ModUIManager.Instance.Show<UIAdvancementsMenu>(AssetBundleConstants.UI, UI_ADVANCEMENTS_MENU, ModUIManager.EUILayer.AfterTitleScreen);
        }
    }
}
