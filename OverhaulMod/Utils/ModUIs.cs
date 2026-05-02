using OverhaulMod.Content;
using OverhaulMod.UI;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Utils
{
    public static class ModUIs
    {
        public const string UI_VERSION_LABEL = "UI_VersionLabel";
        public const string UI_ENDLESS_MODE = "UI_EndlessModeMenu";
        public const string UI_ENDLESS_MODE_LEADERBOARD = "UI_EndlessModeLeaderboard";
        public const string UI_SETTINGS_MENU_REWORK = "UI_SettingsMenuRework";
        public const string UI_TITLE_SCREEN_REWORK = "UI_TitleScreenRework";
        public const string UI_ADVANCEMENT_PROGRESS = "UI_AdvancementProgress";
        public const string UI_ADVANCEMENTS_MENU = "UI_AdvancementsMenuRework";
        public const string UI_WORKSHOP_BROWSER = "UI_WorkshopBrowserRework";
        public const string UI_MESSAGE_POPUP = "UI_MessagePopup";
        public const string UI_FEEDBACK_UI = "UI_FeedbackUIRework";
        public const string UI_PAUSE_MENU = "UI_PauseMenuRework";
        public const string UI_CHAPTER_SELECT_MENU = "UI_ChapterSelectionMenu";
        public const string UI_REPLAY_ENCOUNTER_MENU = "UI_ReplayEncounterMenu";
        public const string UI_LOADING_SCREEN = "UI_LoadingScreen";
        public const string UI_EXCLUSIVE_PERKS_MENU = "UI_ExclusivePerksMenu";
        public const string UI_EXCLUSIVE_PERKS_EDITOR = "UI_ExclusivePerksEditor";
        public const string UI_PERSONALIZATION_ITEMS_BROWSER = "UI_PersonalizationItemsBrowser";
        public const string UI_NEWS_INFO_EDITOR = "UI_NewsInfoEditor";
        public const string UI_RESTART_REQUIRED_SCREEN = "UI_RestartRequiredScreen";
        public const string UI_INFORMATION_SELECT_WINDOW = "UI_InformationSelectWindow";
        public const string UI_OVERHAUL_MOD_INFO_WINDOW = "UI_OverhaulModInfoWindow";
        public const string UI_WORKSHOP_ITEM_PAGE_WINDOW = "UI_WorkshopItemPageWindow";
        public const string UI_MESSAGE_POPUP_FULL_SCREEN = "UI_MessagePopupFullScreen";
        public const string UI_OVERHAUL_UI_MANAGEMENT_PANEL = "UI_OverhaulUIsManagementPanel";
        public const string UI_PERSONALIZATION_EDITOR = "UI_PersonalizationEditor";
        public const string UI_CREDITS_MENU = "UI_CreditsMenu";
        public const string UI_ADDONS_MENU = "UI_AddonsMenu";
        public const string UI_ADDONS_EDITOR = "UI_AddonsEditor";
        public const string UI_CHALLENGES_MENU_REWORK = "UI_ChallengesMenuRework";
        public const string UI_LOCALIZATION_EDITOR = "UI_LocalizationEditor";
        public const string UI_GENERIC_INPUT_FIELD_WINDOW = "UI_GenericInputWindow";
        public const string UI_ADDONS_DOWNLOAD_EDITOR = "UI_AddonsDownloadEditor";
        public const string UI_TITLE_SCREEN_CUSTOMIZATION_PANEL = "UI_TitleScreenCustomizationPanel";
        public const string UI_LEVEL_DESCRIPTION_BROWSER = "UI_LevelDescriptionBrowser";
        public const string UI_PHOTO_MODE_UI_REWORK = "UI_PhotoModeUIRework";
        public const string UI_DEVELOPMENT_GALLERY = "UI_DevelopmentGallery";
        public const string UI_SET_KEY_BIND_WINDOW = "UI_SetKeyBindWindow";
        public const string UI_PERSONALIZATION_EDITOR_ITEMS_BROWSER = "UI_PersonalizationEditorItemsBrowser";
        public const string UI_PERSONALIZATION_EDITOR_VERIFICATION_MENU = "UI_PersonalizationEditorVerificationMenu";
        public const string UI_DEBUG_MENU = "UI_DebugMenu";
        public const string UI_CRASH_SCREEN = "UI_CrashScreen";
        public const string UI_GENERIC_COLOR_PICKER = "UI_GenericColorPicker";
        public const string UI_GENERIC_IMAGE_VIEWER = "UI_GenericImageViewer";
        public const string UI_PERSONALIZATION_EDITOR_AUTHORS_EDIT_MENU = "UI_PersonalizationEditorAuthorsEditMenu";
        public const string UI_DUEL_INVITE_MENU_REWORK = "UI_DuelInviteMenuRework";
        public const string UI_SCREEN_TOOLTIPS = "UI_ScreenTooltips";
        public const string UI_FILE_EXPLORER = "UI_FileExplorer";
        public const string UI_PERSONALIZATION_EDITOR_OBJECT_BROWSER = "UI_PersonalizationEditorObjectBrowser";
        public const string UI_CINEMATIC_EFFECTS = "UI_CinematicEffects";
        public const string UI_IMAGE_EXPLORER = "UI_ImageExplorer";
        public const string UI_AUTO_BUILD_MENU = "UI_AutoBuildMenu";
        public const string UI_PERSONALIZATION_ASSETS_MENU = "UI_PersonalizationAssetsMenu";
        public const string UI_DISCORD_SERVER_MENU = "UI_DiscordServerMenu";
        public const string UI_PERSONALIZATION_EDITOR_EXCLUSIVITY_EDIT_MENU = "UI_PersonalizationEditorExclusivityEditMenu";
        public const string UI_PERSONALIZATION_EDITOR_ABOUT_DIALOG = "UI_PersonalizationEditorAboutDialog";
        public const string UI_ASSET_BUNDLE_ASSETS_BROWSER = "UI_AssetBundleAssetsBrowser";
        public const string UI_PATCH_NOTES = "UI_PatchNotes";
        public const string UI_PERSONALIZATION_EDITOR_ITEM_CREATION_DIALOG = "UI_PersonalizationEditorItemCreationDialog";
        public const string UI_PRESS_ACTION_KEY_DESCRIPTION = "UI_PressActionKeyDescription";
        public const string UI_SUBTITLE_TEXT_FIELD_REWORK = "UI_SubtitleTextFieldRework";
        public const string UI_AUTO_BUILD_SELECTION_MENU = "UI_AutoBuildSelectionMenu";
        public const string UI_PERSONALIZATION_EDITOR_PLAYTEST_HUD = "UI_PersonalizationEditorPlaytestHUD";
        public const string UI_PERSONALIZATION_EDITOR_ITEM_IMPORT_DIALOG = "UI_PersonalizationEditorItemImportDialog";
        public const string UI_UPDATES_WINDOW_REWORK = "UI_UpdatesWindowRework";
        public const string UI_PERSONALIZATION_EDITOR_MAGICA_VOXEL_TIP = "UI_PersonalizationEditorMagicaVoxelTip";
        public const string UI_INTRO = "UI_Intro";
        public const string UI_SETTINGS_IMPORT_EXPORT_MENU = "UI_SettingsImportExportMenu";
        public const string UI_WORKSHOP_ITEM_PAGE_PLAY_OPTIONS = "UI_WorkshopItemPagePlayOptions";
        public const string UI_WORKSHOP_BROWSER_HISTORY_PANEL = "UI_WorkshopBrowserHistoryPanel";
        public const string UI_ADDONS_EDITOR_CREATION_DIALOG = "UI_AddonsEditorCreationDialog";
        public const string UI_ADDON_DETAILS_MENU = "UI_AddonsDetailsMenu";
        public const string UI_UPDATES_EDITOR = "UI_UpdatesEditor";
        public const string UI_UPDATE_DETAILS_WINDOW = "UI_UpdateDetailsWindow";
        public const string UI_TITLE_SCREEN_HYPOCRISIS_SKIN = "UI_TitleScreenHypocrisisSkin";
        public const string UI_PERSONALIZATION_EDITOR_EXPORT_ALL_MENU = "UI_PersonalizationEditorExportAllMenu";
        public const string UI_PERSONALIZATION_EDITOR_ITEM_IMPORT_HELPER = "UI_PersonalizationEditorItemImportHelper";
        public const string UI_PERSONALIZATION_EDITOR_SCREENSHOT_CONTROLS = "UI_PersonalizationEditorScreenshotControls";
        public const string UI_SETTINGS_MENU_V2 = "UI_SettingsMenuReworkV2";
        public const string UI_SETTING_INFO_EDITOR = "UI_SettingInfoEditor";

        public static UIVersionLabel ShowVersionLabel()
        {
            return ModUIManager.Instance.Show<UIVersionLabel>(ModAssetBundles.UI, UI_VERSION_LABEL, ModUIManager.UILayer.BeforeCrashScreen);
        }

        public static UIEndlessModeMenu ShowEndlessModeMenu()
        {
            return ModUIManager.Instance.Show<UIEndlessModeMenu>(ModAssetBundles.UI, UI_ENDLESS_MODE, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static UIEndlessModeLeaderboard ShowLeaderboard(Transform parent, List<HighScoreData> list, string fileName)
        {
            UIEndlessModeLeaderboard leaderboard = ModUIManager.Instance.Show<UIEndlessModeLeaderboard>(ModAssetBundles.UI, UI_ENDLESS_MODE_LEADERBOARD, parent);
            leaderboard.Populate(list, fileName);
            return leaderboard;
        }

        public static UISettingsMenuRework ShowSettingsMenuRework(bool setup)
        {
            UISettingsMenuRework settingsMenuRework = ModUIManager.Instance.Show<UISettingsMenuRework>(ModAssetBundles.UI, UI_SETTINGS_MENU_REWORK, ModUIManager.UILayer.AfterTitleScreen);
            if (setup)
                settingsMenuRework.ShowSetupElements();
            else
                settingsMenuRework.ShowRegularElements();
            return settingsMenuRework;
        }

        public static UISettingsMenuReworkV2 ShowSettingsMenuReworkV2()
        {
            UISettingsMenuReworkV2 settingsMenuRework = ModUIManager.Instance.Show<UISettingsMenuReworkV2>(ModAssetBundles.UI, UI_SETTINGS_MENU_V2, ModUIManager.UILayer.AfterTitleScreen);
            return settingsMenuRework;
        }

        public static UISettingInfoEditor ShowSettingInfoEditor(Transform parent)
        {
            UISettingInfoEditor settingInfoEditor = ModUIManager.Instance.Show<UISettingInfoEditor>(ModAssetBundles.UI, UI_SETTING_INFO_EDITOR, parent);
            return settingInfoEditor;
        }

        public static UITitleScreenRework ShowTitleScreenRework()
        {
            return ModUIManager.Instance.Show<UITitleScreenRework>(ModAssetBundles.UI, UI_TITLE_SCREEN_REWORK, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static UITitleScreenRework ShowTitleScreenReworkIfHaventBefore()
        {
            UITitleScreenRework result = ModUIManager.Instance.Get<UITitleScreenRework>(ModAssetBundles.UI, UI_TITLE_SCREEN_REWORK);
            if (result && result.IsVisible)
                return result;

            return ModUIManager.Instance.Show<UITitleScreenRework>(ModAssetBundles.UI, UI_TITLE_SCREEN_REWORK, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void HideTitleScreenRework()
        {
            _ = ModUIManager.Instance.Hide(ModAssetBundles.UI, UI_TITLE_SCREEN_REWORK);
        }

        public static UIAdvancementProgress ShowAdvancementProgress(GameplayAchievement gameplayAchievement)
        {
            UIAdvancementProgress panel = ModUIManager.Instance.Show<UIAdvancementProgress>(ModAssetBundles.UI, UI_ADVANCEMENT_PROGRESS, ModUIManager.UILayer.AfterTitleScreen);
            if (panel)
            {
                panel.ShowProgress(gameplayAchievement);
            }
            return panel;
        }

        public static UIAdvancementsMenu ShowAdvancementsMenuRework()
        {
            return ModUIManager.Instance.Show<UIAdvancementsMenu>(ModAssetBundles.UI, UI_ADVANCEMENTS_MENU, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static UIWorkshopBrowser ShowWorkshopBrowserRework()
        {
            return ModUIManager.Instance.Show<UIWorkshopBrowser>(ModAssetBundles.UI, UI_WORKSHOP_BROWSER, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static UIMessagePopup ShowMessagePopup()
        {
            return ModUIManager.Instance.Show<UIMessagePopup>(ModAssetBundles.UI, UI_MESSAGE_POPUP, ModUIManager.UILayer.Last);
        }

        public static UIMessagePopup ShowFullScreenMessagePopup()
        {
            return ModUIManager.Instance.Show<UIMessagePopup>(ModAssetBundles.UI, UI_MESSAGE_POPUP_FULL_SCREEN, ModUIManager.UILayer.Last);
        }

        public static UIFeedbackMenu ShowFeedbackUIRework(bool showExitButton)
        {
            UIFeedbackMenu feedbackMenu = ModUIManager.Instance.Show<UIFeedbackMenu>(ModAssetBundles.UI, UI_FEEDBACK_UI, ModUIManager.UILayer.BeforeCrashScreen);
            feedbackMenu.SetExitButtonVisible(showExitButton);
            return feedbackMenu;
        }

        public static UIPauseMenuRework ShowPauseMenuRework()
        {
            return ModUIManager.Instance.Show<UIPauseMenuRework>(ModAssetBundles.UI, UI_PAUSE_MENU, ModUIManager.UILayer.AfterEscMenu);
        }

        public static UIChapterSelectMenuRework ShowChapterSelectMenu()
        {
            return ModUIManager.Instance.Show<UIChapterSelectMenuRework>(ModAssetBundles.UI, UI_CHAPTER_SELECT_MENU, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static UIReplayEncounterMenu ShowChapterLevelSelectMenu(Transform parent, int chapterIndex)
        {
            UIReplayEncounterMenu chapterLevelSelectMenu = ModUIManager.Instance.Show<UIReplayEncounterMenu>(ModAssetBundles.UI, UI_REPLAY_ENCOUNTER_MENU, parent);
            chapterLevelSelectMenu.PopulateChapter(chapterIndex);
            return chapterLevelSelectMenu;
        }

        public static UILoadingScreenRework ShowLoadingScreen()
        {
            return ModUIManager.Instance.Show<UILoadingScreenRework>(ModAssetBundles.UI, UI_LOADING_SCREEN, ModUIManager.UILayer.Last);
        }

        public static void HideLoadingScreen()
        {
            _ = ModUIManager.Instance.Hide(ModAssetBundles.UI, UI_LOADING_SCREEN);
        }

        public static UIExclusivePerkMenu ShowExclusivePerksMenu()
        {
            return ModUIManager.Instance.Show<UIExclusivePerkMenu>(ModAssetBundles.UI, UI_EXCLUSIVE_PERKS_MENU, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static void ShowExclusivePerksEditor(Transform parent)
        {
            _ = ModUIManager.Instance.Show<UIExclusivePerkEditor>(ModAssetBundles.UI, UI_EXCLUSIVE_PERKS_EDITOR, parent);
        }

        public static UIPersonalizationItemBrowser ShowPersonalizationItemsBrowser()
        {
            return ModUIManager.Instance.Show<UIPersonalizationItemBrowser>(ModAssetBundles.UI, UI_PERSONALIZATION_ITEMS_BROWSER, ModUIManager.UILayer.BeforeEscMenu);
        }

        public static UINewsInfoEditor ShowNewsInfoEditor(Transform parent)
        {
            return ModUIManager.Instance.Show<UINewsInfoEditor>(ModAssetBundles.UI, UI_NEWS_INFO_EDITOR, parent);
        }

        public static UIRestartRequiredScreen ShowRestartRequiredScreen(bool allowIgnoring)
        {
            UIRestartRequiredScreen screen = ModUIManager.Instance.Show<UIRestartRequiredScreen>(ModAssetBundles.UI, UI_RESTART_REQUIRED_SCREEN, ModUIManager.UILayer.AfterCrashScreen);
            screen.SetAllowIgnoring(allowIgnoring);
            return screen;
        }

        public static UIInformationSelectWindow ShowInformationSelectMenu()
        {
            return ModUIManager.Instance.Show<UIInformationSelectWindow>(ModAssetBundles.UI, UI_INFORMATION_SELECT_WINDOW, ModUIManager.UILayer.BeforeCrashScreen);
        }

        public static UIOverhaulInfoWindow ShowOverhaulModInfoMenu(Transform parent)
        {
            return ModUIManager.Instance.Show<UIOverhaulInfoWindow>(ModAssetBundles.UI, UI_OVERHAUL_MOD_INFO_WINDOW, parent);
        }

        public static UIWorkshopItemPageWindow ShowWorkshopItemPageWindow(Transform parent)
        {
            return ModUIManager.Instance.Show<UIWorkshopItemPageWindow>(ModAssetBundles.UI, UI_WORKSHOP_ITEM_PAGE_WINDOW, parent);
        }

        public static UIOverhaulUIManagementPanel ShowOverhaulUIManagementPanel(Transform parent)
        {
            return ModUIManager.Instance.Show<UIOverhaulUIManagementPanel>(ModAssetBundles.UI, UI_OVERHAUL_UI_MANAGEMENT_PANEL, parent);
        }

        public static UIPersonalizationEditor ShowPersonalizationEditorUI()
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditor>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR, ModUIManager.UILayer.BeforeEscMenu);
        }

        public static UICreditsMenu ShowCreditsMenu(Transform parent)
        {
            return ModUIManager.Instance.Show<UICreditsMenu>(ModAssetBundles.UI, UI_CREDITS_MENU, parent);
        }

        public static UIAddonsMenu ShowAddonsMenu()
        {
            return ModUIManager.Instance.Show<UIAddonsMenu>(ModAssetBundles.UI, UI_ADDONS_MENU, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static UIAddonsEditor ShowAddonsEditor(Transform parent)
        {
            return ModUIManager.Instance.Show<UIAddonsEditor>(ModAssetBundles.UI, UI_ADDONS_EDITOR, parent);
        }

        public static UIChallengesMenuRework ShowChallengesMenuRework(bool coop, bool privateMatches)
        {
            UIChallengesMenuRework challengesMenuRework = ModUIManager.Instance.Show<UIChallengesMenuRework>(ModAssetBundles.UI, UI_CHALLENGES_MENU_REWORK, ModUIManager.UILayer.AfterTitleScreen);
            challengesMenuRework.Populate(coop, privateMatches);
            return challengesMenuRework;
        }

        public static UILocalizationEditor ShowLocalizationEditor()
        {
            return ModUIManager.Instance.Show<UILocalizationEditor>(ModAssetBundles.UI, UI_LOCALIZATION_EDITOR, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static UIGenericInputFieldWindow ShowGenericInputFieldWindow()
        {
            return ModUIManager.Instance.Show<UIGenericInputFieldWindow>(ModAssetBundles.UI, UI_GENERIC_INPUT_FIELD_WINDOW, ModUIManager.UILayer.Last);
        }

        public static UIAddonsDownloadEditor ShowAddonsDownloadEditor(Transform parent)
        {
            return ModUIManager.Instance.Show<UIAddonsDownloadEditor>(ModAssetBundles.UI, UI_ADDONS_DOWNLOAD_EDITOR, parent);
        }

        public static UITitleScreenCustomizationPanel ShowTitleScreenCustomizationPanel(Transform parent)
        {
            return ModUIManager.Instance.Show<UITitleScreenCustomizationPanel>(ModAssetBundles.UI, UI_TITLE_SCREEN_CUSTOMIZATION_PANEL, parent);
        }

        public static UILevelDescriptionBrowser ShowLevelDescriptionBrowser()
        {
            return ModUIManager.Instance.Show<UILevelDescriptionBrowser>(ModAssetBundles.UI, UI_LEVEL_DESCRIPTION_BROWSER, ModUIManager.UILayer.Last);
        }

        public static UIPhotoModeUIRework ShowPhotoModeUIRework()
        {
            return ModUIManager.Instance.Show<UIPhotoModeUIRework>(ModAssetBundles.UI, UI_PHOTO_MODE_UI_REWORK, ModUIManager.UILayer.BeforeEscMenu);
        }

        public static void HidePhotoModeUIRework()
        {
            _ = ModUIManager.Instance.Hide(ModAssetBundles.UI, UI_PHOTO_MODE_UI_REWORK);
        }

        public static UIDevelopmentGallery ShowDevelopmentGallery(Transform parent)
        {
            return ModUIManager.Instance.Show<UIDevelopmentGallery>(ModAssetBundles.UI, UI_DEVELOPMENT_GALLERY, parent);
        }

        public static UISetKeyBindWindow ShowSetKeyBindWindow(Transform parent)
        {
            return ModUIManager.Instance.Show<UISetKeyBindWindow>(ModAssetBundles.UI, UI_SET_KEY_BIND_WINDOW, parent);
        }

        public static UIPersonalizationEditorItemBrowser ShowPersonalizationEditorItemsBrowser(Transform parent)
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorItemBrowser>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_ITEMS_BROWSER, parent);
        }

        public static UIPersonalizationEditorVerificationMenu ShowPersonalizationEditorVerificationMenu(Transform parent)
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorVerificationMenu>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_VERIFICATION_MENU, parent);
        }

        public static UIDebugMenu ShowDebugMenu()
        {
            return ModUIManager.Instance.Show<UIDebugMenu>(ModAssetBundles.UI, UI_DEBUG_MENU, ModUIManager.UILayer.Last);
        }

        public static void HideDebugMenu()
        {
            _ = ModUIManager.Instance.Hide(ModAssetBundles.UI, UI_DEBUG_MENU);
        }

        public static UICrashScreen ShowCrashScreen(string errorMessage)
        {
            UICrashScreen crashScreen = ModUIManager.Instance.Show<UICrashScreen>(ModAssetBundles.UI, UI_CRASH_SCREEN, ModUIManager.UILayer.AfterCrashScreen);
            crashScreen.RefreshDetailsText();
            crashScreen.SetStackTraceText(errorMessage);
            return crashScreen;
        }

        public static void HideCrashScreen()
        {
            _ = ModUIManager.Instance.Hide(ModAssetBundles.UI, UI_CRASH_SCREEN);
        }

        public static UIGenericColorPicker ShowGenericColorPicker(Transform parent)
        {
            return ModUIManager.Instance.Show<UIGenericColorPicker>(ModAssetBundles.UI, UI_GENERIC_COLOR_PICKER, parent);
        }

        public static void HideGenericColorPicker()
        {
            _ = ModUIManager.Instance.Hide(ModAssetBundles.UI, UI_GENERIC_COLOR_PICKER);
        }

        public static UIGenericImageViewer ShowGenericImageViewer(Transform parent)
        {
            return ModUIManager.Instance.Show<UIGenericImageViewer>(ModAssetBundles.UI, UI_GENERIC_IMAGE_VIEWER, parent);
        }

        public static UIPersonalizationEditorAuthorsEditMenu ShowPersonalizationEditorAuthorsEditMenu(Transform parent)
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorAuthorsEditMenu>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_AUTHORS_EDIT_MENU, parent);
        }

        public static UIDuelInviteMenuRework ShowDuelInviteMenuRework(GameMode gameMode)
        {
            UIDuelInviteMenuRework duelInviteMenuRework = ModUIManager.Instance.Show<UIDuelInviteMenuRework>(ModAssetBundles.UI, UI_DUEL_INVITE_MENU_REWORK, ModUIManager.UILayer.AfterTitleScreen);
            duelInviteMenuRework.Populate(gameMode);
            return duelInviteMenuRework;
        }

        public static UIScreenTooltips ShowScreenTooltips()
        {
            return ModUIManager.Instance.Show<UIScreenTooltips>(ModAssetBundles.UI, UI_SCREEN_TOOLTIPS, ModUIManager.UILayer.Last);
        }

        public static UIFileExplorer ShowFileExplorer(Transform parent)
        {
            return ModUIManager.Instance.Show<UIFileExplorer>(ModAssetBundles.UI, UI_FILE_EXPLORER, parent);
        }

        public static UIPersonalizationEditorObjectBrowser ShowPersonalizationEditorObjectBrowser(Transform parent)
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorObjectBrowser>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_OBJECT_BROWSER, parent);
        }

        public static UICinematicEffects ShowCinematicEffects()
        {
            return ModUIManager.Instance.Show<UICinematicEffects>(ModAssetBundles.UI, UI_CINEMATIC_EFFECTS, ModUIManager.UILayer.AfterEnergyUI);
        }

        public static UIImageExplorer ShowImageExplorer(Transform parent)
        {
            return ModUIManager.Instance.Show<UIImageExplorer>(ModAssetBundles.UI, UI_IMAGE_EXPLORER, parent);
        }

        public static UIAutoBuildMenu ShowAutoBuildMenu()
        {
            return ModUIManager.Instance.Show<UIAutoBuildMenu>(ModAssetBundles.UI, UI_AUTO_BUILD_MENU, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static UIPersonalizationAssetsMenu ShowPersonalizationAssetsMenu(Transform transform)
        {
            return ModUIManager.Instance.Show<UIPersonalizationAssetsMenu>(ModAssetBundles.UI, UI_PERSONALIZATION_ASSETS_MENU, transform);
        }

        public static UIDiscordServerMenu ShowDiscordServerMenu(Transform transform)
        {
            return ModUIManager.Instance.Show<UIDiscordServerMenu>(ModAssetBundles.UI, UI_DISCORD_SERVER_MENU, transform);
        }

        public static UIPersonalizationEditorExclusivityEditMenu ShowPersonalizationEditorExclusivityEditMenu(Transform transform)
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorExclusivityEditMenu>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_EXCLUSIVITY_EDIT_MENU, transform);
        }

        public static UIPersonalizationEditorAboutDialog ShowPersonalizationEditorAboutDialog(Transform transform)
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorAboutDialog>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_ABOUT_DIALOG, transform);
        }

        public static UIAssetBundleAssetsBrowser ShowAssetBundleAssetsBrowser(Transform transform)
        {
            return ModUIManager.Instance.Show<UIAssetBundleAssetsBrowser>(ModAssetBundles.UI, UI_ASSET_BUNDLE_ASSETS_BROWSER, transform);
        }

        public static UIPatchNotes ShowPatchNotes(UIPatchNotes.ShowArguments showArguments)
        {
            UIPatchNotes patchNotes = ModUIManager.Instance.Show<UIPatchNotes>(ModAssetBundles.UI, UI_PATCH_NOTES, ModUIManager.UILayer.AfterTitleScreen, 1);
            patchNotes.SetContext(showArguments);
            return patchNotes;
        }

        public static UIPatchNotes ShowPatchNotes(Transform parent, UIPatchNotes.ShowArguments showArguments)
        {
            UIPatchNotes patchNotes = ModUIManager.Instance.Show<UIPatchNotes>(ModAssetBundles.UI, UI_PATCH_NOTES, parent);
            patchNotes.SetContext(showArguments);
            return patchNotes;
        }

        public static void HidePatchNotes()
        {
            ModUIManager.Instance.Hide(ModAssetBundles.UI, UI_PATCH_NOTES);
        }

        public static UIPersonalizationEditorItemCreationDialog ShowPersonalizationEditorItemCreationDialog(Transform transform)
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorItemCreationDialog>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_ITEM_CREATION_DIALOG, transform);
        }

        public static UIPressActionKeyDescription ShowPressActionKeyDescription(bool overEverything = false)
        {
            return ModUIManager.Instance.Show<UIPressActionKeyDescription>(ModAssetBundles.UI, UI_PRESS_ACTION_KEY_DESCRIPTION, overEverything ? ModUIManager.UILayer.BeforeCrashScreen : ModUIManager.UILayer.AfterEnergyUI);
        }

        public static UISubtitleTextFieldRework ShowSubtitleTextFieldRework()
        {
            return ModUIManager.Instance.Show<UISubtitleTextFieldRework>(ModAssetBundles.UI, UI_SUBTITLE_TEXT_FIELD_REWORK, ModUIManager.UILayer.AfterEnergyUI);
        }

        public static UIAutoBuildSelectionMenu ShowAutoBuildSelectionMenu()
        {
            return ModUIManager.Instance.Show<UIAutoBuildSelectionMenu>(ModAssetBundles.UI, UI_AUTO_BUILD_SELECTION_MENU, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static UIPersonalizationEditorPlaytestHUD ShowPersonalizationEditorPlaytestHUD()
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorPlaytestHUD>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_PLAYTEST_HUD, ModUIManager.UILayer.AfterEnergyUI);
        }

        public static void HidePersonalizationEditorPlaytestHUD()
        {
            _ = ModUIManager.Instance.Hide(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_PLAYTEST_HUD);
        }

        public static UIPersonalizationEditorItemImportDialog ShowPersonalizationEditorItemImportDialog(Transform transform)
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorItemImportDialog>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_ITEM_IMPORT_DIALOG, transform);
        }

        public static UIUpdatesWindowRework ShowUpdatesWindowRework()
        {
            return ModUIManager.Instance.Show<UIUpdatesWindowRework>(ModAssetBundles.UI, UI_UPDATES_WINDOW_REWORK, ModUIManager.UILayer.AfterTitleScreen);
        }

        public static UIPersonalizationEditorMagicaVoxelTip ShowPersonalizationEditorMagicaVoxelTip(Transform transform)
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorMagicaVoxelTip>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_MAGICA_VOXEL_TIP, transform);
        }

        public static UIIntro ShowIntro()
        {
            return ModUIManager.Instance.Show<UIIntro>(ModAssetBundles.UI, UI_INTRO, ModUIManager.UILayer.Last);
        }

        public static UISettingsImportExportMenu ShowSettingsImportExportMenu(Transform transform)
        {
            return ModUIManager.Instance.Show<UISettingsImportExportMenu>(ModAssetBundles.UI, UI_SETTINGS_IMPORT_EXPORT_MENU, transform);
        }

        public static UIWorkshopItemPagePlayOptions ShowWorkshopItemPagePlayOptions(Transform transform)
        {
            return ModUIManager.Instance.Show<UIWorkshopItemPagePlayOptions>(ModAssetBundles.UI, UI_WORKSHOP_ITEM_PAGE_PLAY_OPTIONS, transform);
        }

        public static UIWorkshopBrowserHistoryPanel ShowWorkshopBrowserHistoryPanel(Transform transform)
        {
            return ModUIManager.Instance.Show<UIWorkshopBrowserHistoryPanel>(ModAssetBundles.UI, UI_WORKSHOP_BROWSER_HISTORY_PANEL, transform);
        }

        public static UIAddonsEditorCreationDialog ShowAddonsEditorCreationDialog(Transform transform)
        {
            return ModUIManager.Instance.Show<UIAddonsEditorCreationDialog>(ModAssetBundles.UI, UI_ADDONS_EDITOR_CREATION_DIALOG, transform);
        }

        public static UIAddonDetailsMenu ShowAddonDetailsMenu(Transform transform)
        {
            return ModUIManager.Instance.Show<UIAddonDetailsMenu>(ModAssetBundles.UI, UI_ADDON_DETAILS_MENU, transform);
        }

        public static UIUpdatesEditor ShowUpdatesEditor(Transform transform)
        {
            return ModUIManager.Instance.Show<UIUpdatesEditor>(ModAssetBundles.UI, UI_UPDATES_EDITOR, transform);
        }

        public static UIUpdateDetailsWindow ShowUpdateDetailsWindow(Transform transform, UpdateInfo updateInfo, string branch)
        {
            UIUpdateDetailsWindow detailsWindow = ModUIManager.Instance.Show<UIUpdateDetailsWindow>(ModAssetBundles.UI, UI_UPDATE_DETAILS_WINDOW, transform);
            detailsWindow.Populate(updateInfo, branch);
            return detailsWindow;
        }

        public static UITitleScreenHypocrisisSkin ShowTitleScreenHypocrisisSkin(Transform transform)
        {
            return ModUIManager.Instance.Show<UITitleScreenHypocrisisSkin>(ModAssetBundles.HYPOCRISIS, UI_TITLE_SCREEN_HYPOCRISIS_SKIN, transform);
        }

        public static UIPersonalizationEditorExportAllMenu ShowPersonalizationEditorExportAllMenu(Transform transform)
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorExportAllMenu>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_EXPORT_ALL_MENU, transform);
        }

        public static UIPersonalizationEditorItemImportHelper ShowPersonalizationEditorItemImportHelper(Transform transform)
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorItemImportHelper>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_ITEM_IMPORT_HELPER, transform);
        }

        public static UIPersonalizationEditorScreenshotControls ShowPersonalizationEditorScreenshotControls()
        {
            return ModUIManager.Instance.Show<UIPersonalizationEditorScreenshotControls>(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_SCREENSHOT_CONTROLS, ModUIManager.UILayer.BeforeEscMenu);
        }

        public static void HidePersonalizationEditorScreenshotControls()
        {
            ModUIManager.Instance.Hide(ModAssetBundles.UI, UI_PERSONALIZATION_EDITOR_SCREENSHOT_CONTROLS);
        }
    }
}