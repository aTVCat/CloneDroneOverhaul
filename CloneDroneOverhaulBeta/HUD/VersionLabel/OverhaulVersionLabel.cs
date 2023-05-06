using CDOverhaul.Workshop;
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

        private Transform m_UpperButtonsContainer;
        private Button m_PatchNotesButton;

        private GameObject m_TitleScreenRootButtons;

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
            m_TitleScreenRootButtons = GameUIRoot.Instance.TitleScreenUI.RootButtonsContainerBG;

            m_UpperButtonsContainer = MyModdedObject.GetObject<Transform>(5);
            m_PatchNotesButton = MyModdedObject.GetObject<Button>(6);
            m_PatchNotesButton.onClick.AddListener(onPatchNotesButtonClicked);

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
                if (m_TitleScreenRootButtons != null) m_UpperButtonsContainer.gameObject.SetActive(m_TitleScreenRootButtons.activeInHierarchy);
                if (!OverhaulWorkshopBrowserUI.BrowserIsNull && OverhaulWorkshopBrowserUI.BrowserUIInstance.gameObject.activeSelf)
                {
                    m_VersionLabel.text = string.Empty;
                    return;
                }
                m_VersionLabel.text = string.Concat(m_TitleScreenUIVersionLabel.text,
               "\n",
                OverhaulVersion.ModFullName);
            }
            else
            {
                m_VersionLabel.text = OverhaulVersion.ModShortName;
                m_VersionLabel.gameObject.SetActive(WatermarkEnabled);
                m_DiscordHolderTransform.gameObject.SetActive(false);
                m_UpperButtonsContainer.gameObject.SetActive(false);
            }
        }

        public void RefreshDiscordUserInfo()
        {
            m_DiscordHolderTransform.gameObject.SetActive(false);

            bool discordInitialized = !OverhaulVersion.Upd2Hotfix && (m_ParametersMenu == null || !m_ParametersMenu.gameObject.activeSelf) && GameModeManager.IsOnTitleScreen() && OverhaulDiscordController.SuccessfulInitialization && OverhaulDiscordController.Instance.UserID != -1;
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

        private void onPatchNotesButtonClicked()
        {
            OverhaulPatchNotesUI overhaulPatchNotesUI = GetController<OverhaulPatchNotesUI>();
            if(overhaulPatchNotesUI == null)
            {
                m_PatchNotesButton.interactable = false;
                return;
            }

            overhaulPatchNotesUI.Show();
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (Time.frameCount % 5 == 0)
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