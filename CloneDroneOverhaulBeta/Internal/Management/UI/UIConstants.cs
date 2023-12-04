using CDOverhaul.Patches;
using UnityEngine;

namespace CDOverhaul.HUD
{
    public static class UIConstants
    {
        public const string UI_TEST = "UI_TestElements";
        public static void ShowUITest()
        {
            _ = OverhaulUIManager.reference.Show<UITestElements>(UI_TEST);
        }

        public const string UI_VERSION_LABEL = "UI_VersionLabel";
        public static void ShowVersionLabel()
        {
            _ = OverhaulUIManager.reference.Show<UIOverhaulVersionLabel>(UI_VERSION_LABEL);
        }

        public const string UI_SETTINGS_MENU = "UI_SettingsMenu";
        public static void ShowSettingsMenu()
        {
            _ = OverhaulUIManager.reference.Show<UISettingsMenu>(UI_SETTINGS_MENU);
        }

        public const string UI_FEEDBACK_MENU = "UI_FeedbackUI";
        public static void ShowFeedbackMenu()
        {
            _ = OverhaulUIManager.reference.Show<UIFeedbackMenu>(UI_FEEDBACK_MENU);
        }

        public const string UI_ABOUT_MOD = "UI_AboutMod";
        public static void ShowAboutMod()
        {
            _ = OverhaulUIManager.reference.Show<UIAboutMod>(UI_ABOUT_MOD);
        }

        public const string UI_CHANGELOG = "UI_Changelog";
        public static void ShowChangelogPanel()
        {
            _ = OverhaulUIManager.reference.Show<UIChangelog>(UI_CHANGELOG);
        }

        public const string UI_IMAGE_VIEWER = "UI_ImageViewer";
        public static void ShowImageViewer(Texture2D texture)
        {
            _ = OverhaulUIManager.reference.Show<UIImageViewer>(UI_IMAGE_VIEWER, new object[] { texture });
        }

        public const string UI_ERROR_SCREEN = "UI_ErrorScreen";
        public static void ShowErrorScreen(string error, string stacktrace)
        {
            _ = OverhaulUIManager.reference.Show<UIErrorScreen>(UI_ERROR_SCREEN, new object[] { error, stacktrace });
        }

        public const string UI_PAUSE_MENU = "UI_PauseMenu";
        public static void ShowPauseScreen()
        {
            _ = OverhaulUIManager.reference.Show<UIPauseMenu>(UI_PAUSE_MENU);
        }

        public const string UI_NEW_TITLE_SCREEN = "UI_NewTitleScreen";
        public static void ShowNewTitleScreen()
        {
            _ = OverhaulUIManager.reference.Show<UITitleScreenRework>(UI_NEW_TITLE_SCREEN);
        }

        public static class Arguments
        {
            public const string DONT_UPDATE_EFFECTS = "dontUpdateEffects";
        }
    }
}
