using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIVersionLabel : OverhaulUIBehaviour
    {
        [ModSetting(ModSettingIDs.SHOW_VERSION_LABEL, true)]
        public static bool ShowLabel;

        [UIElement("NewVersionLabel_TitleScreen")]
        private readonly GameObject _watermark;

        [UIElement("NewVersionLabel_TitleScreen")]
        private readonly CanvasGroup _watermarkCanvasGroup;

        [UIElement("Watermark_TitleScreen")]
        private readonly Text _versionText;

        public bool ForceHide;

        private bool _fadeInLabel;

        public override bool CloseOnEscapeButtonPress => false;

        public static UIVersionLabel Instance
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            Instance = this;
            _versionText.text = $"{ModBuild.FullVersionString.ToUpper()} · CLONE DRONE {VersionNumberManager.Instance.GetVersionString()}";

            ModCache.TitleScreenUI.VersionLabel.gameObject.SetActive(false);

            if (GameModeManager.IsOnTitleScreen())
                _watermarkCanvasGroup.alpha = UIIntro.HasEverShownIntro ? 1f : 0f;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }

        public override void Update()
        {
            bool isOnTitleScreen = GameModeManager.IsOnTitleScreen();
            if (isOnTitleScreen)
            {
                if (_fadeInLabel)
                {
                    _watermarkCanvasGroup.alpha += Mathf.Min(Time.unscaledDeltaTime, 0.025f);
                    if (_watermarkCanvasGroup.alpha >= 1f)
                    {
                        _fadeInLabel = false;
                    }
                }
            }

            bool show = !ForceHide && ShowLabel;
            bool rootButtonsAreActive = ModCache.TitleScreenUI.isActiveAndEnabled && ModCache.TitleScreenUI.RootButtonsContainerBG.activeSelf;
            _watermark.SetActive(show && rootButtonsAreActive && isOnTitleScreen && !UITitleScreenHypocrisisSkin.HideVersionLabel);
        }

        public void ShowTitleScreenLabel()
        {
            _fadeInLabel = true;
        }
    }
}