using UnityEngine;

namespace OverhaulMod.UI
{
    public class UIIntro : OverhaulUIBehaviour
    {
        public static bool HasEverShownIntro;

        private CanvasGroup _canvasGroup;

        private bool _fadeOut;

        private float _timeout;

        private ErrorManager _errorManager;

        protected override void OnInitialized()
        {
            _canvasGroup = base.GetComponent<CanvasGroup>();
            _errorManager = ErrorManager.Instance;
        }

        public override void Show()
        {
            base.Show();
            _canvasGroup.alpha = 1f;
            _timeout = Time.unscaledTime + 15f;
        }

        public override void Start()
        {
            HasEverShownIntro = true;
        }

        public override void Update()
        {
            if (!_fadeOut && (!_errorManager || _errorManager.HasCrashed() || Time.unscaledTime > _timeout))
            {
                _fadeOut = true;
            }

            if (_fadeOut)
            {
                _canvasGroup.alpha -= Time.unscaledDeltaTime * 2.5f;
                if (_canvasGroup.alpha <= 0f)
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
            _fadeOut = true;
        }
    }
}
