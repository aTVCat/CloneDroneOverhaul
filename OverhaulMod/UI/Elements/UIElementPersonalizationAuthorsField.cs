using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationAuthorsField : OverhaulUIBehaviour
    {
        [UIElement("Text")]
        private readonly Text _text;

        [UIElementAction(nameof(OnEditButtonClicked))]
        [UIElement("EditButton")]
        private readonly Button _editButton;

        private List<string> _referenceList;
        public List<string> referenceList
        {
            get
            {
                return _referenceList;
            }
            set
            {
                _referenceList = value;
                refreshText();
            }
        }

        private void refreshText()
        {
            _text.text = PersonalizationEditorManager.Instance?.currentEditingItemInfo?.GetAuthorsString();
        }

        public void OnEditButtonClicked()
        {
            UIPersonalizationEditorAuthorsEditMenu menu = ModUIConstants.ShowPersonalizationEditorAuthorsEditMenu(UIPersonalizationEditor.instance.transform);
            menu.Populate(referenceList, refreshText);
        }
    }
}
