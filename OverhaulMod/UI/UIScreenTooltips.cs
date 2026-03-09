using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIScreenTooltips : OverhaulUIBehaviour
    {
        public static UIScreenTooltips instance
        {
            get;
            private set;
        }

        [UIElement("Panel")]
        private readonly CanvasGroup _canvasGroup;

        [UIElement("Text")]
        private readonly Text _text;

        private float _alpha;

        private float _timeLeft;

        public override bool RefreshOnlyCursor => true;
        public override bool CloseOnEscapeButtonPress => false;

        protected override void OnInitialized()
        {
            instance = this;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }

        public override void Update()
        {
            base.Update();

            float d = Time.unscaledDeltaTime;
            _alpha = Mathf.Lerp(_alpha, _timeLeft > 0f ? 1f : 0f, d * 10f);
            _canvasGroup.alpha = NumberUtils.EaseInOutCubic(0f, 1f, _alpha);

            if (_timeLeft > 0f)
                _timeLeft -= d;
        }

        public void ShowText(string text, float duration)
        {
            _text.text = text;
            _timeLeft = duration;
        }
    }
}
