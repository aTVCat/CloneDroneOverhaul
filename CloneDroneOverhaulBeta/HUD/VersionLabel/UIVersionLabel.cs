using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIVersionLabel : OverhaulUI
    {
        private Text m_VersionLabel;
        private Text m_TitleScreenUIVersionLabel;

        public override void Initialize()
        {
            m_VersionLabel = MyModdedObject.GetObject<Text>(2);
            m_TitleScreenUIVersionLabel = GameUIRoot.Instance.TitleScreenUI.VersionLabel;
            m_TitleScreenUIVersionLabel.gameObject.SetActive(false);

            if(m_VersionLabel == null || m_TitleScreenUIVersionLabel == null)
            {
                throw new System.NullReferenceException("Overhaul version label: m_VersionLabel or m_TitleScreenUIVersionLabel is null");
            }
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            m_VersionLabel = null;
            m_TitleScreenUIVersionLabel = null;
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

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

        public override void OnModDeactivated()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (m_TitleScreenUIVersionLabel != null)
            {
                m_TitleScreenUIVersionLabel.gameObject.SetActive(true);
            }
        }
    }
}