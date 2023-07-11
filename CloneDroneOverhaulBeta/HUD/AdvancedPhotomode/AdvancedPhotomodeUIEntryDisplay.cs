using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.Graphics;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class AdvancedPhotomodeUIEntryDisplay : OverhaulBehaviour
    {
        public Toggle ToggleReference
        {
            get;
            private set;
        }

        public Slider SliderReference
        {
            get;
            private set;
        }

        public AdvancedPhotomodeSettingAttribute SettingReference
        {
            get;
            private set;
        }

        private GameObject m_CheckmarkObject;

        public void Initialize(AdvancedPhotomodeSettingAttribute setting)
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();

            ToggleReference = moddedObject.GetObject<Toggle>(1);
            SliderReference = moddedObject.GetObject<Slider>(2);
            SettingReference = setting;

            m_CheckmarkObject = moddedObject.GetObject<Transform>(3).gameObject;

            Populate();
        }

        public void Populate()
        {
            bool isBoolean = SettingReference.Field.FieldType == typeof(bool);
            bool isFloat = SettingReference.Field.FieldType == typeof(float);
            bool isInt = SettingReference.Field.FieldType == typeof(int);

            PopulateToggle(isBoolean);
            PopulateSlider(isFloat || isInt, isFloat);

            if(SettingReference.ContentParameters != null)
            {
                bool hasLoaded = SettingReference.ContentParameters.HasLoadedContent();
                ToggleReference.interactable = hasLoaded;
                SliderReference.interactable = hasLoaded;
            }
        }

        public void SetToggleVisible(bool value)
        {
            ToggleReference.gameObject.SetActive(value);
        }

        public void PopulateToggle(bool settingIsBoolean)
        {
            SetToggleVisible(settingIsBoolean);
            if (!settingIsBoolean)
                return;

            bool value = (bool)SettingReference.Field.GetValue(null);

            AdvancedPhotomodeUITogglePatcher patcher = ToggleReference.gameObject.AddComponent<AdvancedPhotomodeUITogglePatcher>();
            patcher.ToggleReference = ToggleReference;
            patcher.ObjectToToggle = m_CheckmarkObject;
            patcher.ObjectToToggle.SetActive(value);

            ToggleReference.isOn = value;
            ToggleReference.onValueChanged.AddListener(SetToggleValue);
        }

        public void SetToggleValue(bool value)
        {
            ToggleReference.OnDeselect(null);
            SettingReference.Field.SetValue(null, value);
            OverhaulEventsController.DispatchEvent(AdvancedPhotomodeController.PhotoModeSettingUpdateEvent);
        }

        public void SetSliderVisible(bool value)
        {
            SliderReference.gameObject.SetActive(value);
            OverhaulEventsController.DispatchEvent(AdvancedPhotomodeController.PhotoModeSettingUpdateEvent);
        }

        public void PopulateSlider(bool settingIsNumber, bool isFloat)
        {
            SetSliderVisible(settingIsNumber);
            if (!settingIsNumber)
                return;

            if (SettingReference.SliderParameters == null)
            {
                SliderReference.interactable = false;
                return;
            }

            if (isFloat)
                SliderReference.value = (float)SettingReference.Field.GetValue(null);
            else
                SliderReference.value = (int)SettingReference.Field.GetValue(null);
            SliderReference.wholeNumbers = !isFloat;
            SliderReference.minValue = SettingReference.SliderParameters.MinValue;
            SliderReference.maxValue = SettingReference.SliderParameters.MaxValue;
            SliderReference.onValueChanged.AddListener(SetSliderValue);
        }

        public void SetSliderValue(float value)
        {
            bool isFloat = SettingReference.Field.FieldType == typeof(float);

            if(isFloat)
                SettingReference.Field.SetValue(null, value);
            else
                SettingReference.Field.SetValue(null, (int)value);
            OverhaulGraphicsController.PatchAllCameras();
            OverhaulEventsController.DispatchEvent(AdvancedPhotomodeController.PhotoModeSettingUpdateEvent);
        }
    }
}
