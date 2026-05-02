using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPENotification : OverhaulUIBehaviour
    {
        public static readonly Color StandardColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        public static readonly Color SuccessColor = new Color(0.04f, 0.196f, 0.13f, 1f);
        public static readonly Color ErrorColor = new Color(0.3f, 0f, 0f, 1f);

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        [UIElement("Header")]
        private readonly Text _header;

        [UIElement("Description")]
        private readonly Text _description;

        [UIElement("Fill")]
        private readonly Image _fill;

        [UIElement("Notification")]
        private readonly Image _bg;

        [UIElement("Frame")]
        private readonly Image _frame;

        [UIElement("Notification")]
        private readonly CanvasGroup _canvasGroup;

        private float _duration;

        private float _timeLeft;

        public override void Update()
        {
            float d = Time.unscaledDeltaTime;
            float dMultiplied = d * 12.5f;

            _timeLeft -= d;

            _canvasGroup.alpha += Mathf.Lerp(_canvasGroup.alpha, 1f, dMultiplied);
            _fill.fillAmount = (_duration - _timeLeft) / _duration;

            if (_timeLeft <= 0f)
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
            _bg.color = baseColor;
            _frame.color = frameColor;
            _header.text = header;
            _description.text = text;
            _duration = duration;
            _timeLeft = duration;

            _canvasGroup.alpha = 0f;
            _fill.fillAmount = 0f;

            Show();
        }
    }
}
