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

        public static readonly float NormalX = 45f;
        public static readonly float XWhenNotToggle = 105f;

        private Transform Toggle_TickBox;
        private Transform Toggle_BGOff;
        private Transform Toggle_BGOn;
        private Transform Toggle_Tick;

        private static readonly List<SettingEntryBehaviour> _spawnedBehaviours = new List<SettingEntryBehaviour>();

        private ModdedObject _moddedObject;
        private UISettingsMenu _ui;

        public SettingInfo Setting;
        public SettingDescription Description;

        public void Initialize(in UISettingsMenu menu, in ModdedObject moddedObject, in string settingPath, in ESettingPosition position)
        {
            _moddedObject = moddedObject;
            _ui = menu;

            Setting = SettingsController.GetSetting(settingPath);
            Description = SettingsController.GetSettingDescription(settingPath);
            if (Setting == null || Setting.Error)
            {
                base.GetComponent<Image>().color = BGColor_Error.ConvertHexToColor();
                moddedObject.GetObject<Text>(0).text = settingPath;
                return;
            }
            base.GetComponent<Image>().color = BGColor_Normal.ConvertHexToColor();

            string[] array = settingPath.Split('.');

            _moddedObject.GetObject<Text>(0).text = array[2];
            _moddedObject.GetObject<Text>(1).text = string.Empty;
            if (Description != null) _moddedObject.GetObject<Text>(1).text = Description.Description;

            base.GetComponent<Image>().enabled = position == ESettingPosition.Normal;
            _moddedObject.GetObject<Transform>(7).gameObject.SetActive(position == ESettingPosition.Top);
            _moddedObject.GetObject<Transform>(8).gameObject.SetActive(position == ESettingPosition.Center);
            _moddedObject.GetObject<Transform>(9).gameObject.SetActive(position == ESettingPosition.Bottom);

            configToggle(moddedObject, Setting.Type == ESettingType.Bool, position);
        }

        private void configToggle(in ModdedObject m, in bool isBool, in ESettingPosition position)
        {
            Toggle_TickBox = m.GetObject<Transform>(2);
            Toggle_BGOff = m.GetObject<Transform>(3);
            Toggle_BGOn = m.GetObject<Transform>(4);
            Toggle_Tick = m.GetObject<Transform>(5);
            Toggle_TickBox.gameObject.SetActive(isBool);

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
                    t.targetGraphic = _moddedObject.GetObject<Image>(7);
                    break;
                case ESettingPosition.Center:
                    t.targetGraphic = _moddedObject.GetObject<Image>(8);
                    break;
                case ESettingPosition.Bottom:
                    t.targetGraphic = _moddedObject.GetObject<Image>(9);
                    break;
            }

            t.isOn = SettingInfo.GetPref<bool>(Setting);
            setToggleValue(t.isOn);
            t.onValueChanged.AddListener(setToggleValue);
        }
        private void setToggleValue(bool value)
        {
            Toggle_BGOff.gameObject.SetActive(!value);
            Toggle_BGOn.gameObject.SetActive(value);
            Toggle_Tick.gameObject.SetActive(value);
            SettingInfo.SavePref(Setting, value);
        }

        private void OnDestroy()
        {
            if (!_spawnedBehaviours.Contains(this))
            {
                return;
            }
            _spawnedBehaviours.Remove(this);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _ui.PopulateDescription(Setting, Description);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _ui.PopulateDescription(null, null);
        }
    }
}