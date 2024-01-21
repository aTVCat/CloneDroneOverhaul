using CDOverhaul.DevTools;

namespace CDOverhaul
{
    internal class OverhaulCompatibilityChecker
    {
        public static bool CurrentBuildRunsOnSupportedVersion() => Equals(VersionNumberManager.Instance.GetVersionString(), OverhaulVersion.TargetGameVersion);

        public static void CheckGameVersion()
        {
            if (!OverhaulSessionController.GetKey<bool>("HasNotifiedPlayerAboutUnsupportedVersion"))
            {
                OverhaulSessionController.SetKey("HasNotifiedPlayerAboutUnsupportedVersion", true);
                if (!CurrentBuildRunsOnSupportedVersion())
                    DelegateScheduler.Instance.Schedule(ShowDialogueWindow, 3f);
            }
        }

        [DebugAction("Unsupported version")]
        public static void ShowDialogueWindow()
        {
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
            OverhaulFullscreenDialogueWindow window = OverhaulFullscreenDialogueWindow.Instance;
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
        }
    }
}
