using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementTabWithText : UIElementTab
    {
        [UIElement("Text")]
        private readonly Text m_text;

        public string LocalizationID;

        protected override void OnInitialized()
        {
            m_text.text = LocalizationID.IsNullOrEmpty() ? tabId : LocalizationManager.Instance.GetTranslatedString(LocalizationID);
        }
    }
}
