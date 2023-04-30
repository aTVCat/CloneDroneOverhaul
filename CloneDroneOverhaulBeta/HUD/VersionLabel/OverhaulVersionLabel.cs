using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulVersionLabel : OverhaulUI
    {
        [OverhaulSetting("Game interface.Information.Watermark", true, false, "Show mod version label during gameplay")]
        public static bool WatermarkEnabled;

        private OverhaulParametersMenu m_ParametersMenu;

        private Text m_VersionLabel;
        private Text m_TitleScreenUIVersionLabel;

        private Transform m_DiscordHolderTransform;
        private Text m_DiscordUserLabel;
        private Button m_DiscordDisableMessageButton;

        private bool m_wasOnTitleScreenBefore;

        public override void Initialize()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if(GameUIRoot.Instance == null || GameUIRoot.Instance.TitleScreenUI == null || GameUIRoot.Instance.TitleScreenUI.VersionLabel == null)
            {
                base.enabled = false;
                return;
            }

            m_DiscordHolderTransform = MyModdedObject.GetObject<Transform>(1);
            m_DiscordUserLabel = MyModdedObject.GetObject<Text>(2);

            m_VersionLabel = MyModdedObject.GetObject<Text>(0);
            m_TitleScreenUIVersionLabel = GameUIRoot.Instance.TitleScreenUI.VersionLabel;
            m_TitleScreenUIVersionLabel.gameObject.SetActive(false);

            _ = OverhaulEventsController.AddEventListener(SettingsController.SettingChangedEventString, refreshVisibility);

            DelegateScheduler.Instance.Schedule(delegate
            {
                m_ParametersMenu = GetController<OverhaulParametersMenu>();
            }, 0.1f);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            m_VersionLabel = null;
            m_TitleScreenUIVersionLabel = null;
            m_DiscordHolderTransform = null;
            m_DiscordUserLabel = null;
            m_DiscordDisableMessageButton = null;

            OverhaulEventsController.RemoveEventListener(SettingsController.SettingChangedEventString, refreshVisibility);
        }

        public void RefreshVersionLabel()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (GameModeManager.IsOnTitleScreen())
            {
                m_VersionLabel.text = string.Concat(m_TitleScreenUIVersionLabel.text,
               "\n",
                OverhaulVersion.ModFullName);
            }
            else
            {
                m_VersionLabel.text = OverhaulVersion.ModShortName;
                m_VersionLabel.gameObject.SetActive(WatermarkEnabled);
                m_DiscordHolderTransform.gameObject.SetActive(false);
            }
        }

        public void RefreshDiscordUserInfo()
        {
            m_DiscordHolderTransform.gameObject.SetActive(false);

            bool discordInitialized = (m_ParametersMenu == null || !m_ParametersMenu.gameObject.activeSelf) && GameModeManager.IsOnTitleScreen() && OverhaulDiscordController.SuccessfulInitialization && OverhaulDiscordController.Instance.UserID != -1;
            if (!discordInitialized)
            {
                return;
            }

            m_DiscordHolderTransform.gameObject.SetActive(true);
            m_DiscordUserLabel.text = OverhaulDiscordController.Instance.UserName;
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
                if (isOnTitleScreen || isOnTitleScreen != m_wasOnTitleScreenBefore)
                {
                    RefreshVersionLabel();
                }
                m_wasOnTitleScreenBefore = isOnTitleScreen;

                RefreshDiscordUserInfo();
            }
        }
    }
}