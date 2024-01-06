using OverhaulMod.Content;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UINewsDetailsPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("Header")]
        private readonly Text m_titleText;

        [UIElement("Description")]
        private readonly Text m_descriptionText;

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void Populate(NewsInfo newsInfo)
        {
            m_titleText.text = newsInfo.Title;
            m_descriptionText.text = newsInfo.Description;
        }
    }
}
