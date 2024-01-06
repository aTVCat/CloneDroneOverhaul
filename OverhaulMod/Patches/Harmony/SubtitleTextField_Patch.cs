using HarmonyLib;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(SubtitleTextField))]
    internal static class SubtitleTextField_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("onSpeechSentenceStarted")]
        private static bool onSpeechSentenceStarted_Prefix(SubtitleTextField __instance)
        {
            if (!SettingsManager.Instance.ShouldShowSubtitles())
                return false;

            __instance.refreshYOffset();
            SpeechSentence currentSentence = Singleton<SpeechAudioManager>.Instance.GetCurrentSentence();
            if (currentSentence != null)
            {
                __instance.TextField.color = SpeechAudioManager.Instance.GetSubtitleColorForSpeaker(currentSentence.SpeakerName);
                if (string.IsNullOrWhiteSpace(currentSentence.SpeechText))
                    __instance.TextField.text = "!!!NOT_LOCALIZED!!!";
                else
                {
                    string speakerName = ModGameUtils.GetSpeakerNameText(currentSentence.SpeakerName);
                    __instance.TextField.text = $"{speakerName} {currentSentence.SpeechText}";
                }

                __instance.transform.localScale = Vector3.one * 0.5f;
                iTween.ScaleTo(__instance.gameObject, Vector3.one, 0.5f * Time.timeScale);
            }
            return false;
        }
    }
}
