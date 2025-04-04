using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class TransitionBehaviour : ModBehaviour
    {
        public const string PRE_43_BG_COLOR = "0D0D0D";
        public const string POST_43_BG_COLOR = "060E1A";

        private Image m_image;
        private CanvasGroup m_canvasGroup;

        private Text m_text;
        private Text m_tipLabel;
        private Outline m_loadingLabelOutline;
        private CanvasGroup m_loadingIndicator;

        private ErrorManager m_errorManager;

        private Color m_loadingLabelOutlineColor;

        private float m_timeToFade;

        private bool m_use43Variant;

        private bool m_destroyed;

        public bool fadeOut
        {
            get;
            set;
        }

        public float deltaTimeMultiplier
        {
            get;
            set;
        }

        public float waitBeforeFadeOut
        {
            get;
            set;
        }

        public override void Awake()
        {
            m_use43Variant = ModFeatures.IsEnabled(ModFeatures.FeatureType.UpdatedTransitions);

            m_image = base.GetComponent<Image>();
            m_canvasGroup = base.GetComponent<CanvasGroup>();

            m_text = moddedObjectReference.GetObject<Text>(0);
            m_loadingIndicator = moddedObjectReference.GetObject<CanvasGroup>(1);
            m_tipLabel = moddedObjectReference.GetObject<Text>(2);
            m_loadingLabelOutline = moddedObjectReference.GetObject<Outline>(3);

            m_loadingLabelOutlineColor = m_loadingLabelOutline.effectColor;
        }

        public override void Start()
        {
            m_errorManager = ErrorManager.Instance;
            if (TransitionManager.TransitionSound && !fadeOut)
                ModAudioManager.Instance.PlayTransitionSound();

            ModUIConstants.HideLoadingScreen();
        }

        public override void OnDestroy()
        {
            ModAudioManager.Instance.StopTransitionSound();
        }

        public override void Update()
        {
            if (m_destroyed)
                return;

            ErrorManager errorManager = m_errorManager;
            if (errorManager && errorManager.HasCrashed())
            {
                m_destroyed = true;
                Destroy(base.gameObject);
                return;
            }

            bool fo = fadeOut;
            if (fo && m_timeToFade > Time.unscaledTime)
                return;

            float alpha = m_canvasGroup.alpha;
            alpha = Mathf.Lerp(alpha, fo ? 0f : 1f, Mathf.Min(Time.unscaledDeltaTime, 0.016f) * deltaTimeMultiplier);
            m_canvasGroup.alpha = alpha;
            m_canvasGroup.blocksRaycasts = alpha >= 0.9f;

            if (m_use43Variant)
            {
                m_loadingIndicator.alpha = alpha;

                Color color = m_tipLabel.color;
                color.a = alpha;
                m_tipLabel.color = color;

                Color outlineColor = m_loadingLabelOutline.effectColor;
                outlineColor.a = (Mathf.Clamp01(alpha - 0.7f) * 10f) - 2f;
                m_loadingLabelOutline.effectColor = outlineColor;
            }

            if (fo && alpha <= 0.05f)
            {
                Destroy(base.gameObject);
            }
        }

        public void SetElementsVisible(bool textValue, bool tipValue)
        {
            m_text.gameObject.SetActive(!m_use43Variant && textValue);
            m_tipLabel.gameObject.SetActive(ModFeatures.IsEnabled(ModFeatures.FeatureType.TransitionTips) && m_use43Variant && tipValue);
            m_loadingIndicator.gameObject.SetActive(m_use43Variant && textValue);
        }

        public void SetBackgroundColor(Color color)
        {
            m_image.color = color;
        }

        public void RunCoroutine(IEnumerator enumerator)
        {
            if (enumerator == null)
                return;

            _ = ModActionUtils.RunCoroutine(enumerator);
        }

        public void Refresh()
        {
            m_canvasGroup.alpha = fadeOut ? 1f : 0f;
            m_timeToFade = Time.unscaledTime + waitBeforeFadeOut;
        }
    }
}
