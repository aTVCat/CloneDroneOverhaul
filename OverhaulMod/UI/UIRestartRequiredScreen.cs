using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIRestartRequiredScreen : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnRestartButtonClicked))]
        [UIElement("RestartButton")]
        private readonly Button m_restartButton;

        private CanvasGroup m_canvasGroup;

        protected override void OnInitialized()
        {
            m_canvasGroup = base.GetComponent<CanvasGroup>();
        }

        public override void Show()
        {
            base.Show();
            SetTitleScreenButtonActive(false);
            m_canvasGroup.alpha = 0f;
        }

        public override void Hide()
        {
            base.Hide();
            SetTitleScreenButtonActive(true);
        }

        public override void Update()
        {
            m_canvasGroup.alpha += Time.unscaledDeltaTime * 10f;
        }

        public void SetAllowIgnoring(bool value)
        {
            m_exitButton.gameObject.SetActive(value);
        }

        public void OnRestartButtonClicked()
        {
            Process.Start("steam://rungameid/" + 597170U.ToString());
            Application.Quit();
        }
    }
}
