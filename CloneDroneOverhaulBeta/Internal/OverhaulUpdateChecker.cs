using CDOverhaul.DevTools;
using CDOverhaul.HUD;
using CDOverhaul.NetworkAssets;
using ModBotWebsiteAPI;
using System;
using System.Text.RegularExpressions;

namespace CDOverhaul
{
    public static class OverhaulUpdateChecker
    {
        public static int DownloadedVersionNumber
        {
            get;
            private set;
        }

        public static string DownloadedVersionString
        {
            get;
            private set;
        }

        public static Version DownloadedVersion
        {
            get;
            private set;
        }

        public static string DownloadedDescription
        {
            get;
            private set;
        }

        public static bool HasCheckedVersion
        {
            get;
            private set;
        }

        public static bool CouldntCheckVersion
        {
            get;
            private set;
        }

        public static bool HasNewUpdate()
        {
            return OverhaulVersion.IsModBotBuild
                ? OverhaulMod.Base.ModInfo.Version < DownloadedVersionNumber
                : OverhaulVersion.modVersion < DownloadedVersion;
        }

        public static void CheckForUpdates()
        {
            if (HasCheckedVersion || !ModSetupWindow.HasSetTheModUp)
                return;

            HasCheckedVersion = true;
            if (OverhaulVersion.IsModBotBuild)
            {
                checkForUpdateOnModBotSite();
                return;
            }
            checkForUpdateOnGitHub();
        }

        private static void checkForUpdateOnModBotSite()
        {
            API.GetModData(OverhaulVersion.ModID, delegate (JsonObject jsonObject)
            {
                if (jsonObject == null || string.IsNullOrEmpty(jsonObject.RawData))
                {
                    CouldntCheckVersion = true;
                    return;
                }

                DownloadedVersionNumber = (int)(long)jsonObject["Version"];
                DownloadedVersionString = DownloadedVersionNumber.ToString();
                DownloadedDescription = (string)jsonObject["Description"];
                if (!GameModeManager.IsOnTitleScreen() || !OverhaulCompatibilityChecker.playingOnSupportedVersion || !HasNewUpdate())
                    return;

                showModBotWindow();
            });
        }

        private static void checkForUpdateOnGitHub()
        {
            OverhaulDownloadInfo downloadInfo = new OverhaulDownloadInfo();
            downloadInfo.DoneAction = delegate
            {
                if (downloadInfo.Error)
                {
                    CouldntCheckVersion = true;
                    return;
                }

                DownloadedVersionString = Regex.Replace(downloadInfo.DownloadedText, @"\r\n?|\n", string.Empty).Replace("a", string.Empty);
                DownloadedVersion = new Version(DownloadedVersionString);
                if (!GameModeManager.IsOnTitleScreen() || !OverhaulCompatibilityChecker.playingOnSupportedVersion || !HasNewUpdate())
                    return;

                showGitHubWindow();
            };
            OverhaulNetworkAssetsController.DownloadData("https://raw.githubusercontent.com/aTVCat/CloneDroneOverhaul/MainBranch/Version.txt", downloadInfo);
        }

        [DebugAction("[Mod-Bot] New version message")]
        private static void showModBotWindow()
        {
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
            OverhaulFullscreenDialogueWindow window = OverhaulFullscreenDialogueWindow.Instance;
            window.ResetContents();
            window.SetTitle("New update available!");
            window.SetDescription("Changelog:\n" + DownloadedDescription);
            window.SetWindowSize(600f, 450f);
            window.AddButton("Ignore", delegate
            {
                if (GameModeManager.IsOnTitleScreen())
                    GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
                window.Hide();
            });
            window.AddButton("Get update", delegate
            {
                if (GameModeManager.IsOnTitleScreen())
                    GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
                UnityEngine.Application.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
                window.Hide();
            }, "#FFFFFF", null, 100f);
            window.Show();
        }

        [DebugAction("[GitHub] New version message")]
        private static void showGitHubWindow()
        {
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
            OverhaulFullscreenDialogueWindow window = OverhaulFullscreenDialogueWindow.Instance;
            window.ResetContents();
            window.SetTitle("New update available!");
            window.SetDescription("There's a new test build available on GitHub!\nGo check it out!");
            window.SetWindowSize(300f, 170f);
            window.AddButton("Ignore", delegate
            {
                GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
                window.Hide();
            });
            window.AddButton("Get update", delegate
            {
                GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
                UnityEngine.Application.OpenURL("https://github.com/aTVCat/CloneDroneOverhaul/releases");
                window.Hide();
            }, "#FFFFFF", null, 100f);
            window.Show();
        }
    }
}
