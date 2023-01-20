using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using CloneDroneOverhaul.V3.Base;

namespace CloneDroneOverhaul.V3.HUD
{
    public class UISettingButtonBehaviour : MonoBehaviour
    {
        private HUD.UIModSettings _ui;

        public ModSetting MySetting { get; set; }
        public bool IsHidden { get; set; }

        private Text Header;
        private Text Description;

        private bool _isShowingChildren;

        public UISettingButtonBehaviour Initialize(in ModSetting setting, in HUD.UIModSettings ui)
        {
            ModdedObject component = base.GetComponent<ModdedObject>();
            _ui = ui;
            MySetting = setting;

            Header = component.GetObjectFromList<Text>(0);
            Header.text = setting.SettingName;
            Description = component.GetObjectFromList<Text>(1);
            Description.text = OverhaulMain.GetTranslatedString("SDesc_" + setting.RawPath);

            IsHidden = setting.IsHiddenFromMainView;
            base.gameObject.SetActive(!IsHidden);

            component.GetObjectFromList<Button>(9).gameObject.SetActive(setting.HasChildrenSettings);
            component.GetObjectFromList<Button>(9).onClick.AddListener(onChildrenSettingsClicked);
            if (IsHidden) component.GetObjectFromList<Image>(8).color = "2C2E35".hexToColor();

            if (setting.Type == EPlayerPrefType.Bool)
            {
                Toggle toggle = component.GetObjectFromList<Toggle>(4);
                toggle.isOn = (bool)ModDataController.GetPlayerPref(setting.RawPath, setting.Type);
                toggle.onValueChanged.AddListener(setBool);
                component.GetObjectFromList<Transform>(3).gameObject.SetActive(toggle.isOn);
            }
            if (setting.Type == EPlayerPrefType.Float || setting.Type == EPlayerPrefType.Int)
            {
                if (setting.Type == EPlayerPrefType.Int && setting.Dropdown_Options != null)
                {
                    Dropdown dropdown = component.GetObjectFromList<Dropdown>(2);
                    if (setting.Dropdown_Translate)
                    {
                        dropdown.options = new List<Dropdown.OptionData>();
                        foreach (Dropdown.OptionData data in setting.Dropdown_Options)
                        {
                            dropdown.options.Add(new Dropdown.OptionData(OverhaulMain.GetTranslatedString("SDropdown_" + data.text)));
                        }
                    }
                    else
                    {
                        dropdown.options = setting.Dropdown_Options;
                    }
                    dropdown.value = (int)ModDataController.GetPlayerPref(setting.RawPath, setting.Type);
                    dropdown.onValueChanged.AddListener(setInt);
                    return this;
                }

                Slider slider = component.GetObjectFromList<Slider>(2);
                slider.maxValue = setting.Slider_Range.Max;
                slider.minValue = setting.Slider_Range.Min;
                slider.wholeNumbers = setting.Slider_Int;
  
                if (slider.wholeNumbers)
                {
                    slider.value = (int)ModDataController.GetPlayerPref(setting.RawPath, setting.Type);
                    slider.onValueChanged.AddListener(setInt);
                }
                else
                {
                    slider.value = (float)ModDataController.GetPlayerPrefFloat(setting.RawPath);
                    slider.onValueChanged.AddListener(setFloat);
                }
            }
            if(setting.Type == EPlayerPrefType.String)
            {
                InputField field = component.GetObjectFromList<InputField>(2);
                field.text = (string)ModDataController.GetPlayerPref(setting.RawPath, setting.Type);
                field.onEndEdit.AddListener(setString);
            }

            return this;
        }

        private void setBool(bool val)
        {
            ModDataController.SavePlayerPref(MySetting.RawPath, val);
            base.GetComponent<ModdedObject>().GetObjectFromList<Transform>(3).gameObject.SetActive(val);
        }

        private void setInt(int val)
        {
            ModDataController.SavePlayerPref(MySetting.RawPath, Mathf.RoundToInt(val));
        }

        private void setInt(float val)
        {
            ModDataController.SavePlayerPref(MySetting.RawPath, Mathf.RoundToInt(val));
        }

        private void setFloat(float val)
        {
            ModDataController.SavePlayerPref(MySetting.RawPath, val);
        }

        private void setString(string val)
        {
            ModDataController.SavePlayerPref(MySetting.RawPath, val);
        }

        private void onChildrenSettingsClicked()
        {
            _isShowingChildren = !_isShowingChildren;
            List<ModSetting> list = new List<ModSetting>();
            foreach (ModSetting s in MySetting.ChildrenSettings)
            {
                if (s == null)
                {
                    break;
                }
                list.Add(s);
            }

            if (!_isShowingChildren)
            {
                _ui.HideSettings(list);
                return;
            }
            _ui.ShowSettings(list);
        }
    }
}
