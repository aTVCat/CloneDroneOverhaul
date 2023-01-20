using System;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI.Components
{
    public static class AddAndConfigBetterSliderClass
    {
        public static BetterSlider AddAndConfigBetterSlider(this ModdedObject obj, BetterSlider.Settings settings, Action<float> onValueUpdated = null)
        {
            return obj.gameObject.AddComponent<BetterSlider>().SetUp(obj, settings, onValueUpdated);
        }
    }

    public class BetterSlider : MonoBehaviour
    {
        private Text Description;
        private Text Value;
        private Slider TheSlider;
        private Action<float> onValueUpdated;
        private string translateID;

        public BetterSlider SetUp(ModdedObject obj, Settings settings, Action<float> onValueUpdatedAction = null)
        {
            Description = obj.GetObjectFromList<Text>(0);
            Value = obj.GetObjectFromList<Text>(1);
            TheSlider = obj.GetObjectFromList<Slider>(2);
            TheSlider.minValue = settings.MinValue;
            TheSlider.maxValue = settings.MaxValue;
            TheSlider.wholeNumbers = settings.UseInt;
            TheSlider.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(OnSliderUpdated));
            onValueUpdated = onValueUpdatedAction;
            translateID = settings.IDToTranslate;

            return this;
        }

        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(translateID))
            {
                Description.text = OverhaulMain.GetTranslatedString(translateID);
            }
        }

        public void SetValue(float num)
        {
            TheSlider.value = num;
            OnSliderUpdated(num);
        }

        public void SetLimits(float min, float max)
        {
            TheSlider.minValue = min;
            TheSlider.maxValue = max;
        }

        public float SliderValue => TheSlider.value;

        public void OnSliderUpdated(float value)
        {
            Value.text = "[" + Mathf.Round(value) + "]";
            if (onValueUpdated != null)
            {
                onValueUpdated(value);
            }
        }

        public struct Settings
        {
            public float MinValue;
            public float MaxValue;
            public bool UseInt;

            public string IDToTranslate;
        }
    }
}