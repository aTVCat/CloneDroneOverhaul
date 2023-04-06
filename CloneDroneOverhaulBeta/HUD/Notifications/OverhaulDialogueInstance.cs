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

        public void Initialize(float additonalTime = 0f, OverhaulDialogues.Button[] buttons = null)
        {
            m_TimeAppeared = Time.unscaledTime;
            m_TimeToDisappear = m_TimeAppeared + 6f + additonalTime;

            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_CloseButton = moddedObject.GetObject<Button>(1);
            m_CloseButton.onClick.AddListener(DestroyGameObject);
            m_ProgressSlider = moddedObject.GetObject<Slider>(5);
            m_ProgressSlider.value = 0f;
            m_ProgressSlider.maxValue = m_TimeToDisappear - m_TimeAppeared;

            if (!buttons.IsNullOrEmpty())
            {
                foreach (OverhaulDialogues.Button b in buttons)
                {
                    ModdedObject mb = Instantiate(moddedObject.GetObject<Transform>(3).gameObject, moddedObject.GetObject<Transform>(4)).GetComponent<ModdedObject>();
                    mb.GetObject<Text>(0).text = b.Title;
                    if (b.Action != null)
                    {
                        mb.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(b.Action);
                    }
                    else
                    {
                        mb.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DestroyGameObject);
                    }
                    mb.gameObject.SetActive(true);
                }
            }
        }

        private void Update()
        {
            float time = Time.unscaledTime;

            m_ProgressSlider.value = time - m_TimeAppeared;
            if (time >= m_TimeToDisappear)
            {
                DestroyGameObject();
            }
        }
    }
}