using InternalModBot;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UITitleScreenRework : OverhaulUIBehaviour
    {
        [UIElement("ButtonsBG")]
        private readonly GameObject m_Container;

        [UIElementAction(nameof(OnPlaySinglePlayerButtonClicked))]
        [UIElement("PlaySingleplayerButton")]
        private readonly Button m_PlaySinglePlayerButton;

        [UIElementAction(nameof(OnPlayMultiPlayerButtonClicked))]
        [UIElement("PlayMultiplayerButton")]
        private readonly Button m_PlayMultiPlayerButton;

        [UIElementAction(nameof(OnModsButtonClicked))]
        [UIElement("ModsButton")]
        private readonly Button m_ModsButton;

        [UIElementAction(nameof(OnOptionsButtonClicked))]
        [UIElement("OptionsButton")]
        private readonly Button m_OptionsButton;

        private TitleScreenUI m_TitleScreenUI;
        private CanvasGroup m_CanvasGroup;
        private GameObject m_LegacyContainer;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            TitleScreenUI titleScreenUI = GameUIRoot.Instance?.TitleScreenUI;
            if (titleScreenUI)
            {
                CanvasGroup group = titleScreenUI.RootButtonsContainerBG.AddComponent<CanvasGroup>();
                group.alpha = 0f;
                group.interactable = false;
                m_CanvasGroup = group;
                m_LegacyContainer = group.gameObject;
                m_TitleScreenUI = titleScreenUI;
            }
        }

        public override void Update()
        {
            m_Container.SetActive(m_LegacyContainer.activeInHierarchy);
        }

        public void OnPlaySinglePlayerButtonClicked()
        {
            m_TitleScreenUI.OnPlaySingleplayerButtonClicked();
        }

        public void OnPlayMultiPlayerButtonClicked()
        {
            m_TitleScreenUI.OnMultiplayerButtonClicked();
        }

        public void OnModsButtonClicked()
        {
            ModsPanelManager.Instance.openModsMenu();
        }

        public void OnOptionsButtonClicked()
        {
            UIConstants.ShowSettingsMenuRework();
        }
    }
}
