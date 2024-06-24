using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Globalization;
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

        [UIElement("EnableIfPresetDropdown", false)]
        private readonly ModdedObject m_enableIfPresetDropdown;

        [UIElement("VolumeExtraSettings", false)]
        private readonly ModdedObject m_volumeExtraSettings;

        [UIElement("AddVolumeSettingsPresetButton", false)]
        private readonly Button m_addVolumeSettingsPresetButton;

        [UIElement("Content")]
        private readonly Transform m_container;

        [UIElement("NothingToEditOverlay", true)]
        private readonly GameObject m_nothingToEditOverlay;

        private UIElementMouseEventsComponent m_mousePositionChecker;

        private int m_objectId;

        private PersonalizationEditorObjectBehaviour m_object;

        private PersonalizationEditorObjectVolume m_volume;

        private VolumePropertiesController m_volumePropertiesController;

        private VisibilityPropertiesController m_visibilityPropertiesController;

        private bool m_disableCallbacks;

        protected override void OnInitialized()
        {
            m_objectId = -1;
            m_volumePropertiesController = new VolumePropertiesController();
            m_visibilityPropertiesController = new VisibilityPropertiesController();

            m_mousePositionChecker = base.gameObject.AddComponent<UIElementMouseEventsComponent>();
            m_volumeColorsSettings.onColorChanged = OnVolumeColorReplacementsChanged;
        }

        public void Clear()
        {
            TransformUtils.DestroyAllChildren(m_container);
        }

        public void Refresh()
        {
            EditObject(m_object);
        }

        public void EditObjectAgain()
        {
            if (m_objectId == -1)
                return;

            EditObject(PersonalizationEditorObjectManager.Instance.GetInstantiatedObject(m_objectId));
        }

        public void EditObject(PersonalizationEditorObjectBehaviour objectBehaviour)
        {
            bool isNotNull = objectBehaviour;

            ModUIConstants.HideGenericColorPicker();
            m_volumeColorsSettings.Hide();
            m_nothingToEditOverlay.SetActive(!isNotNull);
            if (!isNotNull)
            {
                m_objectId = -1;
                m_object = null;
                m_volume = null;
                return;
            }
            m_objectId = objectBehaviour.UniqueIndex;
            m_object = objectBehaviour;
            m_volume = objectBehaviour.GetComponent<PersonalizationEditorObjectVolume>();

            Clear();

            m_disableCallbacks = true;
            m_positionField.vector = objectBehaviour.transform.localPosition;
            m_rotationField.vector = objectBehaviour.transform.localEulerAngles;
            m_scaleField.vector = objectBehaviour.transform.localScale;

            if (objectBehaviour.GetComponent<PersonalizationEditorObjectVisibilityController>())
            {
                m_visibilityPropertiesController.PopulateFields(this, m_container, objectBehaviour);
            }

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

        public class VisibilityPropertiesController : ObjectPropertiesController
        {
            public override void PopulateFields(UIElementPersonalizationEditorPropertiesPanel propertiesPanel, Transform container, PersonalizationEditorObjectBehaviour objectBehaviour)
            {
                ModdedObject enableIfPresetDropdown = Instantiate(propertiesPanel.m_enableIfPresetDropdown, container);
                enableIfPresetDropdown.gameObject.SetActive(true);

                PersonalizationEditorObjectVisibilityController visibilityController = objectBehaviour.GetComponent<PersonalizationEditorObjectVisibilityController>();

                Dropdown dropdown = enableIfPresetDropdown.GetObject<Dropdown>(0);
                dropdown.options = PersonalizationEditorManager.Instance.GetConditionOptionsDependingOnEditingWeapon(true);

                int conditionDropdownValueToSet = -1;
                for (int i = 0; i < dropdown.options.Count; i++)
                {
                    if (dropdown.options[i] is DropdownWeaponVariantOptionData showConditionOptionData && showConditionOptionData.Value == visibilityController.enableIfWeaponVariant)
                    {
                        conditionDropdownValueToSet = i;
                    }
                }

                if(conditionDropdownValueToSet == -1)
                {
                    dropdown.options.Add(new DropdownWeaponVariantOptionData(visibilityController.enableIfWeaponVariant));
                    dropdown.RefreshShownValue();
                    conditionDropdownValueToSet = dropdown.options.Count - 1;
                }

                dropdown.value = conditionDropdownValueToSet;
                dropdown.onValueChanged.AddListener(delegate (int value)
                {
                    WeaponVariant weaponVariant = (dropdown.options[value] as DropdownWeaponVariantOptionData).Value;
                    visibilityController.enableIfWeaponVariant = weaponVariant;
                    visibilityController.RefreshVisibility();
                });
            }
        }

        public class VolumePropertiesController : ObjectPropertiesController
        {
            public override void PopulateFields(UIElementPersonalizationEditorPropertiesPanel propertiesPanel, Transform container, PersonalizationEditorObjectBehaviour objectBehaviour)
            {
                void populateFieldsAction()
                {
                    propertiesPanel.Refresh();
                }

                PersonalizationEditorObjectVolume volume = objectBehaviour.GetComponent<PersonalizationEditorObjectVolume>();

                ModdedObject volumeExtraSettings = Instantiate(propertiesPanel.m_volumeExtraSettings, container);
                volumeExtraSettings.gameObject.SetActive(true);
                Toggle hideIfNoPresetToggle = volumeExtraSettings.GetObject<Toggle>(0);
                hideIfNoPresetToggle.isOn = volume.hideIfNoPreset;
                hideIfNoPresetToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    volume.hideIfNoPreset = value;
                    GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                });

                Dictionary<WeaponVariant, VolumeSettingsPreset> volumePresets = volume.volumeSettingPresets;
                if (volumePresets != null && volumePresets.Count != 0)
                {
                    foreach (KeyValuePair<WeaponVariant, VolumeSettingsPreset> preset in volumePresets)
                    {
                        VolumeSettingsPreset settingsPreset = preset.Value;

                        ModdedObject display = Instantiate(propertiesPanel.m_volumeSettingsPresetDisplay, container);
                        display.gameObject.SetActive(true);

                        // voxel model file
                        ModdedObject voxelModelFileField = display.GetObject<ModdedObject>(1);
                        InputField voxelModelFileFieldText = voxelModelFileField.GetObject<InputField>(0);
                        voxelModelFileFieldText.text = settingsPreset.VoxFilePath;
                        voxelModelFileField.GetObject<Button>(1).onClick.AddListener(delegate
                        {
                            ModUIUtils.FileExplorer(UIPersonalizationEditor.instance.transform, true, delegate (string filePath)
                            {
                                if (filePath.IsNullOrEmpty())
                                {
                                    voxelModelFileFieldText.text = string.Empty;
                                    settingsPreset.VoxFilePath = string.Empty;
                                }
                                else
                                {
                                    string directoryName = ModIOUtils.GetDirectoryName(PersonalizationEditorManager.Instance.currentEditingItemInfo.FolderPath);
                                    string fileName = Path.GetFileName(filePath);
                                    string path = Path.Combine(directoryName, "files", fileName);

                                    if (settingsPreset.VoxFilePath == path)
                                        return;
                                    else
                                    {
                                        settingsPreset.ColorReplacements = null; // reset color replacements for other palette
                                        Dictionary<string, FavoriteColorSettings> d = settingsPreset.ReplaceWithFavoriteColors;
                                        if (d == null)
                                            settingsPreset.ReplaceWithFavoriteColors = new Dictionary<string, FavoriteColorSettings>();
                                        else
                                            d.Clear();
                                    }

                                    voxelModelFileFieldText.text = path;
                                    settingsPreset.VoxFilePath = path;
                                }

                                GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                            }, PersonalizationItemInfo.GetImportedFilesFolder(PersonalizationEditorManager.Instance.currentEditingItemInfo), "*.vox");
                        });

                        // center pivot
                        Toggle centerPivotToggle = display.GetObject<Toggle>(2);
                        centerPivotToggle.isOn = settingsPreset.CenterPivot;
                        centerPivotToggle.onValueChanged.AddListener(delegate (bool value)
                        {
                            settingsPreset.CenterPivot = value;
                            GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                        });

                        // conditions dropdown
                        bool allowCallback = true;
                        WeaponVariant prevCondition = preset.Key;
                        Dropdown conditionsDropdown = display.GetObject<Dropdown>(0);
                        conditionsDropdown.options = PersonalizationEditorManager.Instance.GetConditionOptionsDependingOnEditingWeapon();

                        int conditionDropdownValueToSet = -1;
                        for (int i = 0; i < conditionsDropdown.options.Count; i++)
                        {
                            if (conditionsDropdown.options[i] is DropdownWeaponVariantOptionData showConditionOptionData && showConditionOptionData.Value == prevCondition)
                            {
                                conditionDropdownValueToSet = i;
                            }
                        }

                        if (conditionDropdownValueToSet == -1)
                        {
                            conditionsDropdown.options.Add(new DropdownWeaponVariantOptionData(prevCondition));
                            conditionsDropdown.RefreshShownValue();

                            conditionDropdownValueToSet = conditionsDropdown.options.Count - 1;
                        }

                        conditionsDropdown.value = conditionDropdownValueToSet;
                        conditionsDropdown.onValueChanged.AddListener(delegate (int value)
                        {
                            if (!allowCallback)
                                return;

                            WeaponVariant condition = (conditionsDropdown.options[value] as DropdownWeaponVariantOptionData).Value;
                            if (volumePresets.ContainsKey(condition))
                            {
                                allowCallback = false;
                                conditionsDropdown.value = ((int)prevCondition) - 1;
                                allowCallback = true;

                                ModUIUtils.MessagePopupOK("Cannot change preset usage condition", "Another preset is already using this condition");
                            }
                            else
                            {
                                _ = volumePresets.Remove(prevCondition);
                                volumePresets.Add(condition, settingsPreset);

                                prevCondition = condition;
                                GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                            }
                        });
                        conditionsDropdown.interactable = volume.GetUnusedShowCondition() != WeaponVariant.None;

                        // active frame
                        void refreshActiveFrameAction()
                        {
                            display.GetObject<GameObject>(4).SetActive(prevCondition == PersonalizationEditorManager.Instance.previewPresetKey);
                        }
                        refreshActiveFrameAction();

                        EventController singleEventController = display.gameObject.AddComponent<EventController>();
                        singleEventController.AddEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, refreshActiveFrameAction);
                        singleEventController.AddEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, refreshActiveFrameAction);

                        // colors
                        void onColorChangedAction(string value)
                        {
                            settingsPreset.ColorReplacements = value;
                            GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                        }

                        display.GetObject<Button>(3).onClick.AddListener(delegate
                        {
                            UIElementPersonalizationEditorVolumeColorsSettings volumeColorsSettings = propertiesPanel.m_volumeColorsSettings;
                            volumeColorsSettings.Show();
                            volumeColorsSettings.Populate(settingsPreset.ColorReplacements, settingsPreset.ReplaceWithFavoriteColors);
                            volumeColorsSettings.onColorChanged = onColorChangedAction;
                        });

                        // delete
                        display.GetObject<Button>(5).onClick.AddListener(delegate
                        {
                            _ = volumePresets.Remove(prevCondition);
                            populateFieldsAction();
                        });

                        // voxel size
                        float prevValue = settingsPreset.VoxelSize;
                        InputField voxelSizeFiled = display.GetObject<InputField>(6);
                        voxelSizeFiled.text = settingsPreset.VoxelSize.ToString(CultureInfo.InvariantCulture);
                        voxelSizeFiled.onEndEdit.AddListener(delegate (string value)
                        {
                            if (!allowCallback)
                                return;

                            float newValue = ModParseUtils.TryParseToFloat(value, prevValue);
                            if (newValue <= 0f)
                            {
                                newValue = 0.01f;

                                allowCallback = false;
                                voxelSizeFiled.text = newValue.ToString(CultureInfo.InvariantCulture);
                                allowCallback = true;
                            }

                            settingsPreset.VoxelSize = newValue;
                            prevValue = newValue;
                            GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                        });
                    }
                }

                if (volume.GetUnusedShowCondition() != WeaponVariant.None)
                {
                    Button newPresetButton = Instantiate(propertiesPanel.m_addVolumeSettingsPresetButton, container);
                    newPresetButton.gameObject.SetActive(true);
                    newPresetButton.onClick.AddListener(delegate
                    {
                        volume.volumeSettingPresets.Add(volume.GetUnusedShowCondition(), new VolumeSettingsPreset()
                        {
                            CenterPivot = true,
                            VoxelSize = 0.1f,
                            ReplaceWithFavoriteColors = new Dictionary<string, FavoriteColorSettings>()
                        });
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
