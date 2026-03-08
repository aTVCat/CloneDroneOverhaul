using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UICrashScreen : OverhaulUIBehaviour
    {
        public static bool HasSentReport;

        public override bool enableCursor => true;

        public override bool closeOnEscapeButtonPress => false;

        [UIElement("StackTrace")]
        private readonly Text _stackTraceText;

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
            expandButton.rectTransform = _stackTracePanel;
            expandButton.collapsedSize = new Vector2(-50f, 175f);
            expandButton.expandedSize = new Vector2(-50f, 350f);

            _ignoreCrashesToggle.isOn = CrashManager.IgnoreCrashes;
            _sendReportButton.interactable = !HasSentReport;
        }

        public void SetStackTraceText(string message)
        {
            _stackTraceText.text = message;
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
