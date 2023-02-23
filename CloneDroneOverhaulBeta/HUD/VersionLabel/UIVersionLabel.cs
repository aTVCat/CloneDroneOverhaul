using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIVersionLabel : UIBase
    {
        private Text m_VersionLabel;
        private TextMeshProUGUI m_VersionLabelTextMeshPro;
        private Text m_TitleScreenUIVersionLabel;

        public override void Initialize()
        {
            m_VersionLabel = MyModdedObject.GetObject<Text>(2);
            m_VersionLabelTextMeshPro = MyModdedObject.GetObject<TextMeshProUGUI>(1);
            m_VersionLabelTextMeshPro.gameObject.SetActive(false);
            m_TitleScreenUIVersionLabel = GameUIRoot.Instance.TitleScreenUI.VersionLabel;
            m_TitleScreenUIVersionLabel.gameObject.SetActive(false);

            HasAddedEventListeners = true;
            HasInitialized = true;
        }

        private void Update()
        {
            if (Time.frameCount % 30 == 0)
            {
                if (GameModeManager.IsOnTitleScreen())
                {
                    m_VersionLabel.text = string.Concat(m_TitleScreenUIVersionLabel.text,
                   "\n",
                    OverhaulVersion.ModFullName);
                }
                else
                {
                    m_VersionLabel.text = OverhaulVersion.ModShortName;
                }
            }
        }
    }
}