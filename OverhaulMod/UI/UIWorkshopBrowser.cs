using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIWorkshopBrowser : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("OldUIButton")]
        public Button LegacyUIButton;

        public override void Show()
        {
            base.Show();
            SetTitleScreenButtonActive(false);
        }

        public override void Hide()
        {
            base.Hide();
            SetTitleScreenButtonActive(true);
        }

        public void OnLegacyUIButtonClicked()
        {
            TitleScreenUI titleScreenUI = GameUIRoot.Instance?.TitleScreenUI;
            if (titleScreenUI)
            {
                Hide();
                titleScreenUI.OnWorkshopBrowserButtonClicked();
            }
        }
    }
}
