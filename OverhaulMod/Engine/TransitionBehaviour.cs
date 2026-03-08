using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class TransitionBehaviour : ModBehaviour
    {
        public bool FadeOut;

        public float DeltaTimeMultiplier;

        public float WaitBeforeFadeOut;

        private Image _bg;

        private CanvasGroup _canvasGroup;

        private GameObject _loadingIndicator;

        private Outline _loadingLabelOutline;

        private ErrorManager _errorManager;

        private float _timeToFade;

        public override void Awake()
        {
            _canvasGroup = base.GetComponent<CanvasGroup>();
            _bg = base.GetComponent<Image>();
            _loadingIndicator = moddedObjectReference.GetObject<GameObject>(0);
            _loadingLabelOutline = moddedObjectReference.GetObject<Outline>(1);
        }

        public override void Start()
        {
            _errorManager = ErrorManager.Instance;
            if (TransitionManager.TransitionSound && !FadeOut)
                ModAudioManager.Instance.PlayTransitionSound();

            ModUIConstants.HideLoadingScreen();
        }

        public override void OnDestroy()
        {
            ModAudioManager.Instance.StopTransitionSound();
        }

        public override void Update()
        {
            ErrorManager errorManager = _errorManager;
            if (errorManager && errorManager.HasCrashed())
            {
                Destroy(base.gameObject);
                return;
            }

            bool fo = FadeOut;
            if (fo && _timeToFade > Time.unscaledTime) return;

            float alpha = _canvasGroup.alpha;
            alpha = Mathf.Lerp(alpha, fo ? 0f : 1f, Mathf.Min(Time.unscaledDeltaTime, 0.016f) * DeltaTimeMultiplier);
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = alpha >= 0.9f;

            Color outlineColor = _loadingLabelOutline.effectColor;
            outlineColor.a = (Mathf.Clamp01(alpha - 0.7f) * 10f) - 2f;
            _loadingLabelOutline.effectColor = outlineColor;

            if (fo && alpha <= 0.05f) Destroy(base.gameObject);
        }

        public void SetColor(Color color)
        {
            _bg.color = color;
        }

        public void SetLoadingIndicatorActive(bool value)
        {
            _loadingIndicator.SetActive(value);
        }

        public void StartFading()
        {
            _canvasGroup.alpha = FadeOut ? 1f : 0f;
            _timeToFade = Time.unscaledTime + WaitBeforeFadeOut;
        }

        public void RunCoroutine(IEnumerator enumerator)
        {
            if (enumerator != null) _ = ModActionUtils.RunCoroutine(enumerator);
        }
    }
}