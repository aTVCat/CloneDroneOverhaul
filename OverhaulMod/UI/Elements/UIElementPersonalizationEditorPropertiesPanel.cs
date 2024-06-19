using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorPropertiesPanel : OverhaulUIBehaviour
    {
        [UIElement("VolumeColorsConfigPanel", typeof(UIElementPersonalizationEditorVolumeColorsSettings), false)]
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

        [UIElement("ForceVolumeSettingsPresetDropdown", false)]
        private readonly ModdedObject m_forceVolumeSettingsPresetDropdown;

        [UIElement("VolumeSettingsPresetDisplay", false)]
        private readonly ModdedObject m_volumeSettingsPresetDisplay;

        [UIElement("AddVolumeSettingsPresetButton", false)]
        private readonly Button m_addVolumeSettingsPresetButton;

        [UIElement("Content")]
        private readonly Transform m_container;

        [UIElement("NothingToEditOverlay", true)]
        private readonly GameObject m_nothingToEditOverlay;

        private UIElementMouseEventsComponent m_mousePositionChecker;

        private PersonalizationEditorObjectBehaviour m_object;

        private PersonalizationEditorObjectVolume m_volume;

        private VolumePropertiesController m_volumePropertiesController;

        private bool m_disableCallbacks;

        protected override void OnInitialized()
        {
            m_volumePropertiesController = new VolumePropertiesController();

            m_mousePositionChecker = base.gameObject.AddComponent<UIElementMouseEventsComponent>();
            m_volumeColorsSettings.onColorChanged = OnVolumeColorReplacementsChanged;
        }

        public void Clear()
        {
            TransformUtils.DestroyAllChildren(m_container);
        }

        public void EditObject(PersonalizationEditorObjectBehaviour objectBehaviour)
        {
            bool isNotNull = objectBehaviour;

            m_nothingToEditOverlay.SetActive(!isNotNull);
            if (!isNotNull)
            {
                m_object = null;
                m_volume = null;
                return;
            }
            m_object = objectBehaviour;
            m_volume = objectBehaviour.GetComponent<PersonalizationEditorObjectVolume>();

            Clear();

            m_disableCallbacks = true;
            m_positionField.vector = objectBehaviour.transform.localPosition;
            m_rotationField.vector = objectBehaviour.transform.localEulerAngles;
            m_scaleField.vector = objectBehaviour.transform.localScale;

            if (m_volume)
            {
                m_volumePropertiesController.PopulateFields(this, m_container, objectBehaviour);
            }

            /*
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
            }*/

            m_disableCallbacks = false;
        }

        public void OnEditVolumeColorsButtonClicked()
        {
            PersonalizationEditorObjectVolume volume = m_volume;
            if (!volume)
                return;

            m_volumeColorsSettings.Show();
            //m_volumeColorsSettings.Populate(volume.colorReplacements);
        }

        public void OnVolumeColorReplacementsChanged(string str)
        {
            /*
            PersonalizationEditorObjectVolume volume = m_volume;
            if (volume)
                volume.colorReplacements = str;*/
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

        public class ObjectPropertiesController
        {
            public virtual void PopulateFields(UIElementPersonalizationEditorPropertiesPanel propertiesPanel, Transform container, PersonalizationEditorObjectBehaviour objectBehaviour)
            {

            }
        }

        public class VolumePropertiesController : ObjectPropertiesController
        {
            public override void PopulateFields(UIElementPersonalizationEditorPropertiesPanel propertiesPanel, Transform container, PersonalizationEditorObjectBehaviour objectBehaviour)
            {
                void populateFieldsAction()
                {
                    propertiesPanel.Clear();
                    propertiesPanel.m_volumePropertiesController.PopulateFields(propertiesPanel, container, objectBehaviour);
                }

                PersonalizationEditorObjectVolume volume = objectBehaviour.GetComponent<PersonalizationEditorObjectVolume>();

                /*
                ModdedObject forceConditionDropdown = Instantiate(propertiesPanel.m_forceVolumeSettingsPresetDropdown, container);
                Dropdown dropdown = forceConditionDropdown.GetObject<Dropdown>(0);
                dropdown.options = volume.GetConditionOptions();
                forceConditionDropdown.gameObject.SetActive(true);*/

                Dictionary<PersonalizationEditorObjectShowConditions, VolumeSettingsPreset> volumePresets = volume.volumeSettingPresets;
                if (volumePresets != null && volumePresets.Count != 0)
                {
                    foreach (KeyValuePair<PersonalizationEditorObjectShowConditions, VolumeSettingsPreset> preset in volumePresets)
                    {
                        ModdedObject display = Instantiate(propertiesPanel.m_volumeSettingsPresetDisplay, container);
                        display.gameObject.SetActive(true);

                        bool allowCallback = true;
                        PersonalizationEditorObjectShowConditions prevCondition = preset.Key;
                        Dropdown conditionsDropdown = display.GetObject<Dropdown>(0);
                        conditionsDropdown.options = PersonalizationEditorManager.Instance.GetConditionOptions();
                        conditionsDropdown.value = ((int)prevCondition) - 1;
                        conditionsDropdown.onValueChanged.AddListener(delegate (int value)
                        {
                            if (!allowCallback)
                                return;

                            PersonalizationEditorObjectShowConditions condition = (PersonalizationEditorObjectShowConditions)(value + 1);
                            if (volumePresets.ContainsKey(condition))
                            {
                                allowCallback = false;
                                conditionsDropdown.value = ((int)prevCondition) - 1;
                                allowCallback = true;

                                ModUIUtils.MessagePopupOK("Cannot change preset usage condition", "Another preset is already using this condition");
                            }
                            else
                            {
                                VolumeSettingsPreset prst = volumePresets[prevCondition];
                                _ = volumePresets.Remove(prevCondition);
                                volumePresets.Add(condition, prst);

                                prevCondition = condition;
                                GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                            }
                        });
                        conditionsDropdown.interactable = volume.GetUnusedShowCondition() != PersonalizationEditorObjectShowConditions.None;

                        Action refreshActiveFrameAction = delegate
                        {
                            display.GetObject<GameObject>(4).SetActive(prevCondition == PersonalizationEditorManager.Instance.previewPresetKey);
                        };
                        refreshActiveFrameAction();

                        EventController singleEventController = display.gameObject.AddComponent<EventController>();
                        singleEventController.AddEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, refreshActiveFrameAction);
                        singleEventController.AddEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, refreshActiveFrameAction);
                    }
                }

                if (volume.GetUnusedShowCondition() != PersonalizationEditorObjectShowConditions.None)
                {
                    Button newPresetButton = Instantiate(propertiesPanel.m_addVolumeSettingsPresetButton, container);
                    newPresetButton.gameObject.SetActive(true);
                    newPresetButton.onClick.AddListener(delegate
                    {
                        volume.volumeSettingPresets.Add(volume.GetUnusedShowCondition(), new VolumeSettingsPreset());
                        populateFieldsAction();
                    });
                }
            }
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
                    string directoryName = ModIOUtils.GetDirectoryName(PersonalizationEditorManager.Instance.currentEditingItemInfo.FolderPath);
                    string fileName = Path.GetFileName(filePath);
                    string path = $"{directoryName}/files/{fileName}";

                    if ((string)m_attribute.propertyInfo.GetValue(m_componentBaseObject) == path)
                        return;

                    m_field.text = path;
                    m_attribute.propertyInfo.SetValue(m_componentBaseObject, path);
                }, PersonalizationItemInfo.GetImportedFilesFolder(PersonalizationEditorManager.Instance.currentEditingItemInfo), m_attribute.FileLocationSearchPattern);
            }
        }
    }
}
