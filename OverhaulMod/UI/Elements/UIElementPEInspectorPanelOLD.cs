using OverhaulMod.Content.Personalization;
using OverhaulMod.Content.Personalization.Objects;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPEInspectorPanelOLD : OverhaulUIBehaviour
    {
        public static readonly List<Dropdown.OptionData> s_weapons = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("Sword"),
            new Dropdown.OptionData("Bow"),
            new Dropdown.OptionData("Hammer"),
            new Dropdown.OptionData("Spear"),
        };

        public static readonly List<Dropdown.OptionData> s_variants = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("Normal"),
            new Dropdown.OptionData("Fire"),
            new Dropdown.OptionData("Normal (Multiplayer)"),
            new Dropdown.OptionData("Fire (Multiplayer)"),
        };

        [UIElement("VolumeColorsConfigPanel", typeof(UIElementPEVolumeColorsSettings), false)]
        private readonly UIElementPEVolumeColorsSettings _volumeColorsSettings;

        [UIElementAction(nameof(OnPositionChanged))]
        [UIElement("PositionPanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field _positionField;

        [UIElementAction(nameof(OnRotationChanged))]
        [UIElement("RotationPanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field _rotationField;

        [UIElementAction(nameof(OnScaleChanged))]
        [UIElement("ScalePanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field _scaleField;

        [UIElement("ForceVolumeSettingsPresetDropdown", false)]
        private readonly ModdedObject _forceVolumeSettingsPresetDropdown;

        [UIElement("VolumeSettingsPresetDisplay", false)]
        private readonly ModdedObject _volumeSettingsPresetDisplay;

        [UIElement("VolumeSettingsPresetDisplay_Acc", false)]
        private readonly ModdedObject _volumeSettingsPresetDisplayAccessory;

        [UIElement("EnableIfPresetDropdown", false)]
        private readonly ModdedObject _enableIfPresetDropdown;

        [UIElement("VolumeExtraSettings", false)]
        private readonly ModdedObject _volumeExtraSettings;

        [UIElement("ColorPickButton", false)]
        private readonly ModdedObject _colorPickButton;

        [UIElement("BoolFieldDisplay", false)]
        private readonly ModdedObject _togglePrefab;

        [UIElement("IntFloatSlider", false)]
        private readonly ModdedObject _intFloatSliderPrefab;

        [UIElement("CvmModelPresetDisplay", false)]
        private readonly ModdedObject _cvmModelPresetDisplay;

        [UIElement("AddVolumeSettingsPresetButton", false)]
        private readonly Button _addVolumeSettingsPresetButton;

        [UIElement("Content")]
        private readonly Transform _container;

        [UIElement("NothingToEditOverlay", true)]
        private readonly GameObject _nothingToEditOverlay;

        [UIElementCallback(true)]
        [UIElementAction(nameof(OnObjectNameChanged))]
        [UIElement("ObjectNameField")]
        private readonly InputField _objectNameField;

        private VolumePropertiesController _volumePropertiesController;

        private VisibilityPropertiesController _visibilityPropertiesController;

        private FireParticlesPropertiesController _fireParticlesPropertiesController;

        private CvmModelPropertiesController _cvmModelPropertiesController;

        private PersonalizationEditorPlacedObject _inspectingObject;

        private int _inspectingObjectId;

        private bool _disableCallbacks;

        private bool _prevObjectState;

        protected override void OnInitialized()
        {
            _inspectingObjectId = -1;
            _volumePropertiesController = new VolumePropertiesController();
            _visibilityPropertiesController = new VisibilityPropertiesController();
            _fireParticlesPropertiesController = new FireParticlesPropertiesController();
            _cvmModelPropertiesController = new CvmModelPropertiesController();
        }

        private void LateUpdate()
        {
            bool newObjectState = _inspectingObject;
            if (newObjectState != _prevObjectState)
            {
                if (!newObjectState)
                {
                    EditObject(null);
                }
                _prevObjectState = newObjectState;
            }
        }

        public void Clear()
        {
            TransformUtils.DestroyAllChildren(_container);
        }

        public void Refresh()
        {
            EditObject(_inspectingObject);
        }

        public int GetEditingObjectUniqueIndex()
        {
            return _inspectingObjectId;
        }

        public void EditObjectAgain()
        {
            if (_inspectingObjectId == -1) return;

            EditObject(PersonalizationEditorObjectManager.Instance.GetInstantiatedObject(_inspectingObjectId));
        }

        public void EditObject(PersonalizationEditorPlacedObject objectBehaviour)
        {
            bool isNotNull = objectBehaviour;

            ModUIs.HideGenericColorPicker();
            _volumeColorsSettings.Hide();
            _nothingToEditOverlay.SetActive(!isNotNull);
            if (!isNotNull)
            {
                _inspectingObjectId = -1;
                _inspectingObject = null;
                _objectNameField.text = string.Empty;
                GlobalEventManager.Instance.Dispatch(PersonalizationEditorObjectManager.OBJECT_SELECTION_CHANGED_EVENT);
                return;
            }
            _inspectingObjectId = objectBehaviour.UniqueIndex;
            _inspectingObject = objectBehaviour;
            GlobalEventManager.Instance.Dispatch(PersonalizationEditorObjectManager.OBJECT_SELECTION_CHANGED_EVENT);

            Clear();

            _disableCallbacks = true;
            _objectNameField.text = objectBehaviour.Name;
            _positionField.Vector = objectBehaviour.transform.localPosition;
            _rotationField.Vector = objectBehaviour.transform.localEulerAngles;
            _scaleField.Vector = objectBehaviour.transform.localScale;

            bool isWeaponSkin = objectBehaviour.SpawnInfo.ItemInfo.Category == PersonalizationCategory.WeaponSkins;

            if (isWeaponSkin && objectBehaviour.GetComponent<PersonalizationEditorVisibilityToggler>())
            {
                _visibilityPropertiesController.PopulateFields(this, _container, objectBehaviour);
            }

            if (objectBehaviour.GetComponent<PersonalizationEditorVoxModel>())
            {
                _volumePropertiesController.PopulateFields(this, _container, objectBehaviour);
            }

            if (objectBehaviour.GetComponent<PersonalizationEditorCVMModel>())
            {
                _cvmModelPropertiesController.PopulateFields(this, _container, objectBehaviour);
            }

            if (objectBehaviour.GetComponent<PersonalizationEditorFireParticles>())
            {
                _fireParticlesPropertiesController.PopulateFields(this, _container, objectBehaviour);
            }

            _disableCallbacks = false;
        }

        public void OnPositionChanged(Vector3 value)
        {
            if (_disableCallbacks)
                return;

            PersonalizationEditorPlacedObject objectBehaviour = _inspectingObject;
            if (!objectBehaviour)
                return;

            objectBehaviour.transform.localPosition = value;
            objectBehaviour.SerializedPosition = value;
        }

        public void OnRotationChanged(Vector3 value)
        {
            if (_disableCallbacks)
                return;

            PersonalizationEditorPlacedObject objectBehaviour = _inspectingObject;
            if (!objectBehaviour)
                return;

            objectBehaviour.transform.localEulerAngles = value;
            objectBehaviour.SerializedEulerAngles = value;
        }

        public void OnScaleChanged(Vector3 value)
        {
            if (_disableCallbacks)
                return;

            PersonalizationEditorPlacedObject objectBehaviour = _inspectingObject;
            if (!objectBehaviour)
                return;

            objectBehaviour.transform.localScale = value;
            objectBehaviour.SerializedScale = value;
        }

        public void OnObjectNameChanged(string str)
        {
            if (!_disableCallbacks && _inspectingObject)
            {
                _inspectingObject.Name = str;
                PersonalizationEditorManager.Instance.SerializeRoot();
                //UIPersonalizationEditor.Instance.ItemConfig.RefreshHierarchyPanel();
            }
        }

        public class ObjectPropertiesController
        {
            public virtual void PopulateFields(UIElementPEInspectorPanelOLD propertiesPanel, Transform container, PersonalizationEditorPlacedObject objectBehaviour)
            {

            }
        }

        public class CvmModelPropertiesController : ObjectPropertiesController
        {
            public override void PopulateFields(UIElementPEInspectorPanelOLD propertiesPanel, Transform container, PersonalizationEditorPlacedObject objectBehaviour)
            {
                void populateFieldsAction()
                {
                    propertiesPanel.Refresh();
                }

                PersonalizationEditorCVMModel model = objectBehaviour.GetComponent<PersonalizationEditorCVMModel>();

                ModdedObject volumeExtraSettings = Instantiate(propertiesPanel._volumeExtraSettings, container);
                volumeExtraSettings.gameObject.SetActive(true);
                Toggle hideIfNoPresetToggle = volumeExtraSettings.GetObject<Toggle>(0);
                hideIfNoPresetToggle.isOn = model.HideIfNoPreset;
                hideIfNoPresetToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    model.HideIfNoPreset = value;
                    GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                });

                Dictionary<WeaponVariant2, CVMModelPreset> presets = model.Presets;
                if (presets != null && presets.Count != 0)
                {
                    foreach (KeyValuePair<WeaponVariant2, CVMModelPreset> keyValue in presets)
                    {
                        CVMModelPreset preset = keyValue.Value;

                        ModdedObject display = Instantiate(propertiesPanel._cvmModelPresetDisplay, container);
                        display.gameObject.SetActive(true);

                        // voxel model file
                        ModdedObject voxelModelFileField = display.GetObject<ModdedObject>(1);
                        InputField voxelModelFileFieldText = voxelModelFileField.GetObject<InputField>(0);
                        voxelModelFileFieldText.text = Path.GetFileName(preset.CvmFilePath);
                        voxelModelFileField.GetObject<Button>(1).onClick.AddListener(delegate
                        {
                            ModUIUtils.FileExplorer(UIPE.Instance.transform, true, delegate (string filePath)
                            {
                                if (filePath.IsNullOrEmpty())
                                {
                                    voxelModelFileFieldText.text = string.Empty;
                                    preset.CvmFilePath = string.Empty;
                                }
                                else
                                {
                                    string directoryName = ModFileUtils.GetDirectoryName(PersonalizationEditorManager.Instance.EditingItemInfo.FolderPath);
                                    string fileName = Path.GetFileName(filePath);
                                    string path = Path.Combine(directoryName, "files", fileName);

                                    if (preset.CvmFilePath == path)
                                        return;

                                    voxelModelFileFieldText.text = fileName;
                                    preset.CvmFilePath = path;

                                    UIPE.Instance.Utilities.SetPreviewingPreset(keyValue.Key);
                                }

                                GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                            }, PersonalizationItemInfo.GetImportedFilesFolder(PersonalizationEditorManager.Instance.EditingItemInfo), "*.cvm");
                        });

                        // conditions dropdown
                        bool allowCallback = true;
                        WeaponVariant2 prevCondition = keyValue.Key;
                        Dropdown conditionsDropdown = display.GetObject<Dropdown>(0);
                        conditionsDropdown.options = PersonalizationEditorManager.Instance.GetPresetsForEditingWeaponSkin();

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

                            WeaponVariant2 condition = (conditionsDropdown.options[value] as DropdownWeaponVariantOptionData).Value;
                            if (presets.ContainsKey(condition))
                            {
                                allowCallback = false;
                                conditionsDropdown.value = ((int)prevCondition) - 1;
                                allowCallback = true;

                                ModUIUtils.MessagePopupOK("Cannot change preset usage condition", "Another preset is already using this condition");
                            }
                            else
                            {
                                _ = presets.Remove(prevCondition);
                                presets.Add(condition, preset);

                                prevCondition = condition;
                                GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                            }
                        });
                        conditionsDropdown.interactable = model.GetUnusedWeaponVariant() != WeaponVariant2.None;

                        // active frame
                        void refreshActiveFrameAction()
                        {
                            display.GetObject<GameObject>(2).SetActive(prevCondition == PersonalizationEditorManager.Instance.PreviewPresetKey);
                        }
                        refreshActiveFrameAction();

                        EventController singleEventController = display.gameObject.AddComponent<EventController>();
                        singleEventController.AddEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, refreshActiveFrameAction);
                        singleEventController.AddEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, refreshActiveFrameAction);

                        // delete
                        display.GetObject<Button>(3).onClick.AddListener(delegate
                        {
                            _ = presets.Remove(prevCondition);
                            populateFieldsAction();
                        });

                        // weapon dropdown
                        Dropdown weaponDropdown = display.GetObject<Dropdown>(4);
                        weaponDropdown.options = s_weapons;
                        switch (preset.Weapon)
                        {
                            case WeaponType.Sword:
                                weaponDropdown.value = 0;
                                break;
                            case WeaponType.Bow:
                                weaponDropdown.value = 1;
                                break;
                            case WeaponType.Hammer:
                                weaponDropdown.value = 2;
                                break;
                            case WeaponType.Spear:
                                weaponDropdown.value = 3;
                                break;
                        }
                        weaponDropdown.onValueChanged.AddListener(delegate (int value)
                        {
                            switch (value)
                            {
                                case 0:
                                    preset.Weapon = WeaponType.Sword;
                                    break;
                                case 1:
                                    preset.Weapon = WeaponType.Bow;
                                    break;
                                case 2:
                                    preset.Weapon = WeaponType.Hammer;
                                    break;
                                case 3:
                                    preset.Weapon = WeaponType.Spear;
                                    break;
                            }
                            GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                        });

                        // variant dropdown
                        Dropdown variantDropdown = display.GetObject<Dropdown>(5);
                        variantDropdown.options = s_variants;
                        switch (preset.Variant)
                        {
                            case WeaponVariant2.Normal:
                                variantDropdown.value = 0;
                                break;
                            case WeaponVariant2.OnFire:
                                variantDropdown.value = 1;
                                break;
                            case WeaponVariant2.NormalMultiplayer:
                                variantDropdown.value = 2;
                                break;
                            case WeaponVariant2.OnFireMultiplayer:
                                variantDropdown.value = 3;
                                break;
                        }
                        variantDropdown.onValueChanged.AddListener(delegate (int value)
                        {
                            switch (value)
                            {
                                case 0:
                                    preset.Variant = WeaponVariant2.Normal;
                                    break;
                                case 1:
                                    preset.Variant = WeaponVariant2.OnFire;
                                    break;
                                case 2:
                                    preset.Variant = WeaponVariant2.NormalMultiplayer;
                                    break;
                                case 3:
                                    preset.Variant = WeaponVariant2.OnFireMultiplayer;
                                    break;
                            }
                            GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                        });

                        // replace colors toggle
                        Toggle replaceColorsToggle = display.GetObject<Toggle>(6);
                        replaceColorsToggle.isOn = preset.ReplaceColors;
                        replaceColorsToggle.onValueChanged.AddListener(delegate (bool value)
                        {
                            preset.ReplaceColors = value;
                            GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                        });

                        // fire particles toggle
                        Toggle fireParticlesToggle = display.GetObject<Toggle>(7);
                        fireParticlesToggle.isOn = preset.ShowFireParticles;
                        fireParticlesToggle.onValueChanged.AddListener(delegate (bool value)
                        {
                            preset.ShowFireParticles = value;
                            GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                        });
                    }
                }

                if (model.GetUnusedWeaponVariant() != WeaponVariant2.None)
                {
                    Button newPresetButton = Instantiate(propertiesPanel._addVolumeSettingsPresetButton, container);
                    newPresetButton.gameObject.SetActive(true);
                    newPresetButton.onClick.AddListener(delegate
                    {
                        model.Presets.Add(model.GetUnusedWeaponVariant(), new CVMModelPreset(true));
                        populateFieldsAction();
                    });
                }
            }
        }

        public class FireParticlesPropertiesController : ObjectPropertiesController
        {
            public override void PopulateFields(UIElementPEInspectorPanelOLD propertiesPanel, Transform container, PersonalizationEditorPlacedObject objectBehaviour)
            {
                PersonalizationEditorFireParticles fireParticles = objectBehaviour.GetComponent<PersonalizationEditorFireParticles>();

                ModdedObject colorPickButton = Instantiate(propertiesPanel._colorPickButton, container);
                colorPickButton.gameObject.SetActive(true);
                colorPickButton.GetObject<Text>(2).text = "Fire color";
                UIElementColorPickerButton colorPickerButtonComponent = colorPickButton.gameObject.AddComponent<UIElementColorPickerButton>();
                colorPickerButtonComponent.InitializeAsElement();
                colorPickerButtonComponent.ColorPickerParent = UIPE.Instance.transform;
                colorPickerButtonComponent.useAlpha = true;
                colorPickerButtonComponent.color = fireParticles.Color;
                colorPickerButtonComponent.onValueChanged.AddListener(delegate (Color color)
                {
                    fireParticles.Color = color;
                    fireParticles.RefreshColor();
                });

                ModdedObject applyFavoriteColorToggleModdedObject = Instantiate(propertiesPanel._togglePrefab, container);
                applyFavoriteColorToggleModdedObject.gameObject.SetActive(true);
                Toggle applyFavoriteColorToggle = applyFavoriteColorToggleModdedObject.GetComponent<Toggle>();
                applyFavoriteColorToggle.isOn = fireParticles.ApplyFavoriteColor;
                applyFavoriteColorToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    fireParticles.ApplyFavoriteColor = value;
                    fireParticles.RefreshColor();
                });
                applyFavoriteColorToggleModdedObject.GetObject<Text>(0).text = "Apply favorite color";

                ModdedObject hueOffsetSliderModdedObject = Instantiate(propertiesPanel._intFloatSliderPrefab, container);
                hueOffsetSliderModdedObject.gameObject.SetActive(true);
                hueOffsetSliderModdedObject.GetObject<Text>(1).text = "Fav. color hue offset";
                Slider hueOffsetSlider = hueOffsetSliderModdedObject.GetObject<Slider>(0);
                hueOffsetSlider.minValue = -0.1f;
                hueOffsetSlider.maxValue = 0.1f;
                hueOffsetSlider.value = fireParticles.FavoriteColorHueOffset;
                hueOffsetSlider.onValueChanged.AddListener(delegate (float value)
                {
                    float ho = Mathf.Round(value * 100f) / 100f;
                    ModUIUtils.Tooltip($"{ho}");
                    fireParticles.FavoriteColorHueOffset = ho;
                    fireParticles.RefreshColor();
                });

                ModdedObject brightnessSliderModdedObject = Instantiate(propertiesPanel._intFloatSliderPrefab, container);
                brightnessSliderModdedObject.gameObject.SetActive(true);
                brightnessSliderModdedObject.GetObject<Text>(1).text = "Fav. color brightness";
                Slider brightnessSlider = brightnessSliderModdedObject.GetObject<Slider>(0);
                brightnessSlider.minValue = 0f;
                brightnessSlider.maxValue = 100f;
                brightnessSlider.value = Mathf.Round(fireParticles.FavoriteColorBrightness * 100f);
                brightnessSlider.onValueChanged.AddListener(delegate (float value)
                {
                    ModUIUtils.Tooltip($"{value}%");
                    fireParticles.FavoriteColorBrightness = value / 100f;
                    fireParticles.RefreshColor();
                });

                ModdedObject saturationSliderModdedObject = Instantiate(propertiesPanel._intFloatSliderPrefab, container);
                saturationSliderModdedObject.gameObject.SetActive(true);
                saturationSliderModdedObject.GetObject<Text>(1).text = "Fav. color saturation";
                Slider saturationSlider = saturationSliderModdedObject.GetObject<Slider>(0);
                saturationSlider.minValue = 0f;
                saturationSlider.maxValue = 100f;
                saturationSlider.value = Mathf.Round(fireParticles.FavoriteColorSaturation * 100f);
                saturationSlider.onValueChanged.AddListener(delegate (float value)
                {
                    ModUIUtils.Tooltip($"{value}%");
                    fireParticles.FavoriteColorSaturation = value / 100f;
                    fireParticles.RefreshColor();
                });

                ModdedObject enableSmokeToggleModdedObject = Instantiate(propertiesPanel._togglePrefab, container);
                enableSmokeToggleModdedObject.gameObject.SetActive(true);
                Toggle enableSmokeToggle = enableSmokeToggleModdedObject.GetComponent<Toggle>();
                enableSmokeToggle.isOn = fireParticles.EnableSmoke;
                enableSmokeToggle.onValueChanged.AddListener(delegate (bool value)
                {
                    fireParticles.EnableSmoke = value;
                    fireParticles.RefreshColor();
                });
                enableSmokeToggleModdedObject.GetObject<Text>(0).text = "Enable smoke";
            }
        }

        public class VisibilityPropertiesController : ObjectPropertiesController
        {
            public override void PopulateFields(UIElementPEInspectorPanelOLD propertiesPanel, Transform container, PersonalizationEditorPlacedObject objectBehaviour)
            {
                ModdedObject enableIfPresetDropdown = Instantiate(propertiesPanel._enableIfPresetDropdown, container);
                enableIfPresetDropdown.gameObject.SetActive(true);

                PersonalizationEditorVisibilityToggler visibilityController = objectBehaviour.GetComponent<PersonalizationEditorVisibilityToggler>();

                Dropdown dropdown = enableIfPresetDropdown.GetObject<Dropdown>(0);
                dropdown.options = PersonalizationEditorManager.Instance.GetPresetsForEditingWeaponSkin(true);

                int conditionDropdownValueToSet = -1;
                for (int i = 0; i < dropdown.options.Count; i++)
                {
                    if (dropdown.options[i] is DropdownWeaponVariantOptionData showConditionOptionData && showConditionOptionData.Value == visibilityController.EnableIfWeaponVariant)
                    {
                        conditionDropdownValueToSet = i;
                    }
                }

                if (conditionDropdownValueToSet == -1)
                {
                    dropdown.options.Add(new DropdownWeaponVariantOptionData(visibilityController.EnableIfWeaponVariant));
                    dropdown.RefreshShownValue();
                    conditionDropdownValueToSet = dropdown.options.Count - 1;
                }

                dropdown.value = conditionDropdownValueToSet;
                dropdown.onValueChanged.AddListener(delegate (int value)
                {
                    WeaponVariant2 weaponVariant = (dropdown.options[value] as DropdownWeaponVariantOptionData).Value;
                    visibilityController.EnableIfWeaponVariant = weaponVariant;
                    visibilityController.RefreshVisibility();
                });
            }
        }

        public class VolumePropertiesController : ObjectPropertiesController
        {
            public override void PopulateFields(UIElementPEInspectorPanelOLD propertiesPanel, Transform container, PersonalizationEditorPlacedObject objectBehaviour)
            {
                void populateFieldsAction()
                {
                    propertiesPanel.Refresh();
                }

                bool isWeaponSkin = objectBehaviour.SpawnInfo.ItemInfo.Category == PersonalizationCategory.WeaponSkins;

                PersonalizationEditorVoxModel volume = objectBehaviour.GetComponent<PersonalizationEditorVoxModel>();

                if (isWeaponSkin)
                {
                    ModdedObject volumeExtraSettings = Instantiate(propertiesPanel._volumeExtraSettings, container);
                    volumeExtraSettings.gameObject.SetActive(true);
                    Toggle hideIfNoPresetToggle = volumeExtraSettings.GetObject<Toggle>(0);
                    hideIfNoPresetToggle.isOn = volume.HideIfNoPreset;
                    hideIfNoPresetToggle.onValueChanged.AddListener(delegate (bool value)
                    {
                        volume.HideIfNoPreset = value;
                        GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                    });
                }

                Dictionary<WeaponVariant2, VolumeSettingsPreset> volumePresets = volume.VolumeSettingPresets;
                if (volumePresets != null && volumePresets.Count != 0)
                {
                    foreach (KeyValuePair<WeaponVariant2, VolumeSettingsPreset> preset in volumePresets)
                    {
                        VolumeSettingsPreset settingsPreset = preset.Value;

                        ModdedObject display = Instantiate((isWeaponSkin || volumePresets.Count != 1) ? propertiesPanel._volumeSettingsPresetDisplay : propertiesPanel._volumeSettingsPresetDisplayAccessory, container);
                        display.gameObject.SetActive(true);

                        // voxel model file
                        ModdedObject voxelModelFileField = display.GetObject<ModdedObject>(1);
                        InputField voxelModelFileFieldText = voxelModelFileField.GetObject<InputField>(0);
                        voxelModelFileFieldText.text = Path.GetFileName(settingsPreset.VoxFilePath);
                        voxelModelFileField.GetObject<Button>(1).onClick.AddListener(delegate
                        {
                            ModUIUtils.FileExplorer(UIPE.Instance.transform, true, delegate (string filePath)
                            {
                                if (filePath.IsNullOrEmpty())
                                {
                                    voxelModelFileFieldText.text = string.Empty;
                                    settingsPreset.VoxFilePath = string.Empty;
                                }
                                else
                                {
                                    string directoryName = ModFileUtils.GetDirectoryName(PersonalizationEditorManager.Instance.EditingItemInfo.FolderPath);
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

                                    voxelModelFileFieldText.text = fileName;
                                    settingsPreset.VoxFilePath = path;

                                    UIPE.Instance.Utilities.SetPreviewingPreset(preset.Key);
                                }

                                GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                            }, PersonalizationItemInfo.GetImportedFilesFolder(PersonalizationEditorManager.Instance.EditingItemInfo), "*.vox");
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
                        WeaponVariant2 prevCondition = preset.Key;
                        Dropdown conditionsDropdown = display.GetObject<Dropdown>(0);
                        conditionsDropdown.options = PersonalizationEditorManager.Instance.GetPresetsForEditingWeaponSkin();

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

                            WeaponVariant2 condition = (conditionsDropdown.options[value] as DropdownWeaponVariantOptionData).Value;
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
                        conditionsDropdown.interactable = volume.GetUnusedWeaponVariant() != WeaponVariant2.None;

                        // active frame
                        void refreshActiveFrameAction()
                        {
                            display.GetObject<GameObject>(4).SetActive(prevCondition == PersonalizationEditorManager.Instance.PreviewPresetKey);
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
                            UIElementPEVolumeColorsSettings volumeColorsSettings = propertiesPanel._volumeColorsSettings;
                            volumeColorsSettings.Show();
                            volumeColorsSettings.Populate(settingsPreset);
                            volumeColorsSettings.OnColorChanged = onColorChangedAction;
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

                            float newValue = ModParseUtils.TryParseFloat(value, prevValue);
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

                if ((isWeaponSkin || volumePresets.Count == 0) && volume.GetUnusedWeaponVariant() != WeaponVariant2.None)
                {
                    Button newPresetButton = Instantiate(propertiesPanel._addVolumeSettingsPresetButton, container);
                    newPresetButton.gameObject.SetActive(true);
                    newPresetButton.onClick.AddListener(delegate
                    {
                        volume.VolumeSettingPresets.Add(volume.GetUnusedWeaponVariant(), new VolumeSettingsPreset()
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
    }
}
