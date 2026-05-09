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

        [UIElement("DebugLabel_TitleScreen")]
        private readonly GameObject _debugIcon;

        [UIElement("Watermark_TitleScreen")]
        private readonly Text _versionText;

        [UIElement("NewVersionLabel_Gameplay")]
        private readonly GameObject _gameplayWatermark;

        [UIElement("NewVersionLabel_Gameplay")]
        private readonly RectTransform _gameplayWatermarkTransform;

        [UIElement("DebugLabel_Gameplay")]
        private readonly GameObject _gameplayDebugIcon;

        [UIElement("Watermark_Gameplay")]
        private readonly Text _gameplayVersionText;

        public bool ForceHide;

        private bool _refreshWidth;

        private bool _fadeInLabel;

        public override bool CloseOnEscapeButtonPress => false;

        public static UIVersionLabel Instance
        {
            get;
            set;
        }

        private float _offsetX;
        public float OffsetX
        {
            get
            {
                return _offsetX;
            }
            set
            {
                _offsetX = value;
                Vector2 anchoredPosition = _gameplayWatermarkTransform.anchoredPosition;
                anchoredPosition.x = 5f + value;
                _gameplayWatermarkTransform.anchoredPosition = anchoredPosition;
            }
        }

        protected override void OnInitialized()
        {
            Instance = this;
            _gameplayVersionText.font = ModResources.EditUndoFont();
            _gameplayVersionText.fontSize = 10;
            _gameplayWatermarkTransform.localScale = Vector3.one * 0.8f;
            RefreshLabels();

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
            if (_refreshWidth)
            {
                _refreshWidth = false;
                RectTransform rectTransform = _gameplayWatermarkTransform;
                Vector2 sideDelta = rectTransform.sizeDelta;
                sideDelta.x = _gameplayVersionText.preferredWidth + 15f;
                rectTransform.sizeDelta = sideDelta;
            }

            bool showGameplayWatermarkOnTitleScreen = TitleScreenCustomizationManager.PanelPosition == TitleScreenPanelPosition.Center;
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

            if (Time.frameCount % 10 != 0)
                return;

            bool show = !ForceHide && ShowLabel && canBeVisible();
            bool rootButtonsAreActive = ModCache.TitleScreenUI.RootButtonsContainerBG.activeInHierarchy;
            _watermark.SetActive(show && !showGameplayWatermarkOnTitleScreen && rootButtonsAreActive && isOnTitleScreen && !UITitleScreenHypocrisisSkin.HideVersionLabel);
            _gameplayWatermark.SetActive(show && (!isOnTitleScreen || (showGameplayWatermarkOnTitleScreen && rootButtonsAreActive && !UITitleScreenHypocrisisSkin.HideVersionLabel)));
        }

        public void RefreshLabels()
        {
            bool debug = ModBuild.IsDebugBuild;
            _versionText.text = $"OVERHAUL MOD {ModBuild.FullVersionString.ToUpper()}\nCLONE DRONE {VersionNumberManager.Instance.GetVersionString()}";
            _debugIcon.SetActive(debug);
            _gameplayVersionText.text = $"OVERHAUL {ModBuild.VersionString.ToUpper()}";
            _gameplayDebugIcon.SetActive(debug);
            _refreshWidth = true;
        }

        public void CenterGameplayWatermark()
        {
            OffsetX = (ModCache.UIRootCanvasScaler.referenceResolution.x / 2f) + _gameplayWatermarkTransform.sizeDelta.x;
        }

        public void ResetGameplayWatermark()
        {
            OffsetX = 0f;
        }

        public void ShowTitleScreenLabel()
        {
            _fadeInLabel = true;
        }

        private bool canBeVisible() => !GameModeManager.IsInLevelEditor() && !ModCache.PhotoManager.IsInPhotoMode();
    }
}