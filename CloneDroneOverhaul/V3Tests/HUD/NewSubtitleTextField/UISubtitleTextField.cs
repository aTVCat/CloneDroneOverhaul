using CloneDroneOverhaul.V3Tests.Gameplay;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace CloneDroneOverhaul.V3Tests.HUD
{
    public class UISubtitleTextField : V3_ModHUDBase
    {
        private GameObject _subtitleGameObject;
        private Text _speakerName;
        private Text _sentence;

        private bool _hasInitialized;
        private bool _isShowingSubtitle;

        private float _timeLeftToCheck;

        private void Start()
        {
            _subtitleGameObject = ModdedObject.GetObjectFromList<RectTransform>(0).gameObject;
            _speakerName = ModdedObject.GetObjectFromList<Text>(1);
            _sentence = ModdedObject.GetObjectFromList<Text>(2);

            Singleton<GlobalEventManager>.Instance.AddEventListener("SpeechSentenceStarted", new Action(this.onSequenceStarted));
            Singleton<GlobalEventManager>.Instance.AddEventListener("SpeechSequenceFinished", new Action(this.onSequenceEnded));
            Singleton<GlobalEventManager>.Instance.AddEventListener("SpeechSentenceCancelled", new Action(this.onSequenceEnded));

            SubtitleTextField textField = FindObjectOfType<SubtitleTextField>();
            if(textField != null)
            {
                textField.gameObject.SetActive(false);
            }

            _hasInitialized = true;
        }

        private void LateUpdate()
        {
            _timeLeftToCheck -= Time.deltaTime;
            if(_timeLeftToCheck <= 0)
            {
                _timeLeftToCheck = 0.1f;
                //_subtitleGameObject.SetActive((!Cursor.visible && Cursor.lockState != CursorLockMode.None && _isShowingSubtitle) || GameFlowManager.Instance.HasPlayerDied() || GameFlowManager.Instance.HasLostRound());
                _subtitleGameObject.SetActive(_isShowingSubtitle);
            }
        }

        private void onSequenceStarted()
        {
            if (!_hasInitialized)
            {
                return;
            }

            SpeechSentence currentSentence = Singleton<SpeechAudioManager>.Instance.GetCurrentSentence();
            if(currentSentence != null)
            {
                ShowSubtitle(currentSentence, SpeechAudioManager.Instance.GetSubtitleColorForSpeaker(currentSentence.SpeakerName));
            }
        }

        private void onSequenceEnded()
        {
            if (!_hasInitialized)
            {
                return;
            }

            _isShowingSubtitle = false;
        }

        public void ShowSubtitle(in SpeechSentence speech, in Color textColor)
        {
            if (!SettingsManager.Instance.ShouldShowSubtitles() || !_hasInitialized)
            {
                return;
            }

            _isShowingSubtitle = true;

            _speakerName.text = speech.SpeakerName.ToString() + ":";
            _sentence.color = textColor;
            if (string.IsNullOrWhiteSpace(speech.SpeechText))
            {
                _sentence.text = "!Not localized!";
            }
            else
            {
                _sentence.text = speech.SpeechText;
            }

            _subtitleGameObject.transform.localScale = Vector3.one * 0.5f;
            iTween.ScaleTo(_subtitleGameObject, Vector3.one, 0.5f * Time.timeScale);
        }
    }
}
