using UnityEngine;

namespace OverhaulMod.UI
{
    public class UIIntro : OverhaulUIBehaviour
    {
        public static bool HasEverShownIntro;

        private CanvasGroup m_canvasGroup;

        private bool m_fadeOut;

        private float m_timeout;

        protected override void OnInitialized()
        {
            m_canvasGroup = base.GetComponent<CanvasGroup>();
        }

        public override void Show()
        {
            base.Show();
            m_canvasGroup.alpha = 1f;
            m_timeout = Time.unscaledTime + 15f;
        }

        public override void Start()
        {
            HasEverShownIntro = true;
        }

        public override void Update()
        {
            if (!m_fadeOut && Time.unscaledTime > m_timeout)
            {
                m_fadeOut = true;
            }

            if (m_fadeOut)
            {
                m_canvasGroup.alpha -= Time.unscaledDeltaTime * 2.5f;
                if (m_canvasGroup.alpha <= 0f)
                {
                    DestroyThis();
                    UIVersionLabel versionLabel = UIVersionLabel.instance;
                    if (versionLabel)
                        versionLabel.ShowTitleScreenLabel();
                }
            }
        }

        public void StartFadingOut()
        {
            m_fadeOut = true;
        }
    }
}
