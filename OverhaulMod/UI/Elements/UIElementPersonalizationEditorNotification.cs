using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI.Elements
{
    public class UIElementPersonalizationEditorNotification : OverhaulUIBehaviour
    {
        public static readonly Color StandardColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        public static readonly Color SuccessColor = new Color(0.04f, 0.196f, 0.13f, 1f);
        public static readonly Color ErrorColor = new Color(0.3f, 0f, 0f, 1f);

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_closeButton;

        [UIElement("Header")]
        private readonly Text m_header;

        [UIElement("Description")]
        private readonly Text m_description;

        [UIElement("Fill")]
        private readonly Image m_fill;

        [UIElement("Notification")]
        private readonly Image m_bg;

        [UIElement("Frame")]
        private readonly Image m_frame;

        [UIElement("Notification")]
        private readonly CanvasGroup m_canvasGroup;

        private float m_duration;

        private float m_timeLeft;

        public override void Update()
        {
            float d = Time.unscaledDeltaTime;
            float dMultiplied = d * 12.5f;

            m_timeLeft -= d;

            m_canvasGroup.alpha += Mathf.Lerp(m_canvasGroup.alpha, 1f, dMultiplied);
            m_fill.fillAmount = (m_duration - m_timeLeft) / m_duration;

            if (m_timeLeft <= 0f)
            {
                Hide();
            }
        }

        public void ShowNotification(string header, string text, Color baseColor, float duration)
        {
            duration = Mathf.Max(duration, 5f);

            HSBColor frameHsbColor = new HSBColor(baseColor);
            frameHsbColor.b = Mathf.Clamp01(frameHsbColor.b + 0.3f);
            Color frameColor = frameHsbColor.ToColor();
            m_bg.color = baseColor;
            m_frame.color = frameColor;
            m_header.text = header;
            m_description.text = text;
            m_duration = duration;
            m_timeLeft = duration;

            m_canvasGroup.alpha = 0f;
            m_fill.fillAmount = 0f;

            Show();
        }
    }
}
