using OverhaulMod.Content.Personalization.Objects;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class UIElementPEInspectorGroup : OverhaulUIBehaviour
    {
        private RectTransform _rectTransform;

        private float _height;

        private PersonalizationEditorComponent _component;

        protected override void OnInitialized()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void PopulateFields(PersonalizationEditorComponent component, UIElementPEInspectorFieldsStore fieldsStore)
        {
            _component = component;
            component.InstantiateSettingsForInspector(this, fieldsStore);
        }

        public void SetHeight(float value)
        {
            _height = value;
            RefreshHeight();
        }

        public void RefreshHeight()
        {
            Vector2 sizeDelta = _rectTransform.sizeDelta;
            sizeDelta.y = _height;
            _rectTransform.sizeDelta = sizeDelta;
        }

        public RectTransform GetContainer() => _rectTransform;
    }
}
