using UnityEngine;
using UnityEngine.UI;
using OverhaulMod.Utils;
using OverhaulMod.Content.Personalization;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorPropertiesPanel : OverhaulUIBehaviour
    {
        [UIElement("VolumeColorsConfigPanel", typeof(UIElementPersonalizationEditorVolumeColorsSettings))]
        private readonly UIElementPersonalizationEditorVolumeColorsSettings m_volumeColorsSettings;

        [UIElementAction(nameof(OnPositionChanged))]
        [UIElement("PositionPanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field m_positionField;

        [UIElementAction(nameof(OnRotationChanged))]
        [UIElement("RotationPanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field m_rotationField;

        [UIElementAction(nameof(OnScaleChanged))]
        [UIElement("ScalePanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field m_scaleField;

        [UIElementAction(nameof(OnEditVolumeColorsButtonClicked))]
        [UIElement("EditVolumeColorsButton")]
        private readonly Button m_editVolumeColorsButton;

        private UIElementMousePositionChecker m_mousePositionChecker;

        private PersonalizationEditorObjectBehaviour m_object;

        private bool m_disableCallbacks;

        protected override void OnInitialized()
        {
            m_mousePositionChecker = base.gameObject.AddComponent<UIElementMousePositionChecker>();
        }

        public override void Update()
        {
            base.Update();
            if(Input.GetMouseButtonDown(0) && !m_mousePositionChecker.isMouseOverElement)
            {
                EditObject(null);
            }
        }

        public void EditObject(PersonalizationEditorObjectBehaviour objectBehaviour)
        {
            if (!objectBehaviour)
            {
                m_object = null;
                Hide();
                return;
            }
            m_object = objectBehaviour;
            Show();

            m_disableCallbacks = true;
            m_positionField.vector = objectBehaviour.transform.localPosition;
            m_rotationField.vector = objectBehaviour.transform.localEulerAngles;
            m_scaleField.vector = objectBehaviour.transform.localScale;
            m_editVolumeColorsButton.gameObject.SetActive(objectBehaviour.Path == "Volume");
            m_disableCallbacks = false;
        }

        public void OnEditVolumeColorsButtonClicked()
        {
            m_volumeColorsSettings.Show();
        }

        public void OnPositionChanged(Vector3 value)
        {
            if (m_disableCallbacks)
                return;

            PersonalizationEditorObjectBehaviour objectBehaviour = m_object;
            if (!objectBehaviour)
                return;

            objectBehaviour.transform.localPosition = value;
        }

        public void OnRotationChanged(Vector3 value)
        {
            if (m_disableCallbacks)
                return;

            PersonalizationEditorObjectBehaviour objectBehaviour = m_object;
            if (!objectBehaviour)
                return;

            objectBehaviour.transform.localEulerAngles = value;
        }

        public void OnScaleChanged(Vector3 value)
        {
            if (m_disableCallbacks)
                return;

            PersonalizationEditorObjectBehaviour objectBehaviour = m_object;
            if (!objectBehaviour)
                return;

            objectBehaviour.transform.localScale = value;
        }
    }
}
