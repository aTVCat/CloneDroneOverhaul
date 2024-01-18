using OverhaulMod.Content;
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
        public const string UI_NEWS_INFO_EDITOR = "UI_NewsInfoEditor";
        public const string UI_RESTART_REQUIRED_SCREEN = "UI_RestartRequiredScreen";
        public const string UI_NEWS_DETAILS_PANEL = "UI_NewsDetailsPanel";
        public const string UI_INFORMATION_SELECT_WINDOW = "UI_InformationSelectWindow";
        public const string UI_OVERHAUL_MOD_INFO_WINDOW = "UI_OverhaulModInfoWindow";
        public const string UI_WORKSHOP_ITEM_PAGE_WINDOW = "UI_WorkshopItemPageWindow";
        public const string UI_MESSAGE_POPUP_FULL_SCREEN = "UI_MessagePopupFullScreen";
        public const string UI_OVERHAUL_UI_MANAGEMENT_PANEL = "UI_OverhaulUIsManagementPanel";
        public const string UI_LEVEL_DESCRIPTION_LIST_EDITOR = "UI_LevelDescriptionListEditor";
        public const string UI_PERSONALIZATION_EDITOR = "UI_PersonalizationEditor";
        public const string UI_MULTIPLAYER_GAMEMODE_SELECT_SCREEN = "UI_MPSelectScreen";
        public const string UI_CREDITS_MENU = "UI_CreditsMenu";
        public const string UI_ADDONS_MENU = "UI_AddonsMenu";
        public const string UI_ADDONS_EDITOR = "UI_AddonsEditor";
        public const string UI_CHALLENGES_MENU_REWORK = "UI_ChallengesMenuRework";

        public static void ShowOtherModsMenu()
        {
            _ = ModUIManager.Instance.Show<UIOtherMods>(AssetBundleConstants.UI, UI_OTHER_MODS, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowEndlessModeMenu()
        {
            _ = ModUIManager.Instance.Show<UIEndlessModeMenu>(AssetBundleConstants.UI, UI_ENDLESS_MODE, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowLeaderboard(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIEndlessModeLeaderboard>(AssetBundleConstants.UI, UI_ENDLESS_MODE_LEADERBOARD, parent);
        }

        public static void ShowSettingsMenuRework()
        {
            _ = ModUIManager.Instance.Show<UISettingsMenuRework>(AssetBundleConstants.UI, UI_SETTINGS_MENU, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowTitleScreenRework()
        {
            _ = ModUIManager.Instance.Show<UITitleScreenRework>(AssetBundleConstants.UI, UI_TITLE_SCREEN, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowAdvancementProgress(GameplayAchievement gameplayAchievement)
        {
            UIAdvancementProgress panel = ModUIManager.Instance.Show<UIAdvancementProgress>(AssetBundleConstants.UI, UI_ADVANCEMENT_PROGRESS, ModUIManager.UILayer.AfterTitleScreen);
            if (panel)
            {
                panel.ShowProgress(gameplayAchievement);
            }
        }

        public static void ShowAdvancementsMenuRework()
        {
            _ = ModUIManager.Instance.Show<UIAdvancementsMenu>(AssetBundleConstants.UI, UI_ADVANCEMENTS_MENU, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowWorkshopBrowserRework()
        {
            _ = ModUIManager.Instance.Show<UIWorkshopBrowser>(AssetBundleConstants.UI, UI_WORKSHOP_BROWSER, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static UIMessagePopup ShowMessagePopup()
        {
            return ModUIManager.Instance.Show<UIMessagePopup>(AssetBundleConstants.UI, UI_MESSAGE_POPUP, ModUIManager.UILayer.Last);
        }

        public static UIMessagePopup ShowFullScreenMessagePopup()
        {
            return ModUIManager.Instance.Show<UIMessagePopup>(AssetBundleConstants.UI, UI_MESSAGE_POPUP_FULL_SCREEN, ModUIManager.UILayer.Last);
        }

        public static void ShowFeedbackUIRework()
        {
            _ = ModUIManager.Instance.Show<UIFeedbackMenu>(AssetBundleConstants.UI, UI_FEEDBACK_UI, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowCommunityHub()
        {
            _ = ModUIManager.Instance.Show<UICommunityHub>(AssetBundleConstants.UI, UI_COMMUNITY_HUB, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowPauseMenuRework()
        {
            _ = ModUIManager.Instance.Show<UIPauseMenuRework>(AssetBundleConstants.UI, UI_PAUSE_MENU, ModUIManager.UILayer.AfterEscMenu);
        }

        public static void ShowChapterSelectMenu()
        {
            _ = ModUIManager.Instance.Show<UIChapterSelectMenuRework>(AssetBundleConstants.UI, UI_CHAPTER_SELECT_MENU, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowChapterLevelSelectMenu(Transform parent, int chapterIndex)
        {
            UIChapterSectionSelectMenu chapterLevelSelectMenu = ModUIManager.Instance.Show<UIChapterSectionSelectMenu>(AssetBundleConstants.UI, UI_CHAPTER_LEVEL_SELECT_MENU, parent);
            chapterLevelSelectMenu.PopulateChapter(chapterIndex);
        }

        public static void ShowLoadingScreen()
        {
            _ = ModUIManager.Instance.Show<UILoadingScreenRework>(AssetBundleConstants.UI, UI_LOADING_SCREEN, ModUIManager.UILayer.Last);
        }

        public static void HideLoadingScreen()
        {
            _ = ModUIManager.Instance.Hide(AssetBundleConstants.UI, UI_LOADING_SCREEN);
        }

        public static void ShowExclusiveContentMenu()
        {
            _ = ModUIManager.Instance.Show<UIExclusiveContentMenu>(AssetBundleConstants.UI, UI_EXCLUSIVE_CONTENT_MENU, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowExclusiveContentEditor(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIExclusiveContentEditor>(AssetBundleConstants.UI, UI_EXCLUSIVE_CONTENT_EDITOR, parent);
        }

        public static void ShowMultiplayerConnectScreen()
        {
            _ = ModUIManager.Instance.Show<UIMultiplayerConnectScreen>(AssetBundleConstants.UI, UI_CONNECT_SCREEN, ModUIManager.UILayer.AfterMultiplayerConnectScreen);
        }

        public static void ShowNewsPanel()
        {
            _ = ModUIManager.Instance.Show<UINewsPanel>(AssetBundleConstants.UI, UI_NEWS_PANEL, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowContentDownloadWindow()
        {
            _ = ModUIManager.Instance.Show<UIContentDownloadWindow>(AssetBundleConstants.UI, UI_CONTENT_DOWNLOAD_WINDOW, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowUpdatesWindow()
        {
            _ = ModUIManager.Instance.Show<UIUpdatesWindow>(AssetBundleConstants.UI, UI_UPDATES_WINDOW, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowPersonalizationItemsBrowser()
        {
            _ = ModUIManager.Instance.Show<UIPersonalizationItemsBrowser>(AssetBundleConstants.UI, UI_PERSONALIZATION_ITEMS_BROWSER, ModUIManager.UILayer.BeforeEscMenu);
        }

        public static void ShowUpdateInfoEditor(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIUpdateInfoEditor>(AssetBundleConstants.UI, UI_UPDATE_INFO_EDITOR, parent);
        }

        public static void ShowNewsInfoEditor(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UINewsInfoEditor>(AssetBundleConstants.UI, UI_NEWS_INFO_EDITOR, parent);
        }

        public static void ShowRestartRequiredScreen(bool allowIgnoring)
        {
            UIRestartRequiredScreen screen = ModUIManager.Instance.Show<UIRestartRequiredScreen>(AssetBundleConstants.UI, UI_RESTART_REQUIRED_SCREEN, ModUIManager.UILayer.AfterCrashScreen);
            screen.SetAllowIgnoring(allowIgnoring);
        }

        public static void ShowNewsDetailsPanel(Transform parent, NewsInfo newsInfo)
        {
            UINewsDetailsPanel panel = ModUIManager.Instance.Show<UINewsDetailsPanel>(AssetBundleConstants.UI, UI_NEWS_DETAILS_PANEL, parent);
            panel.Populate(newsInfo);
        }

        public static void ShowInformationSelectMenu()
        {
            _ = ModUIManager.Instance.Show<UIInformationSelectWindow>(AssetBundleConstants.UI, UI_INFORMATION_SELECT_WINDOW, ModUIManager.UILayer.BeforeCrashScreen);
        }

        public static void ShowOverhaulModInfoMenu(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIOverhaulInfoWindow>(AssetBundleConstants.UI, UI_OVERHAUL_MOD_INFO_WINDOW, parent);
        }

        public static void ShowWorkshopItemPageWindow(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIWorkshopItemPageWindow>(AssetBundleConstants.UI, UI_WORKSHOP_ITEM_PAGE_WINDOW, parent);
        }

        public static void ShowOverhaulUIManagementPanel(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIOverhaulUIManagementPanel>(AssetBundleConstants.UI, UI_OVERHAUL_UI_MANAGEMENT_PANEL, parent);
        }

        public static void ShowLevelDescriptionListEditor()
        {
            _ = ModUIManager.Instance.Show<UILevelDescriptionListEditor>(AssetBundleConstants.UI, UI_LEVEL_DESCRIPTION_LIST_EDITOR, ModUIManager.UILayer.BeforeCrashScreen);
        }

        public static void ShowPersonalizationEditorUI()
        {
            _ = ModUIManager.Instance.Show<UIPersonalizationEditor>(AssetBundleConstants.UI, UI_PERSONALIZATION_EDITOR, ModUIManager.UILayer.BeforeEscMenu);
        }

        public static void ShowMultiplayerGameModeSelectScreen()
        {
            _ = ModUIManager.Instance.Show<UIMultiplayerGameModeSelectScreen>(AssetBundleConstants.UI, UI_MULTIPLAYER_GAMEMODE_SELECT_SCREEN, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowCreditsMenu(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIAllCreditsMenu>(AssetBundleConstants.UI, UI_CREDITS_MENU, parent);
        }

        public static void ShowAddonsMenu()
        {
            _ = ModUIManager.Instance.Show<UIAddonsMenu>(AssetBundleConstants.UI, UI_ADDONS_MENU, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowAddonsEditor(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIAddonsEditor>(AssetBundleConstants.UI, UI_ADDONS_EDITOR, parent);
        }

        public static void ShowChallengesMenuRework()
        {
            _ = ModUIManager.Instance.Show<UIChallengesMenuRework>(AssetBundleConstants.UI, UI_CHALLENGES_MENU_REWORK, ModUIManager.UILayer.AfterTitleScreen);
        }
    }
}
