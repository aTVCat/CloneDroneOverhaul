using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIScreenTooltips : OverhaulUIBehaviour
    {
        private const float START_SCALE = 1.25f;

        private const float END_SCALE = 1f;

        public static UIScreenTooltips Instance
        {
            get;
            private set;
        }

        [UIElement("Panel")]
        private readonly CanvasGroup _canvasGroup;

        [UIElement("Panel")]
        private readonly Transform _paneTransform;

        [UIElement("Text")]
        private readonly Text _text;

        private float _alpha;

        private float _timeLeft;

        private float _scaleProgress;

        private bool _hasFaded;

        public override bool RefreshOnlyCursor => true;
        public override bool CloseOnEscapeButtonPress => false;

        protected override void OnInitialized()
        {
            Instance = this;
            _hasFaded = true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }

        public override void Update()
        {
            base.Update();

            float deltaTime = Time.unscaledDeltaTime;

            _scaleProgress = Mathf.Min(1f, _scaleProgress + (deltaTime * 5f));
            float scaleMultiplier = Mathf.Lerp(START_SCALE, END_SCALE, _scaleProgress);
            _paneTransform.localScale = Vector3.one * scaleMultiplier;

            _alpha = Mathf.Lerp(_alpha, _timeLeft > 0f ? 1f : 0f, deltaTime * 10f);
            _canvasGroup.alpha = NumberUtils.EaseInOutCubic(0f, 1f, _alpha);

            _timeLeft = Mathf.Max(0f, _timeLeft - deltaTime);
            if (_timeLeft == 0f) _hasFaded = true;
        }

        public void ShowText(string text, float duration)
        {
            bool newText = _hasFaded || text != _text.text;
            _text.text = text;
            _timeLeft = duration;
            _hasFaded = false;

            if (newText) _scaleProgress = 0f;
        }
    }
}
