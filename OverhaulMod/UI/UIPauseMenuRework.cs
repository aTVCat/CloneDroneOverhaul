using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPauseMenuRework : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("ResumeButton")]
        private readonly Button m_resumeButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("LegacyUIButton")]
        private readonly Button m_legacyUIButton;

        public override bool enableCursorIfVisible
        {
            get
            {
                return true;
            }
        }

        public static bool disableOverhauledVersion { get; set; }

        public override void Show()
        {
            base.Show();
            TimeManager.Instance.OnGamePaused();
        }

        public override void Hide()
        {
            base.Hide();
            TimeManager.Instance.OnGameUnPaused();
        }

        public void OnLegacyUIButtonClicked()
        {
            Hide();
            ModUIUtility.ShowVanillaEscMenu();
        }
    }
}
