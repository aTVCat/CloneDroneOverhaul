using InternalModBot;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UITitleScreenRework : OverhaulUIBehaviour
    {
        [UIElement("ButtonsBG")]
        private readonly GameObject m_container;

        [UIElementAction(nameof(OnPlaySinglePlayerButtonClicked))]
        [UIElement("PlaySingleplayerButton")]
        private readonly Button m_playSinglePlayerButton;

        [UIElementAction(nameof(OnPlayMultiPlayerButtonClicked))]
        [UIElement("PlayMultiplayerButton")]
        private readonly Button m_playMultiPlayerButton;

        [UIElementAction(nameof(OnModsButtonClicked))]
        [UIElement("ModsButton")]
        private readonly Button m_modsButton;

        [UIElementAction(nameof(OnOptionsButtonClicked))]
        [UIElement("OptionsButton")]
        private readonly Button m_optionsButton;

        [UIElementAction(nameof(OnAdvancementsButtonClicked))]
        [UIElement("AchievementsButton")]
        private readonly Button m_advancementsButton;

        [UIElementAction(nameof(OnExitButtonClicked))]
        [UIElement("ExitButton")]
        private readonly Button m_quitButton;

        private TitleScreenUI m_titleScreenUI;
        private CanvasGroup m_canvasGroup;
        private GameObject m_legacyContainer;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            TitleScreenUI titleScreenUI = ModCache.titleScreenUI;
            if (titleScreenUI)
            {
                CanvasGroup group = titleScreenUI.RootButtonsContainerBG.AddComponent<CanvasGroup>();
                group.alpha = 0f;
                group.interactable = false;
                m_canvasGroup = group;
                m_legacyContainer = group.gameObject;
                m_titleScreenUI = titleScreenUI;
            }
        }

        public override void Update()
        {
            m_container.SetActive(m_legacyContainer.activeInHierarchy);
        }

        public void OnPlaySinglePlayerButtonClicked()
        {
            m_titleScreenUI.OnPlaySingleplayerButtonClicked();
        }

        public void OnPlayMultiPlayerButtonClicked()
        {
            m_titleScreenUI.OnMultiplayerButtonClicked();
        }

        public void OnModsButtonClicked()
        {
            ModsPanelManager.Instance.openModsMenu();
        }

        public void OnAdvancementsButtonClicked()
        {
            UIConstants.ShowAdvancementsMenuRework();
        }

        public void OnOptionsButtonClicked()
        {
            UIConstants.ShowSettingsMenuRework();
        }

        public void OnExitButtonClicked()
        {
            Application.Quit();
        }
    }
}
