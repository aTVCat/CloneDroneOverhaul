using OverhaulMod.Engine;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIRestartRequiredScreen : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnRestartButtonClicked))]
        [UIElement("RestartButton")]
        private readonly Button _restartButton;

        private CanvasGroup _canvasGroup;

        public override bool hideTitleScreen => true;

        protected override void OnInitialized()
        {
            _canvasGroup = base.GetComponent<CanvasGroup>();
        }

        public override void Show()
        {
            base.Show();
            _canvasGroup.alpha = 0f;
        }

        public override void Update()
        {
            _canvasGroup.alpha += Time.unscaledDeltaTime * 5f;
        }

        public void SetAllowIgnoring(bool value)
        {
            _exitButton.gameObject.SetActive(value);
        }

        public void OnRestartButtonClicked()
        {
            ModSettingsDataManager.Instance.Save();
            _ = Process.Start("steam://rungameid/" + 597170U.ToString());
            Application.Quit();
        }
    }
}
