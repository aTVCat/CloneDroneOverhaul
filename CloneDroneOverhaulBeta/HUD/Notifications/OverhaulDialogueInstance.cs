using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulDialogueInstance : OverhaulBehaviour
    {
        private Button m_CloseButton;
        private Slider m_ProgressSlider;

        private float m_TimeAppeared;
        private float m_TimeToDisappear;

        public void Initialize()
        {
            m_TimeAppeared = Time.unscaledTime;
            m_TimeToDisappear = m_TimeAppeared + 6f;

            ModdedObject m = base.GetComponent<ModdedObject>();
            m_CloseButton = m.GetObject<Button>(1);
            m_CloseButton.onClick.AddListener(DestroyGameObject);
            m_ProgressSlider = m.GetObject<Slider>(5);
            m_ProgressSlider.value = 0f;
            m_ProgressSlider.maxValue = m_TimeToDisappear - m_TimeAppeared;
        }

        private void Update()
        {
            float time = Time.unscaledTime;

            m_ProgressSlider.value = time - m_TimeAppeared;
            if(time >= m_TimeToDisappear)
            {
                DestroyGameObject();
            }
        }
    }
}