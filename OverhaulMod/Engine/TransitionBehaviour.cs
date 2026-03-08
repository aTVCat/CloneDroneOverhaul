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

        private Image m_bg;

        private CanvasGroup m_canvasGroup;

        private GameObject m_loadingIndicator;

        private Outline m_loadingLabelOutline;

        private ErrorManager m_errorManager;

        private float m_timeToFade;

        public override void Awake()
        {
            m_canvasGroup = base.GetComponent<CanvasGroup>();
            m_bg = base.GetComponent<Image>();
            m_loadingIndicator = moddedObjectReference.GetObject<GameObject>(0);
            m_loadingLabelOutline = moddedObjectReference.GetObject<Outline>(1);
        }

        public override void Start()
        {
            m_errorManager = ErrorManager.Instance;
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
            ErrorManager errorManager = m_errorManager;
            if (errorManager && errorManager.HasCrashed())
            {
                Destroy(base.gameObject);
                return;
            }

            bool fo = FadeOut;
            if (fo && m_timeToFade > Time.unscaledTime) return;

            float alpha = m_canvasGroup.alpha;
            alpha = Mathf.Lerp(alpha, fo ? 0f : 1f, Mathf.Min(Time.unscaledDeltaTime, 0.016f) * DeltaTimeMultiplier);
            m_canvasGroup.alpha = alpha;
            m_canvasGroup.blocksRaycasts = alpha >= 0.9f;

            Color outlineColor = m_loadingLabelOutline.effectColor;
            outlineColor.a = (Mathf.Clamp01(alpha - 0.7f) * 10f) - 2f;
            m_loadingLabelOutline.effectColor = outlineColor;

            if (fo && alpha <= 0.05f) Destroy(base.gameObject);
        }

        public void SetColor(Color color)
        {
            m_bg.color = color;
        }

        public void SetLoadingIndicatorActive(bool value)
        {
            m_loadingIndicator.SetActive(value);
        }

        public void StartFading()
        {
            m_canvasGroup.alpha = FadeOut ? 1f : 0f;
            m_timeToFade = Time.unscaledTime + WaitBeforeFadeOut;
        }

        public void RunCoroutine(IEnumerator enumerator)
        {
            if (enumerator != null) _ = ModActionUtils.RunCoroutine(enumerator);
        }
    }
}