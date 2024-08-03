using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UISubtitleTextFieldRework : OverhaulUIBehaviour
    {
        [UIElement("BG")]
        private readonly RectTransform m_bg;

        [UIElement("BG", false)]
        private readonly GameObject m_bgObject;

        [UIElement("BG")]
        private readonly CanvasGroup m_bgCanvasGroup;

        [UIElement("Text")]
        private readonly Text m_text;

        public override bool closeOnEscapeButtonPress => false;

        private float m_expandProgress;

        private bool m_show;

        protected override void OnInitialized()
        {
            GlobalEventManager.Instance.AddEventListener("SpeechSentenceStarted", onSentenceStarted);
            GlobalEventManager.Instance.AddEventListener("SpeechSequenceFinished", onSentenceFinishedOrCancelled);
            GlobalEventManager.Instance.AddEventListener("SpeechSentenceCancelled", onSentenceFinishedOrCancelled);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            GlobalEventManager.Instance.RemoveEventListener("SpeechSentenceStarted", onSentenceStarted);
            GlobalEventManager.Instance.RemoveEventListener("SpeechSequenceFinished", onSentenceFinishedOrCancelled);
            GlobalEventManager.Instance.RemoveEventListener("SpeechSentenceCancelled", onSentenceFinishedOrCancelled);
        }

        public override void Update()
        {
            Text textComponent = m_text;

            RectTransform rt = m_bg;
            Vector2 sd = rt.sizeDelta;
            sd.x = Mathf.Lerp(0f, Mathf.Min(textComponent.preferredWidth + 10f, 400f), NumberUtils.EaseOutQuad(0f, 1f, m_expandProgress));
            sd.y = Mathf.Lerp(0f, textComponent.preferredHeight + 10f, NumberUtils.EaseOutQuad(0f, 1f, m_expandProgress));
            rt.sizeDelta = sd;

            m_bgObject.SetActive(m_expandProgress > 0f);
            if (!m_show && m_expandProgress == 0f)
            {
                if (!textComponent.text.IsNullOrEmpty())
                    textComponent.text = null;
            }

            m_expandProgress = Mathf.Clamp01(m_expandProgress + ((m_show ? 1f : -1f) * Time.unscaledDeltaTime * 5f));
        }

        private void onSentenceStarted()
        {
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
                    ShowText($"{ModGameUtils.GetSpeakerNameText(currentSentence.SpeakerName)} {currentSentence.SpeechText}", speechAudioManager.GetSubtitleColorForSpeaker(currentSentence.SpeakerName));
                }
            }
        }

        private void onSentenceFinishedOrCancelled()
        {
            HideText();
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
    }
}
