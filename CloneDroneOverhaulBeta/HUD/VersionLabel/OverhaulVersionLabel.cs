using CDOverhaul.Gameplay;
using CDOverhaul.Workshop;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulVersionLabel : OverhaulUI
    {
        [OverhaulSetting("Game interface.Information.Watermark", true, false, "Show mod version label during gameplay")]
        public static bool EnableWatermarkInGameplay;

        public static OverhaulVersionLabel Instance;

        private Text m_TitleScreenUIVersionLabel;
        private GameObject m_TitleScreenRootButtons;

        private GameObject m_TitleScreenWatermark;
        private Text m_TitleScreenVersionLabel;
        private GameObject m_TitleScreenDebugLabel;

        private GameObject m_GameplayWatermark;
        private Text m_GameplayVersionLabel;
        private GameObject m_GameplayDebugLabel;

        private GameObject m_UpperButtonsContainer;
        private Button m_PatchNotesButton;

        public override void Initialize()
        {
            Instance = this;
            m_TitleScreenWatermark = MyModdedObject.GetObject<Transform>(0).gameObject;
            m_TitleScreenWatermark.SetActive(false);
            m_TitleScreenVersionLabel = MyModdedObject.GetObject<Text>(1);
            m_TitleScreenVersionLabel.text = OverhaulVersion.Watermark;
            m_TitleScreenDebugLabel = MyModdedObject.GetObject<Transform>(5).gameObject;
            m_TitleScreenDebugLabel.SetActive(OverhaulVersion.IsDebugBuild);

            m_GameplayWatermark = MyModdedObject.GetObject<Transform>(2).gameObject;
            m_GameplayWatermark.gameObject.SetActive(false);
            m_GameplayVersionLabel = MyModdedObject.GetObject<Text>(3);
            m_GameplayVersionLabel.text = OverhaulVersion.ShortenedWatermark;
            m_GameplayDebugLabel = MyModdedObject.GetObject<Transform>(4).gameObject;
            m_GameplayDebugLabel.SetActive(OverhaulVersion.IsDebugBuild);

            m_TitleScreenUIVersionLabel = GameUIRoot.Instance.TitleScreenUI.VersionLabel;
            m_TitleScreenUIVersionLabel.gameObject.SetActive(false);
            m_TitleScreenRootButtons = GameUIRoot.Instance.TitleScreenUI.RootButtonsContainerBG;

            m_UpperButtonsContainer = MyModdedObject.GetObject<Transform>(6).gameObject;
            m_PatchNotesButton = MyModdedObject.GetObject<Button>(7);
            m_PatchNotesButton.onClick.AddListener(onPatchNotesButtonClicked);
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();
            Instance = null;
        }

        private void Update()
        {
            if (Time.frameCount % 5 == 0)
                Refresh();
        }

        public void Refresh()
        {
            if (IsDisposedOrDestroyed())
                return;

            bool isOnTitleScreen = GameModeManager.IsOnTitleScreen();
            bool shouldHideOnTitleScreen = OverhaulWorkshopBrowserUI.IsActive;
            bool shouldHideInGameplay = !EnableWatermarkInGameplay;

            m_UpperButtonsContainer.gameObject.SetActive(m_TitleScreenRootButtons.activeInHierarchy);
            m_TitleScreenWatermark.SetActive(isOnTitleScreen && !shouldHideOnTitleScreen);
            m_GameplayWatermark.SetActive(!isOnTitleScreen && !shouldHideInGameplay);
        }

        private void onPatchNotesButtonClicked()
        {
            OverhaulPatchNotesUI overhaulPatchNotesUI = GetController<OverhaulPatchNotesUI>();
            if (!overhaulPatchNotesUI)
            {
                m_PatchNotesButton.gameObject.SetActive(false);
                return;
            }

            overhaulPatchNotesUI.Show();
        }
    }
}