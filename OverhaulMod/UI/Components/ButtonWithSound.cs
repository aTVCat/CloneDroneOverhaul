using OverhaulMod.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            switch (Sound)
            {
                case SoundType.Choose:
                    AudioManager.Instance.PlayClipGlobal(ModAudioLibrary.Instance.HyperdomeUIChoose);
                    break;
                case SoundType.Choose_NoEcho:
                    AudioManager.Instance.PlayClipGlobal(ModAudioLibrary.Instance.HyperdomeUIChoose_NoEcho);
                    break;
                case SoundType.Back:
                    AudioManager.Instance.PlayClipGlobal(ModAudioLibrary.Instance.HyperdomeUIBack);
                    break;
                case SoundType.Back_NoEcho:
                    AudioManager.Instance.PlayClipGlobal(ModAudioLibrary.Instance.HyperdomeUIChoose_NoEcho);
                    break;
                case SoundType.Click:
                    AudioManager.Instance.PlayClipGlobal(ModAudioLibrary.Instance.HyperdomeUIClick);
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
