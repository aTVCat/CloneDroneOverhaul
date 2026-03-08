using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class BetterSliderCallback : ModBehaviour
    {
        private bool _isWaitingMouseButtonGetUp;
        private float _valueToSet;
        private Slider.SliderEvent _sliderEvent;

        public override void Awake()
        {
            Slider slider = base.GetComponent<Slider>();
            if (!slider)
            {
                base.enabled = false;
                return;
            }

            Slider.SliderEvent newSliderEvent = new Slider.SliderEvent();
            newSliderEvent.AddListener(onValueChanged);

            _sliderEvent = slider.onValueChanged;
            slider.onValueChanged = newSliderEvent;
        }

        public override void Update()
        {
            if (_isWaitingMouseButtonGetUp && !Input.GetMouseButton(0))
            {
                _isWaitingMouseButtonGetUp = false;
                _sliderEvent.Invoke(_valueToSet);
            }
        }

        private void onValueChanged(float value)
        {
            _valueToSet = value;
            _isWaitingMouseButtonGetUp = true;
        }
    }
}
