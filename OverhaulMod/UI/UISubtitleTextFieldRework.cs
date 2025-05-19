﻿using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UISubtitleTextFieldRework : OverhaulUIBehaviour
    {
        [ModSetting(ModSettingsConstants.ENABLE_SUBTITLE_TEXT_FIELD_REWORK, true)]
        public static bool EnableRework;

        [ModSetting(ModSettingsConstants.SUBTITLE_TEXT_FIELD_UPPER_POSITION, true)]
        public static bool BeOnTop;

        [ModSetting(ModSettingsConstants.SUBTITLE_TEXT_FIELD_BG, false)]
        public static bool EnableBG;

        [ModSetting(ModSettingsConstants.SUBTITLE_TEXT_FIELD_FONT, 1)]
        public static int FontType;

        [ModSetting(ModSettingsConstants.SUBTITLE_TEXT_FIELD_FONT_SIZE, 11)]
        public static int FontSize;

        [UIElement("BG")]
        private readonly RectTransform m_bg;

        [UIElement("BG", false)]
        private readonly GameObject m_bgObject;

        [UIElement("BG")]
        private readonly CanvasGroup m_bgCanvasGroup;

        [UIElement("BG")]
        private readonly Image m_bgImage;

        [UIElement("Text")]
        private readonly Text m_text;

        private BetterOutline m_textOutline;

        public override bool closeOnEscapeButtonPress => false;

        private StringBuilder m_stringBuilder;

        private float m_expandProgress;

        private bool m_show;

        private int m_siblingIndex;

        protected override void OnInitialized()
        {
            m_stringBuilder = new StringBuilder();

            Destroy(m_text.GetComponent<Outline>());
            BetterOutline betterOutline = m_text.gameObject.AddComponent<BetterOutline>();
            betterOutline.effectColor = Color.black;
            betterOutline.effectDistance = Vector2.one * 1.25f;
            m_textOutline = betterOutline;

            GlobalEventManager.Instance.AddEventListener("SpeechSentenceStarted", onSentenceStarted);
            GlobalEventManager.Instance.AddEventListener("SpeechSequenceFinished", onSentenceFinishedOrCancelled);
            GlobalEventManager.Instance.AddEventListener("SpeechSentenceCancelled", onSentenceFinishedOrCancelled);
            GlobalEventManager.Instance.AddEventListener<string>(ModResources.ASSET_BUNDLE_LOADED_EVENT, onAssetBundleLoaded);

            ModSettingsManager.Instance.AddSettingValueChangedListener(refreshSettings, ModSettingsConstants.SUBTITLE_TEXT_FIELD_BG);
            ModSettingsManager.Instance.AddSettingValueChangedListener(refreshSettings, ModSettingsConstants.SUBTITLE_TEXT_FIELD_UPPER_POSITION);
            ModSettingsManager.Instance.AddSettingValueChangedListener(refreshSettings, ModSettingsConstants.SUBTITLE_TEXT_FIELD_FONT);
            ModSettingsManager.Instance.AddSettingValueChangedListener(refreshSettings, ModSettingsConstants.SUBTITLE_TEXT_FIELD_FONT_SIZE);

            refreshSettings(null);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            GlobalEventManager.Instance.RemoveEventListener("SpeechSentenceStarted", onSentenceStarted);
            GlobalEventManager.Instance.RemoveEventListener("SpeechSequenceFinished", onSentenceFinishedOrCancelled);
            GlobalEventManager.Instance.RemoveEventListener("SpeechSentenceCancelled", onSentenceFinishedOrCancelled);
            GlobalEventManager.Instance.RemoveEventListener<string>(ModResources.ASSET_BUNDLE_LOADED_EVENT, onAssetBundleLoaded);

            ModSettingsManager.Instance.RemoveSettingValueChangedListener(refreshSettings, ModSettingsConstants.SUBTITLE_TEXT_FIELD_BG);
            ModSettingsManager.Instance.RemoveSettingValueChangedListener(refreshSettings, ModSettingsConstants.SUBTITLE_TEXT_FIELD_UPPER_POSITION);
            ModSettingsManager.Instance.RemoveSettingValueChangedListener(refreshSettings, ModSettingsConstants.SUBTITLE_TEXT_FIELD_FONT);
            ModSettingsManager.Instance.RemoveSettingValueChangedListener(refreshSettings, ModSettingsConstants.SUBTITLE_TEXT_FIELD_FONT_SIZE);
        }

        public override void Update()
        {
            Text textComponent = m_text;

            RectTransform rt = m_bg;
            Vector2 sd = rt.sizeDelta;
            sd.x = Mathf.Lerp(0f, Mathf.Min(textComponent.preferredWidth + 15f, 400f), NumberUtils.EaseOutQuad(0f, 1f, m_expandProgress));
            sd.y = Mathf.Lerp(0f, textComponent.preferredHeight + 12.5f, NumberUtils.EaseOutQuad(0f, 1f, m_expandProgress));
            rt.sizeDelta = sd;

            m_bgObject.SetActive(m_expandProgress > 0f);
            if (!m_show && m_expandProgress == 0f)
            {
                if (!textComponent.text.IsNullOrEmpty())
                    textComponent.text = null;
            }

            m_expandProgress = Mathf.Clamp01(m_expandProgress + ((m_show ? 1f : -1f) * Time.unscaledDeltaTime * 5f));
        }

        private void onAssetBundleLoaded(string assetBundle)
        {
            if (assetBundle == AssetBundleConstants.UI_EXTRA)
                refreshSettings(null);
        }

        private void onSentenceStarted()
        {
            if (!EnableRework || !SettingsManager.Instance.ShouldShowSubtitles())
                return;

            if (BeOnTop)
            {
                float y;
                if (ModCache.gameUIRoot.Multiplayer1v1UI.PlayerStatsPanel.gameObject.activeInHierarchy)
                    y = -40f;
                else if (ModCache.gameUIRoot.BattleRoyaleUI.WaitingRoomLabel.gameObject.activeInHierarchy || ModCache.gameUIRoot.CurrentlySpectatingUI.gameObject.activeInHierarchy || ModCache.gameUIRoot.CoopUpgradeTimerUI.gameObject.activeInHierarchy)
                    y = -60f;
                else
                    y = -10f;

                RectTransform rectTransform = m_bg;
                Vector2 ap = rectTransform.anchoredPosition;
                ap.y = y;
                rectTransform.anchoredPosition = ap;
            }

            if (FontType == 0)
            {
                m_text.font = LocalizationManager.Instance.GetCurrentSubtitlesFont();
            }

            SpeechAudioManager speechAudioManager = SpeechAudioManager.Instance;
            SpeechSentence currentSentence = speechAudioManager.GetCurrentSentence();
            if (currentSentence != null)
            {
                if (string.IsNullOrWhiteSpace(currentSentence.SpeechText))
                {
                    ShowText("!Not localized speech sentence!", Color.red);
                }
                else
                {
                    _ = m_stringBuilder.Clear();
                    if (ModCore.ShowSpeakerName)
                    {
                        string speakerName = ModGameUtils.GetSpeakerNameText(currentSentence.SpeakerName);
                        if (ModCore.SwapSubtitlesColor)
                        {
                            speakerName = speakerName.AddColor(speechAudioManager.GetSubtitleColorForSpeaker(currentSentence.SpeakerName));
                        }
                        else
                        {
                            speakerName = speakerName.AddColor(Color.white);
                        }

                        _ = m_stringBuilder.Append(speakerName);
                        _ = m_stringBuilder.Append(' ');
                    }
                    _ = m_stringBuilder.Append(currentSentence.SpeechText);
                    ShowText(m_stringBuilder.ToString(), ModCore.SwapSubtitlesColor ? Color.white : speechAudioManager.GetSubtitleColorForSpeaker(currentSentence.SpeakerName));
                }
            }
        }

        private void onSentenceFinishedOrCancelled()
        {
            HideText();
        }

        private void refreshSettings(object obj)
        {
            m_textOutline.enabled = !EnableBG;
            m_bgImage.enabled = EnableBG;

            RectTransform rectTransform = m_bg;
            rectTransform.anchorMax = new Vector2(0.5f, BeOnTop ? 1f : 0f);
            rectTransform.anchorMin = rectTransform.anchorMax;
            rectTransform.pivot = new Vector2(0.5f, BeOnTop ? 1f : 0f);
            rectTransform.anchoredPosition = new Vector2(0f, BeOnTop ? -10f : 55f);

            m_text.fontSize = FontSize;
            m_text.font = ModResources.FontByIndex(FontType);
        }

        public void ShowText(string text, Color color)
        {
            m_text.color = color;
            m_text.text = text;
            m_expandProgress = 0f;
            m_show = true;
        }

        public void HideText()
        {
            m_show = false;
        }

        public void SetSiblingIndex(bool last)
        {
            if (last)
            {
                m_siblingIndex = base.transform.GetSiblingIndex();
                base.transform.SetAsLastSibling();
            }
            else if (m_siblingIndex != 0)
            {
                base.transform.SetSiblingIndex(m_siblingIndex);
            }
        }
    }
}
