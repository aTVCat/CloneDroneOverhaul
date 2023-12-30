using OverhaulMod.Utils;
using UnityEngine;
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

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                ModUIConstants.ShowWorkshopItemPageWindow(base.transform);
            }
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
