using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OverhaulMod.UI.Elements
{
    public class UIElementPersonalizationAuthorsField : OverhaulUIBehaviour
    {
        [UIElement("Text")]
        private readonly Text m_text;

        [UIElementAction(nameof(OnEditButtonClicked))]
        [UIElement("EditButton")]
        private readonly Button m_editButton;

        private List<string> m_referenceList;
        public List<string> referenceList
        {
            get
            {
                return m_referenceList;
            }
            set
            {
                m_referenceList = value;
                refreshText();
            }
        }

        private void refreshText()
        {
            m_text.text = PersonalizationEditorManager.Instance?.editingItemInfo?.GetAuthorsString();
        }

        public void OnEditButtonClicked()
        {
            UIPersonalizationEditorAuthorsEditMenu menu = ModUIConstants.ShowPersonalizationEditorAuthorsEditMenu(UIPersonalizationEditor.instance.transform);
            menu.Populate(referenceList, refreshText);
        }
    }
}
