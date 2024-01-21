using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class ModBotTagRemoverBehaviour : OverhaulBehaviour
    {
        public string NormalUsername;
        public Text TextComponent;

        private bool m_ForceRemoveTags;

        public override void OnEnable()
        {
            m_ForceRemoveTags = true;
        }

        private void LateUpdate()
        {
            if (Time.frameCount % 30 != 0 && !m_ForceRemoveTags)
                return;

            m_ForceRemoveTags = true;

            if (!TextComponent)
            {
                TextComponent = base.GetComponent<Text>();
                if (!TextComponent)
                {
                    base.enabled = false;
                    return;
                }
            }

            if (TextComponent.text != NormalUsername)
            {
                replaceName();
            }
        }

        private void replaceName()
        {
            if (!ModBotTagDisabler.DisableTags)
                return;

            if (TextComponent)
            {
                TextComponent.text = NormalUsername;
            }
        }
    }
}
