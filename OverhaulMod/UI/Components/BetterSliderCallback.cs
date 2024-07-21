using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class BetterSliderCallback : ModBehaviour
    {
        private bool m_isWaitingMouseButtonGetUp;
        private float m_valueToSet;
        private Slider.SliderEvent m_sliderEvent;

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

            m_sliderEvent = slider.onValueChanged;
            slider.onValueChanged = newSliderEvent;
        }

        public override void Update()
        {
            if (m_isWaitingMouseButtonGetUp && !Input.GetMouseButton(0))
            {
                m_isWaitingMouseButtonGetUp = false;
                m_sliderEvent.Invoke(m_valueToSet);
            }
        }

        private void onValueChanged(float value)
        {
            m_valueToSet = value;
            m_isWaitingMouseButtonGetUp = true;
        }
    }
}
