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
            ModDebug.RefreshEnvironmentInfoString();
            _detailsText.text = ModDebug.GetEnvironemntInfoString();
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
