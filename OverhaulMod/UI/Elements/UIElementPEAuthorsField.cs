using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPEAuthorsField : OverhaulUIBehaviour
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
            _text.text = PersonalizationEditorManager.Instance?.EditingItemInfo?.GetAuthorsString();
        }

        public void OnEditButtonClicked()
        {
            UIPEAuthorsEditMenu menu = ModUIs.ShowPersonalizationEditorAuthorsEditMenu(UIPE.Instance.transform);
            menu.Populate(referenceList, refreshText);
        }
    }
}
