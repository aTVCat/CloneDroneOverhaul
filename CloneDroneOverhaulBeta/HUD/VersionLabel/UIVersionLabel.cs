using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIVersionLabel : OverhaulUI
    {
        [OverhaulSetting("Game interface.Information.Watermark", true, false, "Hide mod version label during gameplay")]
        public static bool WatermarkEnabled;

        private Text m_VersionLabel;
        private Text m_ExclusiveLabel;
        private Text m_TitleScreenUIVersionLabel;

        private bool m_wasOnTitleScreenBefore;

        public override void Initialize()
        {
            m_VersionLabel = MyModdedObject.GetObject<Text>(2);
            m_TitleScreenUIVersionLabel = GameUIRoot.Instance.TitleScreenUI.VersionLabel;
            m_TitleScreenUIVersionLabel.gameObject.SetActive(false);
            m_ExclusiveLabel = MyModdedObject.GetObject<Text>(3);
            m_ExclusiveLabel.gameObject.SetActive(false);

            _ = OverhaulEventManager.AddEventListener(ExclusivityController.OnLoginSuccessEventString, onLoginSuccess);
            _ = OverhaulEventManager.AddEventListener(SettingsController.SettingChangedEventString, refreshVisibility);
            m_wasOnTitleScreenBefore = true;

            if (m_VersionLabel == null || m_TitleScreenUIVersionLabel == null)
            {
                throw new System.NullReferenceException("Overhaul version label: m_VersionLabel or m_TitleScreenUIVersionLabel is null");
            }
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            m_VersionLabel = null;
            m_TitleScreenUIVersionLabel = null;
            m_ExclusiveLabel = null;
        }

        private void onLoginSuccess()
        {
            if(ExclusiveRolesController.GetExclusivePlayerInfo(ExclusivityController.GetLocalPlayfabID(), out ExclusivePlayerInfo? info))
            {
                m_ExclusiveLabel.gameObject.SetActive(GameModeManager.IsOnTitleScreen());
                m_ExclusiveLabel.text = "You have exclusive access to some stuff, " + info.Value.Name + " Yay!";
                m_ExclusiveLabel.color = info.Value.FavColor;
            }
        }

        private void refreshVisibility()
        {
            m_wasOnTitleScreenBefore = !m_wasOnTitleScreenBefore;
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (Time.frameCount % 30 == 0)
            {
                bool isOnTitleScreen = GameModeManager.IsOnTitleScreen();
                if(isOnTitleScreen != m_wasOnTitleScreenBefore)
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
                        m_ExclusiveLabel.gameObject.SetActive(false);
                        m_VersionLabel.gameObject.SetActive(WatermarkEnabled);
                    }
                }
                m_wasOnTitleScreenBefore = isOnTitleScreen;
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