using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPressActionKeyDescription : OverhaulUIBehaviour
    {
        [UIElement("BG")]
        private readonly RectTransform m_bg;

        [UIElement("BG", false)]
        private readonly GameObject m_bgObject;

        [UIElement("BG")]
        private readonly CanvasGroup m_bgCanvasGroup;

        [UIElement("Text")]
        private readonly Text m_text;

        public override bool closeOnEscapeButtonPress => false;

        private float m_expandProgress;

        private bool m_show;

        public override void Update()
        {
            Text textComponent = m_text;

            RectTransform rt = m_bg;
            Vector2 sd = rt.sizeDelta;
            sd.x = Mathf.Lerp(0f, Mathf.Clamp(textComponent.preferredWidth + 10f, 100f, 200f), NumberUtils.EaseOutQuad(0f, 1f, m_expandProgress));
            sd.y = Mathf.Lerp(0f, textComponent.preferredHeight + 10f, NumberUtils.EaseOutQuad(0f, 1f, m_expandProgress));
            rt.sizeDelta = sd;

            m_bgObject.SetActive(m_expandProgress > 0f);
            if (!m_show && m_expandProgress == 0f)
            {
                if (!textComponent.text.IsNullOrEmpty())
                    textComponent.text = null;
            }

            m_expandProgress = Mathf.Clamp01(m_expandProgress + ((m_show ? 1f : -1f) * Time.unscaledDeltaTime * 7.5f));
        }

        public void ShowText(string text)
        {
            m_text.text = text;
            m_show = true;
        }

        public void HideText()
        {
            m_show = false;
        }
    }
}
