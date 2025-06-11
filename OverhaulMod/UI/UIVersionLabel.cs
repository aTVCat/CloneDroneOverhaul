using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIVersionLabel : OverhaulUIBehaviour
    {
        [ModSetting(ModSettingsConstants.SHOW_VERSION_LABEL, true)]
        public static bool ShowLabel;

        [ModSetting(ModSettingsConstants.SHOW_DEVELOPER_BUILD_LABEL, false)]
        public static bool ShowDeveloperBuildLabel;

        [UIElement("NewVersionLabel_TitleScreen")]
        private readonly GameObject m_watermark;

        [UIElement("NewVersionLabel_TitleScreen")]
        private readonly CanvasGroup m_watermarkCanvasGroup;

        [UIElement("DebugLabel_TitleScreen")]
        private readonly GameObject m_debugIcon;

        [UIElement("Watermark_TitleScreen")]
        private readonly Text m_versionText;

        [UIElement("NewVersionLabel_Gameplay")]
        private readonly GameObject m_gameplayWatermark;

        [UIElement("NewVersionLabel_Gameplay")]
        private readonly RectTransform m_gameplayWatermarkTransform;

        [UIElement("DebugLabel_Gameplay")]
        private readonly GameObject m_gameplayDebugIcon;

        [UIElement("Watermark_Gameplay")]
        private readonly Text m_gameplayVersionText;

        [UIElement("DeveloperBuildLabel", false)]
        private readonly GameObject m_devBuildLabelObject;

        public bool ForceHide;

        private bool m_refreshWidth;

        private bool m_fadeInLabel;

        public override bool closeOnEscapeButtonPress => false;

        public static UIVersionLabel instance
        {
            get;
            set;
        }

        public bool showWatermark
        {
            get
            {
                return ShowLabel && !GameModeManager.IsInLevelEditor() && !ModCache.photoManager.IsInPhotoMode();
            }
        }

        private float m_offsetX;
        public float offsetX
        {
            get
            {
                return m_offsetX;
            }
            set
            {
                m_offsetX = value;
                Vector2 anchoredPosition = m_gameplayWatermarkTransform.anchoredPosition;
                anchoredPosition.x = 5f + value;
                m_gameplayWatermarkTransform.anchoredPosition = anchoredPosition;
            }
        }

        protected override void OnInitialized()
        {
            instance = this;

            bool update = ModFeatures.IsEnabled(ModFeatures.FeatureType.VersionLabelUpdates);
            m_gameplayVersionText.font = update ? ModResources.EditUndoFont() : ModResources.PiksieliProstoFont();
            m_gameplayVersionText.fontSize = update ? 10 : 6;
            m_gameplayWatermarkTransform.localScale = update ? Vector3.one * 0.9f : Vector3.one;
            RefreshLabels();

            ModSettingsManager.Instance.AddSettingValueChangedListener(onDevBuildLabelSettingChanged, ModSettingsConstants.SHOW_DEVELOPER_BUILD_LABEL);
            onDevBuildLabelSettingChanged(ShowDeveloperBuildLabel);

            ModCache.titleScreenUI.VersionLabel.gameObject.SetActive(false);

            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.Intro) && GameModeManager.IsOnTitleScreen())
                m_watermarkCanvasGroup.alpha = UIIntro.HasEverShownIntro ? 1f : 0f;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            instance = null;

            ModSettingsManager.Instance.RemoveSettingValueChangedListener(onDevBuildLabelSettingChanged, ModSettingsConstants.SHOW_DEVELOPER_BUILD_LABEL);
        }

        public override void Update()
        {
            if (m_refreshWidth)
            {
                m_refreshWidth = false;
                RectTransform rectTransform = m_gameplayWatermarkTransform;
                Vector2 sideDelta = rectTransform.sizeDelta;
                sideDelta.x = m_gameplayVersionText.preferredWidth + 15f;
                rectTransform.sizeDelta = sideDelta;
            }

            bool isOnTitleScreen = GameModeManager.IsOnTitleScreen();
            if (isOnTitleScreen)
            {
                if (m_fadeInLabel)
                {
                    m_watermarkCanvasGroup.alpha += Mathf.Min(Time.unscaledDeltaTime, 0.025f);
                    if (m_watermarkCanvasGroup.alpha >= 1f)
                    {
                        m_fadeInLabel = false;
                    }
                }
            }

            if (Time.frameCount % 10 != 0)
                return;

            bool show = !ForceHide && showWatermark;
            m_watermark.SetActive(show && ModCache.titleScreenUI.RootButtonsContainerBG.activeInHierarchy && isOnTitleScreen && !UITitleScreenHypocrisisSkin.HideVersionLabel);
            m_gameplayWatermark.SetActive(show && !isOnTitleScreen);
        }

        public void RefreshLabels()
        {
            bool debug = ModBuildInfo.debug;
            m_versionText.text = $"OVERHAUL {ModBuildInfo.fullVersionString.ToUpper()}\nCLONE DRONE {VersionNumberManager.Instance.GetVersionString()}";
            m_debugIcon.SetActive(debug);
            m_gameplayVersionText.text = $"OVERHAUL {ModBuildInfo.versionString.ToUpper()}";
            m_gameplayDebugIcon.SetActive(debug);
            m_refreshWidth = true;
        }

        public void ShowTitleScreenLabel()
        {
            m_fadeInLabel = true;
        }

        private void onDevBuildLabelSettingChanged(object obj)
        {
            m_devBuildLabelObject.SetActive(obj is bool b && ModBuildInfo.isDeveloperBuild && ModBuildInfo.debug && b);
        }

        public void OnOtherModsButtonClicked()
        {
            _ = ModUIConstants.ShowOtherModsMenu();
        }
    }
}
