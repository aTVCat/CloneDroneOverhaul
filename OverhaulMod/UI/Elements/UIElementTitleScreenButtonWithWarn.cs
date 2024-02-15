using OverhaulMod.Content;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementTitleScreenButtonWithWarn : OverhaulUIBehaviour
    {
        [UIElement("WarnIndicator", false)]
        private GameObject m_indicator;

        private Animator m_animator;

        private float m_timeToUpdate;

        public bool isUpdatesButton
        {
            get;
            set;
        }

        public bool isNewsButton
        {
            get;
            set;
        }

        public override void Start()
        {
            m_animator = base.GetComponent<Animator>();
            m_timeToUpdate = 3f;
        }

        public override void Update()
        {
            float d = Time.unscaledDeltaTime;
            m_timeToUpdate -= d;
            if(m_timeToUpdate < 0f)
            {
                m_timeToUpdate = 3f;
                SetWarnActive((isNewsButton && NewsManager.Instance.ShouldHighlightNewsButton()) || (isUpdatesButton && UpdateManager.Instance.ShouldHighlightUpdatesButton()));
            }
        }

        public void SetWarnActive(bool value)
        {
            m_indicator.SetActive(value);
            m_animator.enabled = value;
        }
    }
}
