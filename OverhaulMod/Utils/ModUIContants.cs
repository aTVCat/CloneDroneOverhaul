using OverhaulMod.UI;
using UnityEngine;

namespace OverhaulMod.Utils
{
    public static class ModUIConstants
    {
        public const string UI_OTHER_MODS = "UI_OtherMods";
        public const string UI_ENDLESS_MODE = "UI_EndlessModeMenu";
        public const string UI_ENDLESS_MODE_LEADERBOARD = "UI_EndlessModeLeaderboard";
        public const string UI_SETTINGS_MENU = "UI_SettingsMenuRework";
        public const string UI_TITLE_SCREEN = "UI_TitleScreenRework";
        public const string UI_ADVANCEMENT_PROGRESS = "UI_AdvancementProgress";
        public const string UI_ADVANCEMENTS_MENU = "UI_AdvancementsMenuRework";
        public const string UI_WORKSHOP_BROWSER = "UI_WorkshopBrowserRework";
        public const string UI_MESSAGE_POPUP = "UI_MessagePopup";
        public const string UI_FEEDBACK_UI = "UI_FeedbackUIRework";
        public const string UI_COMMUNITY_HUB = "UI_CommunityHub";
        public const string UI_PAUSE_MENU = "UI_PauseMenuRework";
        public const string UI_CHAPTER_SELECT_MENU = "UI_ChapterSelectionMenu";
        public const string UI_CHAPTER_LEVEL_SELECT_MENU = "UI_ChapterLevelSelectionMenu";
        public const string UI_LOADING_SCREEN = "UI_LoadingScreen";
        public const string UI_EXCLUSIVE_CONTENT_MENU = "UI_ExclusiveContentMenu";
        public const string UI_EXCLUSIVE_CONTENT_EDITOR = "UI_ExclusiveContentEditor";
        public const string UI_CONNECT_SCREEN = "UI_ConnectScreen";
        public const string UI_NEWS_PANEL = "UI_NewsPanel";
        public const string UI_CONTENT_DOWNLOAD_WINDOW = "UI_ContentDownloadWindow";
        public const string UI_UPDATES_WINDOW = "UI_UpdatesWindow";
        public const string UI_PERSONALIZATION_ITEMS_BROWSER = "UI_PersonalizationItemsBrowser";
        public const string UI_UPDATE_INFO_EDITOR = "UI_UpdateInfoEditor";

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

        public static void ShowWorkshopBrowserRework()
        {
            _ = ModUIManager.Instance.Show<UIWorkshopBrowser>(AssetBundleConstants.UI, UI_WORKSHOP_BROWSER, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static UIMessagePopup ShowMessagePopup()
        {
            return ModUIManager.Instance.Show<UIMessagePopup>(AssetBundleConstants.UI, UI_MESSAGE_POPUP, ModUIManager.EUILayer.Last);
        }

        public static void ShowFeedbackUIRework()
        {
            _ = ModUIManager.Instance.Show<UIFeedbackMenu>(AssetBundleConstants.UI, UI_FEEDBACK_UI, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowCommunityHub()
        {
            _ = ModUIManager.Instance.Show<UICommunityHub>(AssetBundleConstants.UI, UI_COMMUNITY_HUB, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowPauseMenuRework()
        {
            _ = ModUIManager.Instance.Show<UIPauseMenu>(AssetBundleConstants.UI, UI_PAUSE_MENU, ModUIManager.EUILayer.AfterEscMenu);
        }

        public static void ShowChapterSelectMenu()
        {
            _ = ModUIManager.Instance.Show<UIChapterSelectMenu>(AssetBundleConstants.UI, UI_CHAPTER_SELECT_MENU, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowChapterLevelSelectMenu(Transform parent, int chapterIndex)
        {
            UIChapterSectionSelectMenu chapterLevelSelectMenu = ModUIManager.Instance.Show<UIChapterSectionSelectMenu>(AssetBundleConstants.UI, UI_CHAPTER_LEVEL_SELECT_MENU, parent);
            chapterLevelSelectMenu.PopulateChapter(chapterIndex);
        }

        public static void ShowLoadingScreen()
        {
            _ = ModUIManager.Instance.Show<UILoadingScreen>(AssetBundleConstants.UI, UI_LOADING_SCREEN, ModUIManager.EUILayer.Last);
        }

        public static void HideLoadingScreen()
        {
            _ = ModUIManager.Instance.Hide(AssetBundleConstants.UI, UI_LOADING_SCREEN);
        }

        public static void ShowExclusiveContentMenu()
        {
            _ = ModUIManager.Instance.Show<UIExclusiveContentMenu>(AssetBundleConstants.UI, UI_EXCLUSIVE_CONTENT_MENU, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowExclusiveContentEditor(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIExclusiveContentEditor>(AssetBundleConstants.UI, UI_EXCLUSIVE_CONTENT_EDITOR, parent);
        }

        public static void ShowMultiplayerConnectScreen()
        {
            _ = ModUIManager.Instance.Show<UIMultiplayerConnectScreen>(AssetBundleConstants.UI, UI_CONNECT_SCREEN, ModUIManager.EUILayer.AfterMultiplayerConnectScreen);
        }

        public static void ShowNewsPanel()
        {
            _ = ModUIManager.Instance.Show<UINewsPanel>(AssetBundleConstants.UI, UI_NEWS_PANEL, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowContentDownloadWindow()
        {
            _ = ModUIManager.Instance.Show<UIContentDownloadWindow>(AssetBundleConstants.UI, UI_CONTENT_DOWNLOAD_WINDOW, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowUpdatesWindow()
        {
            _ = ModUIManager.Instance.Show<UIUpdatesWindow>(AssetBundleConstants.UI, UI_UPDATES_WINDOW, ModUIManager.EUILayer.AfterTitleScreen);
        }

        public static void ShowPersonalizationItemsBrowser()
        {
            _ = ModUIManager.Instance.Show<UIPersonalizationItemsBrowser>(AssetBundleConstants.UI, UI_PERSONALIZATION_ITEMS_BROWSER, ModUIManager.EUILayer.BeforeEscMenu);
        }

        public static void ShowUpdateInfoEditor(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIUpdateInfoEditor>(AssetBundleConstants.UI, UI_UPDATE_INFO_EDITOR, parent);
        }
    }
}
