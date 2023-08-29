using CDOverhaul.DevTools;

namespace CDOverhaul
{
    internal static class OverhaulCompatibilityChecker
    {
        private static bool s_HasCheckedVersionCompatibility;

        private static bool s_playingOnSupportedVersion;
        public static bool playingOnSupportedVersion
        {
            get
            {
                if (!s_HasCheckedVersionCompatibility)
                {
                    s_playingOnSupportedVersion = Equals(VersionNumberManager.Instance?.GetVersionString(), OverhaulVersion.TargetGameVersion);
                    s_HasCheckedVersionCompatibility = true;
                }
                return s_playingOnSupportedVersion;
            }
        }

        public static void CheckGameVersion()
        {
            if (!playingOnSupportedVersion)
                DelegateScheduler.Instance.Schedule(ShowUnsupportedVersionDialogue, 3f);
        }

        [DebugAction("Unsupported version")]
        public static void ShowUnsupportedVersionDialogue()
        {
            OverhaulFullscreenDialogueWindow window = OverhaulFullscreenDialogueWindow.Instance;
            if (!window)
                return;

            window.ResetContents();
            window.SetTitle("Unsupported game version");
            window.SetDescription("Current Overhaul mod build is made for version " + OverhaulVersion.TargetGameVersion + " of the game.\nYou may encounter bugs and crashes.\nIt's recommended to disable the mod or update one (If \"Get update\" button is active), but you can ignore this message.");
            window.SetIcon(OverhaulFullscreenDialogueWindow.IconType.Warn);
            window.SetWindowSize(500f, 250f);
            window.AddButton("OK", delegate
            {
                GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
                window.Hide();
            });
            window.AddButton("Get update", delegate
            {
                GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
                UnityEngine.Application.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
                window.Hide();
            }, "#FFFFFF", OverhaulUpdateChecker.HasNewUpdate, 100f);
            window.Show();
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
        }
    }
}
