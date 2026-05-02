using OverhaulMod.UI;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class TransitionBehaviour : ModBehaviour
    {
        public const int FRAMES_TO_CHANGE_LOADING_TEXT = 10;

        public bool FadeOut;

        public float DeltaTimeMultiplier;

        public float WaitBeforeFadeOut;

        public bool IsCrossScene;

        private Image _bg;

        private CanvasGroup _canvasGroup;

        private GameObject _loadingIndicator;

        private Text _loadingLabel;

        private Outline _loadingLabelOutline;

        private CDHDLoadingAnimation _loadingAnimation;

        private ErrorManager _errorManager;

        private float _timeToFade;

        private bool _isChangingLoadingText;

        public override void Awake()
        {
            _canvasGroup = base.GetComponent<CanvasGroup>();
            _bg = base.GetComponent<Image>();
            _loadingIndicator = ModdedObjectComponent.GetObject<GameObject>(0);
            _loadingLabel = ModdedObjectComponent.GetObject<Text>(1);
            _loadingLabelOutline = _loadingLabel.GetComponent<Outline>();

            _loadingAnimation = base.gameObject.AddComponent<CDHDLoadingAnimation>();
            _loadingAnimation.SetImage(ModdedObjectComponent.GetObject<Image>(2));
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
            if (IsCrossScene) // hide cross scene canvas
            {
                base.transform.parent.gameObject.SetActive(false);
            }
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

            if (!_isChangingLoadingText)
            {
                Color outlineColor = _loadingLabelOutline.effectColor;
                outlineColor.a = (Mathf.Clamp01(alpha - 0.7f) * 10f) - 2f;
                _loadingLabelOutline.effectColor = outlineColor;
            }

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

        public void ChangeLoadingText(string localizedStringId)
        {
            if (isActiveAndEnabled) StartCoroutine(changeLoadingTextCoroutine(localizedStringId));
        }

        public void RunCoroutine(IEnumerator enumerator)
        {
            if (enumerator != null) enumerator.Run(IsCrossScene);
        }

        private IEnumerator changeLoadingTextCoroutine(string localizedStringId)
        {
            _isChangingLoadingText = true;
            float initalAlpha = _loadingLabelOutline.effectColor.a;
            for (int i = 0; i < FRAMES_TO_CHANGE_LOADING_TEXT; i++)
            {
                Color outlineColor = _loadingLabelOutline.effectColor;
                outlineColor.a -= 1f / FRAMES_TO_CHANGE_LOADING_TEXT * initalAlpha;
                _loadingLabelOutline.effectColor = outlineColor;
                yield return null;
            }

            _loadingLabel.GetComponent<LocalizedTextField>().ChangeIDAndTryLocalize(localizedStringId);

            for (int i = 0; i < FRAMES_TO_CHANGE_LOADING_TEXT; i++)
            {
                Color outlineColor = _loadingLabelOutline.effectColor;
                outlineColor.a += 1f / FRAMES_TO_CHANGE_LOADING_TEXT;
                _loadingLabelOutline.effectColor = outlineColor;
                yield return null;
            }
            _isChangingLoadingText = false;

            yield break;
        }
    }
}