using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.HUD
{
    public static class UIConstants
    {
        public const string UI_TEST = "UI_TestElements";
        public static void ShowUITest()
        {
            OverhaulUIManager.reference.Show<UITestElements>(UI_TEST);
        }

        public const string UI_VERSION_LABEL = "UI_VersionLabel";
        public static void ShowVersionLabel()
        {
            OverhaulUIManager.reference.Show<UIOverhaulVersionLabel>(UI_VERSION_LABEL);
        }

        public const string UI_SETTINGS_MENU = "UI_SettingsMenu";
        public static void ShowSettingsMenu()
        {
            OverhaulUIManager.reference.Show<UISettingsMenu>(UI_SETTINGS_MENU);
        }

        public const string UI_FEEDBACK_MENU = "UI_FeedbackUI";
        public static void ShowFeedbackMenu()
        {
            OverhaulUIManager.reference.Show<UIFeedbackMenu>(UI_FEEDBACK_MENU);
        }

        public const string UI_ABOUT_MOD = "UI_AboutMod";
        public static void ShowAboutMod()
        {
            OverhaulUIManager.reference.Show<UIAboutMod>(UI_ABOUT_MOD);
        }

        public const string UI_CHANGELOG = "UI_Changelog";
        public static void ShowChangelogPanel()
        {
            OverhaulUIManager.reference.Show<UIChangelog>(UI_CHANGELOG);
        }

        public const string UI_IMAGE_VIEWER = "UI_ImageViewer";
        public static void ShowImageViewer(Texture2D texture)
        {
            OverhaulUIManager.reference.Show<UIImageViewer>(UI_IMAGE_VIEWER, new object[] { texture });
        }

        public const string UI_ERROR_SCREEN = "UI_ErrorScreen";
        public static void ShowErrorScreen(string error, string stacktrace)
        {
            OverhaulUIManager.reference.Show<UIErrorScreen>(UI_ERROR_SCREEN, new object[] { error, stacktrace });
        }

        public static class Arguments
        {
            public const string DONT_UPDATE_EFFECTS = "dontUpdateEffects";
        }
    }
}
