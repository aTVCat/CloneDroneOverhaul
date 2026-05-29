using OverhaulMod.Content.Personalization.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI.Elements
{
    public class UIElementPEInspectorRotatorSettings : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnPreviewToggled))]
        [UIElement("PreviewToggle")]
        private readonly Toggle _previewToggle;

        [UIElementAction(nameof(OnAxisChanged))]
        [UIElement("AxisField", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field _axisField;

        private PersonalizationEditorRotator _rotatorComponent;

        private bool _disableCallbacks;

        public void SetComponent(PersonalizationEditorRotator rotatorComponent)
        {
            _rotatorComponent = rotatorComponent;
        }

        public void SetValuesFromComponent()
        {
            _disableCallbacks = true;
            _previewToggle.isOn = _rotatorComponent.PreviewInEditor;
            _axisField.Vector = _rotatorComponent.Direction;
            _disableCallbacks = false;
        }

        public void OnPreviewToggled(bool value)
        {
            if (_disableCallbacks) return;

            _rotatorComponent.PreviewInEditor = value;
            _rotatorComponent.ResetRotation();
        }

        public void OnAxisChanged(Vector3 value)
        {
            if (_disableCallbacks) return;

            _rotatorComponent.Direction = value;
        }
    }
}