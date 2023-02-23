using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class SettingEntryBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public const string BGColor_Normal = "#3A3B40";
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

        private static readonly List<SettingEntryBehaviour> _spawnedBehaviours = new List<SettingEntryBehaviour>();

        private ModdedObject m_ModdedObject;
        private UISettingsMenu m_UI;

        public SettingInfo Setting;
        public SettingDescription Description;

        public void Initialize(in UISettingsMenu menu, in ModdedObject moddedObject, in string settingPath, in ESettingPosition position)
        {
            m_ModdedObject = moddedObject;
            m_UI = menu;

            m_IDButton = m_ModdedObject.GetObject<Button>(6);
            m_IDButton.onClick.AddListener(copyID);
            m_DefValueButton = m_ModdedObject.GetObject<Button>(12);
            m_DefValueButton.onClick.AddListener(setDefValue);

            Setting = SettingsController.GetSetting(settingPath);
            Description = SettingsController.GetSettingDescription(settingPath);
            if (Setting == null || Setting.Error)
            {
                base.GetComponent<Image>().color = BGColor_Error.ConvertHexToColor();
                moddedObject.GetObject<Text>(0).text = settingPath;
                return;
            }
            string[] array = settingPath.Split('.');

            m_ModdedObject.GetObject<Text>(0).text = array[2];
            m_ModdedObject.GetObject<Text>(1).text = string.Empty;
            if (Description != null)
            {
                m_ModdedObject.GetObject<Text>(1).text = Description.Description;
            }

            base.GetComponent<Image>().enabled = position == ESettingPosition.Normal;
            m_ModdedObject.GetObject<Transform>(7).gameObject.SetActive(position == ESettingPosition.Top);
            m_ModdedObject.GetObject<Transform>(8).gameObject.SetActive(position == ESettingPosition.Center);
            m_ModdedObject.GetObject<Transform>(9).gameObject.SetActive(position == ESettingPosition.Bottom);

            configToggle(moddedObject, Setting.Type == ESettingType.Bool, position);
            configSlider(moddedObject, Setting.SliderParameters);
            configDropdown(moddedObject, Setting.DropdownParameters);
            configInputField(moddedObject);
        }

        private void configInputField(in ModdedObject m)
        {
            m_InputField = m.GetObject<InputField>(13);
            m_InputField.gameObject.SetActive(Setting.Type == ESettingType.String);
            if (!m_InputField.gameObject.activeSelf)
            {
                return;
            }

            m_InputField.text = SettingInfo.GetPref<string>(Setting);
            m_InputField.onEndEdit.AddListener(setInputFieldValue);
        }

        private void configDropdown(in ModdedObject m, in SettingDropdownParameters parameters)
        {
            m_Dropdown = m.GetObject<Dropdown>(11);
            m_Dropdown.gameObject.SetActive(parameters != null);
            if(!m_Dropdown.gameObject.activeSelf)
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
            m_Slider.gameObject.SetActive(parameters != null);
            if (!m_Slider.gameObject.activeSelf)
            {
                return;
            }

            m_Slider.wholeNumbers = parameters.IsInt;
            m_Slider.minValue = parameters.Min;
            m_Slider.maxValue = parameters.Max;
            if (parameters.IsInt)
            {
                m_Slider.value = SettingInfo.GetPref<int>(Setting);
            }
            else
            {
                m_Slider.value = SettingInfo.GetPref<float>(Setting);
            }
            m_Slider.onValueChanged.AddListener(setSliderValue);
        }

        private void configToggle(in ModdedObject m, in bool isBool, in ESettingPosition position)
        {
            m_ToggleTickBox = m.GetObject<Transform>(2);
            m_ToggleBGOff = m.GetObject<Transform>(3);
            m_ToggleBGOn = m.GetObject<Transform>(4);
            m_ToggleTick = m.GetObject<Transform>(5);
            m_ToggleTickBox.gameObject.SetActive(isBool);

            Toggle t = base.GetComponent<Toggle>();
            t.enabled = isBool;
            if (!isBool)
            {
                return;
            }

            switch (position)
            {
                case ESettingPosition.Normal:
                    t.targetGraphic = base.GetComponent<Image>();
                    break;
                case ESettingPosition.Top:
                    t.targetGraphic = m_ModdedObject.GetObject<Image>(7);
                    break;
                case ESettingPosition.Center:
                    t.targetGraphic = m_ModdedObject.GetObject<Image>(8);
                    break;
                case ESettingPosition.Bottom:
                    t.targetGraphic = m_ModdedObject.GetObject<Image>(9);
                    break;
            }

            t.isOn = SettingInfo.GetPref<bool>(Setting);
            setToggleValue(t.isOn);
            t.onValueChanged.AddListener(setToggleValue);
        }

        private void setInputFieldValue(string value)
        {
            SettingInfo.SavePref(Setting, value);
        }

        private void setToggleValue(bool value)
        {
            m_ToggleBGOff.gameObject.SetActive(!value);
            m_ToggleBGOn.gameObject.SetActive(value);
            m_ToggleTick.gameObject.SetActive(value);
            SettingInfo.SavePref(Setting, value);
        }

        private void setDropdownValue(int value)
        {
            SettingInfo.SavePref(Setting, value);
        }

        private void setSliderValue(float value)
        {
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
            TextEditor editor = new TextEditor
            {
                text = Setting.RawPath
            };
            editor.SelectAll();
            editor.Copy();
        }

        private void setDefValue()
        {
            SettingInfo.SavePref(Setting, Setting.DefaultValue);
            m_UI.PopulateCategory(Setting.Category);
        }

        private void OnDestroy()
        {
            _ = _spawnedBehaviours.Remove(this);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            m_UI.PopulateDescription(Setting, Description);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            m_UI.PopulateDescription(null, null);
        }
    }
}