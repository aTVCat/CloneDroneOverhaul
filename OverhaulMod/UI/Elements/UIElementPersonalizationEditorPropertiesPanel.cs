using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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

        [UIElement("Content")]
        private readonly Transform m_container;

        [UIElement("FileLocationFieldDisplay", false)]
        private readonly ModdedObject m_fileLocationFieldDisplay;

        private List<FieldDisplay> m_fieldDisplays;

        private UIElementMousePositionChecker m_mousePositionChecker;

        private PersonalizationEditorObjectBehaviour m_object;

        private bool m_disableCallbacks;

        protected override void OnInitialized()
        {
            m_fieldDisplays = new List<FieldDisplay>();
            m_mousePositionChecker = base.gameObject.AddComponent<UIElementMousePositionChecker>();
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetMouseButtonDown(0) && !m_mousePositionChecker.isMouseOverElement)
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

            List<FieldDisplay> list = m_fieldDisplays;
            if (!list.IsNullOrEmpty())
            {
                foreach (FieldDisplay fd in list)
                    Destroy(fd.gameObject);

                list.Clear();
            }

            m_disableCallbacks = true;
            m_positionField.vector = objectBehaviour.transform.localPosition;
            m_rotationField.vector = objectBehaviour.transform.localEulerAngles;
            m_scaleField.vector = objectBehaviour.transform.localScale;
            m_editVolumeColorsButton.gameObject.SetActive(objectBehaviour.Path == "Volume");

            foreach (PersonalizationEditorObjectPropertyAttribute attribute in objectBehaviour.GetProperties())
            {
                FieldDisplay fieldDisplay = null;
                if (attribute.propertyInfo.PropertyType == typeof(string))
                {
                    if (attribute.IsFileLocation)
                    {
                        fieldDisplay = Instantiate(m_fileLocationFieldDisplay, m_container).gameObject.AddComponent<FileLocationField>();
                    }
                }

                if (fieldDisplay)
                {
                    fieldDisplay.gameObject.SetActive(true);
                    fieldDisplay.InitializeElement();
                    list.Add(fieldDisplay);

                    PersonalizationEditorObjectComponentBase cb = (PersonalizationEditorObjectComponentBase)objectBehaviour.GetComponent(attribute.propertyInfo.DeclaringType);
                    fieldDisplay.Set(attribute, cb, attribute.propertyInfo.GetValue(cb));
                }
            }

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

        public class FieldDisplay : OverhaulUIBehaviour
        {
            protected PersonalizationEditorObjectPropertyAttribute m_attribute;

            protected PersonalizationEditorObjectComponentBase m_componentBaseObject;

            public virtual void Set(PersonalizationEditorObjectPropertyAttribute attribute, PersonalizationEditorObjectComponentBase componentBaseObject, object value)
            {
                m_attribute = attribute;
                m_componentBaseObject = componentBaseObject;
            }
        }

        public class FileLocationField : FieldDisplay
        {
            [UIElement("InputField")]
            private readonly InputField m_field;

            [UIElementAction(nameof(OnEditButtonClicked))]
            [UIElement("EditButton")]
            private readonly Button m_editButton;

            public override void Set(PersonalizationEditorObjectPropertyAttribute attribute, PersonalizationEditorObjectComponentBase componentBaseObject, object value)
            {
                base.Set(attribute, componentBaseObject, value);
                m_field.text = value?.ToString();
            }

            public void OnEditButtonClicked()
            {
                ModUIUtils.FileExplorer(UIPersonalizationEditor.instance.transform, true, delegate (string filePath)
                {
                    string directoryName = ModIOUtils.GetDirectoryName(PersonalizationEditorManager.Instance.editingItemInfo.FolderPath);
                    string fileName = Path.GetFileName(filePath);
                    string path = $"{directoryName}/files/{fileName}";
                    Debug.Log(directoryName);
                    Debug.Log(fileName);
                    Debug.Log(path);

                    if ((string)m_attribute.propertyInfo.GetValue(m_componentBaseObject) == path)
                        return;

                    m_field.text = path;
                    m_attribute.propertyInfo.SetValue(m_componentBaseObject, path);
                }, PersonalizationItemInfo.GetImportedFilesFolder(PersonalizationEditorManager.Instance.editingItemInfo), m_attribute.FileLocationSearchPattern);
            }
        }
    }
}
