using OverhaulMod.Content;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class UIElementTitleScreenButtonWithWarn : OverhaulUIBehaviour
    {
        [UIElement("WarnIndicator", false)]
        private readonly GameObject _indicator;

        private Animator _animator;

        private float _timeToUpdate;

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
            _animator = base.GetComponent<Animator>();
            _timeToUpdate = 3f;
        }

        public override void Update()
        {
            float d = Time.unscaledDeltaTime;
            _timeToUpdate -= d;
            if (_timeToUpdate < 0f)
            {
                _timeToUpdate = 3f;
                SetWarnActive((isNewsButton && NewsManager.Instance.ShouldHighlightNewsButton()) || (isUpdatesButton && UpdateManager.Instance.ShouldHighlightUpdatesButton()));
            }
        }

        public void SetWarnActive(bool value)
        {
            _indicator.SetActive(value);
            _animator.enabled = value;
        }
    }
}
