﻿using HarmonyLib;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(SubtitleTextField))]
    internal static class SubtitleTextField_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SubtitleTextField.onSpeechSentenceStarted))]
        private static bool onSpeechSentenceStarted_Prefix(SubtitleTextField __instance)
        {
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.SubtitleTextFieldRework) && UISubtitleTextFieldRework.EnableRework)
                return false;

            if (!ModCore.ShowSpeakerName)
                return true;

            if (!SettingsManager.Instance.ShouldShowSubtitles())
                return false;

            __instance.refreshYOffset();
            SpeechSentence currentSentence = SpeechAudioManager.Instance.GetCurrentSentence();
            if (currentSentence != null)
            {
                __instance.TextField.color = ModCore.SwapSubtitlesColor ? Color.white : SpeechAudioManager.Instance.GetSubtitleColorForSpeaker(currentSentence.SpeakerName);
                if (currentSentence.SpeechText.IsNullOrWhiteSpace())
                    __instance.TextField.text = "!!!NOT_LOCALIZED!!!";
                else
                {
                    string speakerName = ModGameUtils.GetSpeakerNameText(currentSentence.SpeakerName);
                    if (ModCore.SwapSubtitlesColor)
                    {
                        speakerName = speakerName.AddColor(SpeechAudioManager.Instance.GetSubtitleColorForSpeaker(currentSentence.SpeakerName));
                    }
                    else
                    {
                        speakerName = speakerName.AddColor(Color.white);
                    }

                    __instance.TextField.text = $"{speakerName} {(ModCore.SwapSubtitlesColor ? currentSentence.SpeechText.AddColor(Color.white) : currentSentence.SpeechText)}";
                }

                __instance.transform.localScale = Vector3.one * 0.5f;
                iTween.ScaleTo(__instance.gameObject, Vector3.one, 0.5f * Time.timeScale);
            }
            return false;
        }
    }
}
