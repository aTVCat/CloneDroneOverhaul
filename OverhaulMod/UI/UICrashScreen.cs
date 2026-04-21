using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UICrashScreen : OverhaulUIBehaviour
    {
        public static bool HasSentReport;

        public override bool EnableCursor => true;

        public override bool CloseOnEscapeButtonPress => false;

        [UIElement("StackTrace")]
        private readonly Text _stackTraceText;

        [UIElement("DetailsLabel")]
        private readonly Text _detailsText;

        [UIElement("Overlay", false)]
        private readonly GameObject _overlay;

        [UIElementAction(nameof(OnIgnoreCrashButtonClicked))]
        [UIElement("IgnoreCrashButton")]
        private readonly Button _ignoreCrashButton;

        [UIElementAction(nameof(OnMainMenuButtonClicked))]
        [UIElement("MainMenuButton")]
        private readonly Button _mainMenuButton;

        [UIElementAction(nameof(OnExitGameButtonClicked))]
        [UIElement("ExitGameButton")]
        private readonly Button _exitGameButton;

        [UIElementAction(nameof(OnSendReportButtonClicked))]
        [UIElement("SendReportButton")]
        private readonly Button _sendReportButton;

        [UIElementAction(nameof(OnIgnoreCrashesToggleChanged))]
        [UIElement("IgnoreCrashesToggle")]
        private readonly Toggle _ignoreCrashesToggle;

        [UIElement("ExpandButton", typeof(UIElementExpandButton))]
        private readonly UIElementExpandButton _expandButton;

        [UIElement("ScrollRect")]
        private readonly RectTransform _stackTracePanel;

        protected override void OnInitialized()
        {
            UIElementExpandButton expandButton = _expandButton;
            expandButton.RectTransformReference = _stackTracePanel;
            expandButton.CollapsedSize = new Vector2(-50f, 175f);
            expandButton.ExpandedSize = new Vector2(-50f, 350f);
            expandButton.Callback = onStackTraceCollapsedOrExpanded;

            _ignoreCrashesToggle.isOn = CrashManager.IgnoreCrashes;
            _sendReportButton.interactable = !HasSentReport;
        }

        public void RefreshDetailsText()
        {
            // runtime info
            string overhaulVersion = $"Overhaul {ModBuild.Version}";
            string modBotVersion = $"Mod-Bot {ModLibrary.Properties.Resources.ModBotVersion}";
            string gameVersion = $"Clone Drone {VersionNumberManager.Instance.GetVersionString()}";
            string unityVersion = $"Unity {Application.unityVersion}";
            string platform = $"{(GameVersionManager.IsSteamBuild() ? "Steam" : "Non-Steam")}";

            // game environment info
            GameFlowManager gameFlowManager = GameFlowManager.Instance;
            string gameMode = gameFlowManager ? gameFlowManager.GetCurrentGameMode().ToString() : "N/A";

            LevelManager levelManager = LevelManager.Instance;
            string levelId = levelManager ? levelManager.GetCurrentLevelID() : "N/A";

            ArenaLiftManager arenaLiftManager = ArenaLiftManager.Instance;
            string liftTarget = arenaLiftManager && arenaLiftManager.Lift ? arenaLiftManager.GetLiftTarget().ToString() : "N/A";

            string detailsString = $"{overhaulVersion} · {modBotVersion} · {gameVersion} · {unityVersion} · {platform} | {gameMode} · {levelId} · {liftTarget}";
            _detailsText.text = detailsString;
        }

        public void SetStackTraceText(string message)
        {
            _stackTraceText.text = message;
        }

        private void onStackTraceCollapsedOrExpanded(bool isExpanded)
        {
            _overlay.SetActive(isExpanded);
        }

        public void OnIgnoreCrashButtonClicked()
        {
            Hide();

            if (ErrorManager.Instance)
                ErrorManager.Instance._hasCrashed = false;

            if (TimeManager.Instance)
                TimeManager.Instance.OnGameUnPaused();
        }

        public void OnMainMenuButtonClicked()
        {
            Hide();
            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
        }

        public void OnExitGameButtonClicked()
        {
            Application.Quit();
        }

        public void OnSendReportButtonClicked()
        {
            HasSentReport = true;
            _sendReportButton.interactable = false;
            PostmanManager.Instance.SendCrashReport(_stackTraceText.text, delegate
            {
                ModUIUtils.MessagePopupOK("Report sent!", string.Empty, true);
            }, delegate (string error)
            {
                ModUIUtils.MessagePopupOK("Report send error", error, true);
            });
        }

        public void OnIgnoreCrashesToggleChanged(bool value)
        {
            CrashManager.IgnoreCrashes = value;
        }
    }
}
