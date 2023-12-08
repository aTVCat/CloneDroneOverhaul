using CDOverhaul.Gameplay.Combat;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class ParametersMenuSetting : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public const string BGColor_Normal = "#242527";
        public const string BGColor_Error = "#A63C43";

        private Image m_Image;
        private CanvasGroup m_CanvasGroup;

        private Transform m_ToggleTickBox;
        private Transform m_ToggleBGOff;
        private Transform m_ToggleBGOn;
        private Transform m_ToggleTick;

        private Toggle m_Toggle;
        private Slider m_Slider;
        private Dropdown m_Dropdown;
        private InputField m_InputField;

        private Button m_IDButton;
        private Button m_DefValueButton;

        private static readonly List<ParametersMenuSetting> _spawnedBehaviours = new List<ParametersMenuSetting>();

        private ModdedObject m_ModdedObject;
        private ParametersMenu m_UI;

        public SettingInfo Setting;
        public OverhaulSettingDescription Description;
        private ParametersMenuSettingPosition m_MyPos;

        private bool m_HasChangedSettingValueBefore;

        public void Initialize(in ParametersMenu menu, in ModdedObject moddedObject, in string settingPath, in ParametersMenuSettingPosition position, bool notFirstInit = false)
        {
            if (IsDisposedOrDestroyed())
                return;

            if (!notFirstInit)
            {
                m_UI = menu;
                m_ModdedObject = moddedObject;

                m_Image = base.GetComponent<Image>();
                m_CanvasGroup = m_ModdedObject.GetObject<CanvasGroup>(16);

                m_IDButton = m_ModdedObject.GetObject<Button>(6);
                m_IDButton.onClick.AddListener(copySettingId);
                m_IDButton.gameObject.SetActive(false/*OverhaulVersion.IsDebugBuild*/);

                m_DefValueButton = m_ModdedObject.GetObject<Button>(12);
                m_DefValueButton.onClick.AddListener(setSettingDefaultValue);

                m_ModdedObject.GetObject<Text>(17).text = OverhaulLocalizationController.GetTranslation("Reset");

                base.gameObject.AddComponent<OverhaulUISelectionOutline>().SetGraphic(m_ModdedObject.GetObject<Image>(15));
            }

            if (Setting == null) Setting = OverhaulSettingsController.GetSetting(settingPath);
            if (Setting == null || Setting.Error)
            {
                base.gameObject.SetActive(false);
                return;
            }
            if (Description == null) Description = OverhaulSettingsController.GetSettingDescription(settingPath);

            if (!notFirstInit)
            {
                string[] array = settingPath.Split('.');

                m_ModdedObject.GetObject<Text>(0).text = OverhaulLocalizationController.GetTranslation(ParametersMenu.SettingTranslationPrefix + array[2]);
                m_ModdedObject.GetObject<Text>(1).text = string.Empty;
                if (Description != null)
                {
                    m_ModdedObject.GetObject<Text>(1).text = OverhaulLocalizationController.GetTranslation(ParametersMenu.SettingDescTranslationPrefix + array[2]);
                }

                base.GetComponent<Image>().enabled = position == ParametersMenuSettingPosition.Normal;
                m_ModdedObject.GetObject<Transform>(7).gameObject.SetActive(position == ParametersMenuSettingPosition.Top);
                m_ModdedObject.GetObject<Image>(7).color = BGColor_Normal.ToColor();
                m_ModdedObject.GetObject<Transform>(8).gameObject.SetActive(position == ParametersMenuSettingPosition.Center);
                m_ModdedObject.GetObject<Image>(8).color = BGColor_Normal.ToColor();
                m_ModdedObject.GetObject<Transform>(9).gameObject.SetActive(position == ParametersMenuSettingPosition.Bottom);
                m_ModdedObject.GetObject<Image>(9).color = BGColor_Normal.ToColor();
                m_MyPos = position;
            }

            configToggle(moddedObject, Setting.Type == OverhaulSettingTypes.Bool, position);
            configSlider(moddedObject, Setting.SliderParameters);
            configDropdown(moddedObject, Setting.DropdownParameters);
            configInputField(moddedObject);

            if (!notFirstInit)
                OverhaulEvents.AddEventListener(OverhaulSettingsController.SettingChangedEventString, refresh);

            refresh();
        }

        protected override void OnDisposed()
        {
            OverhaulEvents.RemoveEventListener(OverhaulSettingsController.SettingChangedEventString, refresh);
            _ = _spawnedBehaviours.Remove(this);

            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public void SetIsInteractable(bool value)
        {
            m_CanvasGroup.alpha = value ? 1f : 0.5f;
            m_CanvasGroup.interactable = value;

            if (!m_Toggle || Setting == null || Setting.Error)
                return;

            m_Toggle.enabled = value && Setting.Type == OverhaulSettingTypes.Bool && !Setting.ForceInputField;
        }

        private void refresh()
        {
            if (IsDisposedOrDestroyed())
                return;

            SetIsInteractable(Setting != null && Setting.IsUnlocked());
        }

        private void configInputField(in ModdedObject m)
        {
            m_InputField = m.GetObject<InputField>(13);
            if (!m_InputField)
                return;

            bool active = Setting.Type == OverhaulSettingTypes.String || Setting.ForceInputField;

            m_InputField.gameObject.SetActive(active);
            if (!active)
                return;

            m_InputField.text = SettingInfo.GetPref<object>(Setting).ToString();
            m_InputField.onEndEdit.AddListener(setInputFieldValue);
        }

        private void configDropdown(in ModdedObject m, in OverhaulSettingDropdownParameters parameters)
        {
            m_Dropdown = m.GetObject<Dropdown>(11);
            if (!m_Dropdown)
                return;

            bool active = parameters != null && !Setting.ForceInputField;

            m_Dropdown.gameObject.SetActive(active);
            if (!active)
                return;

            m_Dropdown.options = parameters.Options;
            m_Dropdown.value = SettingInfo.GetPref<int>(Setting);
            m_Dropdown.onValueChanged.AddListener(setDropdownValue);
        }

        private void configSlider(in ModdedObject m, in OverhaulSettingSliderParameters parameters)
        {
            m_Slider = m.GetObject<Slider>(10);
            if (!m_Slider)
                return;

            bool active = parameters != null && !Setting.ForceInputField;

            m_Slider.gameObject.SetActive(active);
            if (!active)
                return;

            m_Slider.wholeNumbers = parameters.UseWholeNumbers;
            m_Slider.minValue = parameters.Min;
            m_Slider.maxValue = parameters.Max;
            m_Slider.value = parameters.UseWholeNumbers ? SettingInfo.GetPref<int>(Setting) : SettingInfo.GetPref<float>(Setting);
            m_Slider.onValueChanged.AddListener(setSliderValue);
        }

        private void configToggle(in ModdedObject m, in bool isBool, in ParametersMenuSettingPosition position)
        {
            m_Toggle = base.GetComponent<Toggle>();
            m_ToggleTickBox = m.GetObject<Transform>(2);
            m_ToggleBGOff = m.GetObject<Transform>(3);
            m_ToggleBGOn = m.GetObject<Transform>(4);
            m_ToggleTick = m.GetObject<Transform>(5);
            if (!m_Toggle || !m_ToggleTickBox || !m_ToggleBGOn || !m_ToggleBGOff || !m_ToggleTick)
                return;

            bool active = isBool && !Setting.ForceInputField;

            m_ToggleTickBox.gameObject.SetActive(active);
            m_Toggle.enabled = active;
            if (!isBool)
                return;

            setToggleValue(SettingInfo.GetPref<bool>(Setting), false);
            m_Toggle.onValueChanged.AddListener(setToggleValue);

            // This was just an experiment
            if (OverhaulFeatureAvailabilitySystem.ImplementedInBuild.ApplyThemeColorOnSettings)
                m_ToggleBGOn.GetComponent<Image>().color = OverhaulCombatState.GetUIThemeColor(ParametersMenu.DefaultBarColor);
        }

        private void setInputFieldValue(string value)
        {
            if (IsDisposedOrDestroyed())
                return;

            object toSet;
            switch (Setting.Type)
            {
                case OverhaulSettingTypes.Bool:
                    bool success1 = bool.TryParse(value, out bool result);
                    toSet = success1 ? result : Setting.DefaultValue;
                    break;
                case OverhaulSettingTypes.Float:
                    bool success2 = float.TryParse(value, out float result2);
                    toSet = success2 ? result2 : Setting.DefaultValue;
                    break;
                case OverhaulSettingTypes.Int:
                    bool success3 = int.TryParse(value, out int result3);
                    toSet = success3 ? result3 : Setting.DefaultValue;
                    break;
                default:
                    toSet = value;
                    break;
            }

            SettingInfo.SavePref(Setting, toSet);
            if (!m_HasChangedSettingValueBefore) informUser();
            m_HasChangedSettingValueBefore = true;
        }

        private void setToggleValue(bool value)
        {
            setToggleValue(value, true);
        }

        private void setToggleValue(bool value, bool updateSetting = true)
        {
            if (IsDisposedOrDestroyed())
                return;

            m_ToggleBGOff.gameObject.SetActive(!value);
            m_ToggleBGOn.gameObject.SetActive(value);
            m_ToggleTick.gameObject.SetActive(value);
            if (!updateSetting)
            {
                m_Toggle.isOn = value;
                return;
            }

            SettingInfo.SavePref(Setting, value);
            if (!m_HasChangedSettingValueBefore) informUser();
            m_HasChangedSettingValueBefore = true;
        }

        private void setDropdownValue(int value)
        {
            if (IsDisposedOrDestroyed())
                return;

            SettingInfo.SavePref(Setting, value);
            if (!m_HasChangedSettingValueBefore) informUser();
            m_HasChangedSettingValueBefore = true;
        }

        private void setSliderValue(float value)
        {
            if (IsDisposedOrDestroyed())
                return;

            if (Setting.SliderParameters.UseWholeNumbers)
                SettingInfo.SavePref(Setting, Mathf.RoundToInt(value));
            else
                SettingInfo.SavePref(Setting, value);
        }

        private void copySettingId()
        {
            if (IsDisposedOrDestroyed())
                return;

            Setting.RawPath.CopyToClipboard();
        }

        private void setSettingDefaultValue()
        {
            if (IsDisposedOrDestroyed())
                return;

            SettingInfo.SavePref(Setting, Setting.DefaultValue);

            Initialize(m_UI, m_ModdedObject, Setting.RawPath, m_MyPos, true);

            if (!m_HasChangedSettingValueBefore) informUser();
            m_HasChangedSettingValueBefore = true;
        }

        private void informUser()
        {
            if (Setting == null)
                return;

            switch (Setting.SendMessageOfType)
            {
                case 1:
                    OverhaulDialogues.CreateDialogueFromPreset(OverhaulDialoguePresetType.RestartTheGame);
                    break;
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
        }
    }
}