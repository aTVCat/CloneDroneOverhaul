using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementTabWithText : UIElementTab
    {
        [UIElement("Text")]
        private readonly Text m_text;

        protected override void OnInitialized()
        {
            m_text.text = tabId;
        }
    }
}
