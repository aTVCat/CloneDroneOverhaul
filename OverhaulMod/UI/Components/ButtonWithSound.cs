using OverhaulMod.Engine;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class ButtonWithSound : MonoBehaviour
    {
        public SoundType Sound;

        private void Start()
        {
            Button button = base.GetComponent<Button>();
            if (button)
            {
                button.onClick.AddListener(onClicked);
            }
        }

        private void onClicked()
        {
            if (!ModAudioManager.UISounds) return;

            switch (Sound)
            {
                case SoundType.Choose:
                    ModAudioManager.Instance.PlayOneShotGlobal(ModAudioLibrary.Instance.HyperdomeUIChoose);
                    break;
                case SoundType.Choose_NoEcho:
                    ModAudioManager.Instance.PlayOneShotGlobal(ModAudioLibrary.Instance.HyperdomeUIChoose_NoEcho);
                    break;
                case SoundType.Back:
                    ModAudioManager.Instance.PlayOneShotGlobal(ModAudioLibrary.Instance.HyperdomeUIBack);
                    break;
                case SoundType.Back_NoEcho:
                    ModAudioManager.Instance.PlayOneShotGlobal(ModAudioLibrary.Instance.HyperdomeUIChoose_NoEcho);
                    break;
                case SoundType.Click:
                    ModAudioManager.Instance.PlayOneShotGlobal(ModAudioLibrary.Instance.HyperdomeUIClick);
                    break;
            }
        }

        public enum SoundType
        {
            None,

            Choose,
            Choose_NoEcho,

            Back,
            Back_NoEcho,

            Click
        }
    }
}
