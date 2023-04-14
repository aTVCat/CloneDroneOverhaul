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

        private Transform m_ToggleTickBox;
        private Transform m_ToggleBGOff;
        private Transform m_ToggleBGOn;
        private Transform m_ToggleTick;

        private Slider m_Slider;
        private Dropdown m_Dropdown;
        private InputField m_InputField;

        private Button m_IDButton;
        private Button m_DefValueButton;

        private Transform m_LockedBG;

        private static readonly List<ParametersMenuSetting> _spawnedBehaviours = new List<ParametersMenuSetting>();

        private ModdedObject m_ModdedObject;
        private OverhaulParametersMenu m_UI;

        public SettingInfo Setting;
        public SettingDescription Description;
        private ParametersMenuSettingPosition m_MyPos;

        private bool m_HasChangedSettingValueBefore;

        public void Initialize(in OverhaulParametersMenu menu, in ModdedObject moddedObject, in string settingPath, in ParametersMenuSettingPosition position, bool notFirstInit = false)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (m_ModdedObject == null) m_ModdedObject = moddedObject;
            if (m_UI == null) m_UI = menu;

            if (!notFirstInit)
            {
                m_IDButton = m_ModdedObject.GetObject<Button>(6);
                m_IDButton.onClick.AddListener(copyID);
                m_IDButton.gameObject.SetActive(OverhaulVersion.IsDebugBuild);
                m_DefValueButton = m_ModdedObject.GetObject<Button>(12);
                m_DefValueButton.onClick.AddListener(setDefValue);
                m_LockedBG = m_ModdedObject.GetObject<Transform>(14);
            }

            if (Setting == null) Setting = SettingsController.GetSetting(settingPath);
            if (Description == null) Description = SettingsController.GetSettingDescription(settingPath);
            if (Setting == null || Setting.Error)
            {
                base.GetComponent<Image>().color = BGColor_Error.ConvertHexToColor();
                moddedObject.GetObject<Text>(0).text = settingPath;
                return;
            }

            if (!notFirstInit)
            {
                string[] array = settingPath.Split('.');

                m_ModdedObject.GetObject<Text>(0).text = OverhaulLocalizationController.GetTranslation(OverhaulParametersMenu.SettingTranslationPrefix + array[2]);
                m_ModdedObject.GetObject<Text>(1).text = string.Empty;
                if (Description != null)
                {
                    m_ModdedObject.GetObject<Text>(1).text = OverhaulLocalizationController.GetTranslation(OverhaulParametersMenu.SettingDescTranslationPrefix + array[2]);
                }

                base.GetComponent<Image>().enabled = position == ParametersMenuSettingPosition.Normal;
                m_ModdedObject.GetObject<Transform>(7).gameObject.SetActive(position == ParametersMenuSettingPosition.Top);
                m_ModdedObject.GetObject<Image>(7).color = BGColor_Normal.ConvertHexToColor();
                m_ModdedObject.GetObject<Transform>(8).gameObject.SetActive(position == ParametersMenuSettingPosition.Center);
                m_ModdedObject.GetObject<Image>(8).color = BGColor_Normal.ConvertHexToColor();
                m_ModdedObject.GetObject<Transform>(9).gameObject.SetActive(position == ParametersMenuSettingPosition.Bottom);
                m_ModdedObject.GetObject<Image>(9).color = BGColor_Normal.ConvertHexToColor();
                m_MyPos = position;
            }

            configToggle(moddedObject, Setting.Type == SettingType.Bool, position);
            configSlider(moddedObject, Setting.SliderParameters);
            configDropdown(moddedObject, Setting.DropdownParameters);
            configInputField(moddedObject);

            if (!notFirstInit)
            {
                _ = OverhaulEventsController.AddEventListener(SettingsController.SettingChangedEventString, onSettingRefreshed);
            }
            onSettingRefreshed();
        }

        protected override void OnDisposed()
        {
            OverhaulEventsController.RemoveEventListener(SettingsController.SettingChangedEventString, onSettingRefreshed);
            _ = _spawnedBehaviours.Remove(this);
            m_ToggleBGOff = null;
            m_ToggleBGOn = null;
            m_ToggleTickBox = null;
            m_ToggleTick = null;
            m_Slider = null;
            m_Dropdown = null;
            m_InputField = null;
            m_DefValueButton = null;
            m_IDButton = null;
            m_ModdedObject = null;
            m_LockedBG = null;
            m_UI = null;
            Setting = null;
            Description = null;
        }

        private void onSettingRefreshed()
        {
            if (IsDisposedOrDestroyed() || m_LockedBG == null)
            {
                return;
            }
            m_LockedBG.gameObject.SetActive(Setting == null || !Setting.IsUnlocked());
        }

        private void configInputField(in ModdedObject m)
        {
            m_InputField = m.GetObject<InputField>(13);
            m_InputField.gameObject.SetActive(Setting.Type == SettingType.String || Setting.ForceInputField);
            if (!m_InputField.gameObject.activeSelf)
            {
                return;
            }

            m_InputField.text = SettingInfo.GetPref<object>(Setting).ToString();
            m_InputField.onEndEdit.AddListener(setInputFieldValue);
        }

        private void configDropdown(in ModdedObject m, in SettingDropdownParameters parameters)
        {
            m_Dropdown = m.GetObject<Dropdown>(11);
            m_Dropdown.gameObject.SetActive(parameters != null && !Setting.ForceInputField);
            if (!m_Dropdown.gameObject.activeSelf)
            {
                return;
            }

            m_Dropdown.options = parameters.Options;
            m_Dropdown.value = SettingInfo.GetPref<int>(Setting);
            m_Dropdown.onValueChanged.AddListener(setDropdownValue);
        }

        private void configSlider(in ModdedObject m, in SettingSliderParameters parameters)
        {
            m_Slider = m.GetObject<Slider>(10);
            m_Slider.gameObject.SetActive(parameters != null && !Setting.ForceInputField);
            if (!m_Slider.gameObject.activeSelf)
            {
                return;
            }

            m_Slider.wholeNumbers = parameters.IsInt;
            m_Slider.minValue = parameters.Min;
            m_Slider.maxValue = parameters.Max;
            m_Slider.value = parameters.IsInt ? SettingInfo.GetPref<int>(Setting) : SettingInfo.GetPref<float>(Setting);
            m_Slider.onValueChanged.AddListener(setSliderValue);
        }

        private void configToggle(in ModdedObject m, in bool isBool, in ParametersMenuSettingPosition position)
        {
            m_ToggleTickBox = m.GetObject<Transform>(2);
            m_ToggleBGOff = m.GetObject<Transform>(3);
            m_ToggleBGOn = m.GetObject<Transform>(4);
            m_ToggleTick = m.GetObject<Transform>(5);
            m_ToggleTickBox.gameObject.SetActive(isBool && !Setting.ForceInputField);

            Toggle t = base.GetComponent<Toggle>();
            t.enabled = isBool && !Setting.ForceInputField;
            if (!isBool)
            {
                return;
            }

            switch (position)
            {
                case ParametersMenuSettingPosition.Normal:
                    t.targetGraphic = base.GetComponent<Image>();
                    break;
                case ParametersMenuSettingPosition.Top:
                    t.targetGraphic = m_ModdedObject.GetObject<Image>(7);
                    break;
                case ParametersMenuSettingPosition.Center:
                    t.targetGraphic = m_ModdedObject.GetObject<Image>(8);
                    break;
                case ParametersMenuSettingPosition.Bottom:
                    t.targetGraphic = m_ModdedObject.GetObject<Image>(9);
                    break;
            }

            t.isOn = SettingInfo.GetPref<bool>(Setting);
            setToggleValue(t.isOn);
            t.onValueChanged.AddListener(setToggleValue);
        }

        private void setInputFieldValue(string value)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            object toSet;
            switch (Setting.Type)
            {
                case SettingType.Bool:
                    bool success1 = bool.TryParse(value, out bool result);
                    toSet = success1 ? result : Setting.DefaultValue;
                    break;
                case SettingType.Float:
                    bool success2 = float.TryParse(value, out float result2);
                    toSet = success2 ? result2 : Setting.DefaultValue;
                    break;
                case SettingType.Int:
                    bool success3 = int.TryParse(value, out int result3);
                    toSet = success3 ? result3 : Setting.DefaultValue;
                    break;
                default:
                    toSet = value;
                    break;
            }
            SettingInfo.SavePref(Setting, toSet);
            if (m_HasChangedSettingValueBefore) tryInformPlayer();
            m_HasChangedSettingValueBefore = true;
        }

        private void setToggleValue(bool value)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            m_ToggleBGOff.gameObject.SetActive(!value);
            m_ToggleBGOn.gameObject.SetActive(value);
            m_ToggleTick.gameObject.SetActive(value);
            SettingInfo.SavePref(Setting, value);
            if (m_HasChangedSettingValueBefore) tryInformPlayer();
            m_HasChangedSettingValueBefore = true;
        }

        private void setDropdownValue(int value)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            SettingInfo.SavePref(Setting, value);
            if (m_HasChangedSettingValueBefore) tryInformPlayer();
            m_HasChangedSettingValueBefore = true;
        }

        private void setSliderValue(float value)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            if (Setting.SliderParameters.IsInt)
            {
                SettingInfo.SavePref(Setting, Mathf.RoundToInt(value));
            }
            else
            {
                SettingInfo.SavePref(Setting, value);
            }
        }

        private void copyID()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            TextEditor editor = new TextEditor
            {
                text = Setting.RawPath
            };
            editor.SelectAll();
            editor.Copy();
        }

        private void setDefValue()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            SettingInfo.SavePref(Setting, Setting.DefaultValue);

            Initialize(m_UI, m_ModdedObject, Setting.RawPath, m_MyPos, true);
            if (m_HasChangedSettingValueBefore) tryInformPlayer();
            m_HasChangedSettingValueBefore = true;

            /*
            m_UI.PopulateCategory(Setting.Category);*/
        }

        private void tryInformPlayer()
        {
            if (Setting != null && Setting.SendMessageOfType != 0)
            {
                if (Setting.SendMessageOfType == 1)
                {
                    OverhaulDialogues.CreateDialogueFromPreset(OverhaulDialoguePresetType.RestartTheGame);
                }
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            m_UI.PopulateDescription(Setting, Description);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            m_UI.PopulateDescription(null, null);
        }
    }
}