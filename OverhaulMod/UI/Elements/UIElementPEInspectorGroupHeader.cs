using OverhaulMod.Content.Personalization.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPEInspectorGroupHeader : OverhaulUIBehaviour
    {
        [UIElement("Icon")]
        private readonly Image _icon;

        [UIElement("Name")]
        private readonly Text _name;

        [UIElement("Arrow")]
        private readonly RectTransform _arrow;

        private UIElementPEInspectorGroup _group;

        private PersonalizationEditorComponent _component;

        private bool _isExpanded;

        protected override void OnInitialized()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(onClicked);
        }

        public void SetComponent(PersonalizationEditorComponent component)
        {
            _component = component;
            _name.text = component.GetDisplayName();
            _icon.sprite = component.GetIcon();
            _icon.enabled = _icon.sprite;
        }

        public void SetGroup(UIElementPEInspectorGroup group)
        {
            _group = group;
        }

        public void SetIsExpanded(bool value)
        {
            float arrowRotation = value ? -90f : 0f;
            _arrow.localEulerAngles = new Vector3(0f, 0f, arrowRotation);
            _group.gameObject.SetActive(value);

            _isExpanded = value;
        }

        private void onClicked()
        {
            SetIsExpanded(!_isExpanded);
        }
    }
}
