using InternalModBot;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAutoBuildMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnConfigureBRUpgradesButtonClicked))]
        [UIElement("ConfigureBRUpgradesButton")]
        private readonly Button m_configureBRUpgradesButton;

        [UIElementAction(nameof(OnCloseUpgradeUIButtonClicked))]
        [UIElement("CloseUpgradeUIButton", false)]
        private readonly Button m_closeUpgradeUIButton;

        [UIElementAction(nameof(OnAutoActivationToggled))]
        [UIElement("ActivateOnMatchStartToggle")]
        private readonly Toggle m_autoActivationToggle;

        [KeyBindSetter(KeyCode.U)]
        [UIElementAction(nameof(OnKeyBindChanged))]
        [UIElement("KeyBind")]
        private readonly UIElementKeyBindSetter m_keyBind;

        [UIElement("Panel", true)]
        private readonly GameObject m_panel;

        [UIElement("SaveSlotsPanel", false)]
        private readonly GameObject m_saveSlotsPanel;

        private int m_upgradeUISiblingIndex;

        private Font m_originalFont;

        public override bool refreshOnlyCursor => true;

        public bool isShowingUpgradeUI
        {
            get;
            set;
        }

        public GameObject objectToShow
        {
            get;
            set;
        }

        public override void Show()
        {
            base.Show();
            m_keyBind.key = AutoBuildManager.AutoBuildKeyBind;
            m_autoActivationToggle.isOn = ModSettingsManager.GetBoolValue(ModSettingsConstants.AUTO_BUILD_ACTIVATION_ON_MATCH_START);
        }

        public override void OnEnable()
        {
            SetUpgradeUISiblingIndex(false);
        }

        public override void OnDisable()
        {
            AutoBuildManager autoBuildManager = AutoBuildManager.Instance;
            autoBuildManager.isInAutoBuildConfigurationMode = false;
            autoBuildManager.ResetUpgrades();

            GameObject gameObject = objectToShow;
            if (gameObject)
                gameObject.SetActive(true);

            objectToShow = null;
            SetUpgradeUISiblingIndex(true);

            ModSettingsDataManager.Instance.Save();
        }

        public void SetUpgradeUISiblingIndex(bool initial)
        {
            if (initial)
            {
                ModCache.gameUIRoot.UpgradeUI.transform.SetSiblingIndex(m_upgradeUISiblingIndex);
            }
            else
            {
                Transform transform = ModCache.gameUIRoot.UpgradeUI.transform;
                m_upgradeUISiblingIndex = transform.GetSiblingIndex();
                transform.SetSiblingIndex(ModUIManager.Instance.GetSiblingIndex(ModUIManager.UILayer.AfterTitleScreen) + 1);
            }
        }

        public void OnConfigureBRUpgradesButtonClicked()
        {
            AutoBuildManager autoBuildManager = AutoBuildManager.Instance;
            autoBuildManager.isInAutoBuildConfigurationMode = true;
            autoBuildManager.ResetUpgrades(autoBuildManager.buildList.Builds[0].GetUpgradesFromData(), autoBuildManager.buildList.Builds[0].SkillPoints);

            UpgradePagesManager._currentPageIndex = 0;
            UpgradeUI upgradeUI = ModCache.gameUIRoot.UpgradeUI;
            upgradeUI.Show(false, true, false);
            upgradeUI.ExitButton.SetActive(false);
            upgradeUI.StoryBackButton.SetActive(false);
            upgradeUI.StoryConfirmButtonLabel.transform.parent.parent.gameObject.SetActive(false);
            upgradeUI.StoryModeHumanLabel.text = LocalizationManager.Instance.GetTranslatedString("select_upgrades...");
            if (!m_originalFont)
                m_originalFont = upgradeUI.StoryModeHumanLabel.font;
            upgradeUI.StoryModeHumanLabel.font = LocalizationManager.Instance.GetFontForCurrentLanguage();
            upgradeUI.StoryModeHumanLabel.color = new Color(1f, 0f, 0.5255f, 1f);

            isShowingUpgradeUI = true;

            m_panel.SetActive(false);
            m_closeUpgradeUIButton.gameObject.SetActive(true);
            m_saveSlotsPanel.SetActive(true);
        }

        public void OnCloseUpgradeUIButtonClicked()
        {
            AutoBuildManager autoBuildManager = AutoBuildManager.Instance;
            autoBuildManager.isInAutoBuildConfigurationMode = false;
            autoBuildManager.buildList.Builds[0].SetUpgradesFromData(GameDataManager.Instance.GetAvailableSkillPoints());
            autoBuildManager.SaveBuildInfo();

            UpgradeUI upgradeUI = ModCache.gameUIRoot.UpgradeUI;
            upgradeUI.Hide();
            if (m_originalFont)
                upgradeUI.StoryModeHumanLabel.font = m_originalFont;

            isShowingUpgradeUI = false;

            m_panel.SetActive(true);
            m_closeUpgradeUIButton.gameObject.SetActive(false);
            m_saveSlotsPanel.SetActive(false);
        }

        public void OnAutoActivationToggled(bool value)
        {
            ModSettingsManager.SetBoolValue(ModSettingsConstants.AUTO_BUILD_ACTIVATION_ON_MATCH_START, value, true);
        }

        public void OnKeyBindChanged(KeyCode keyCode)
        {
            ModSettingsManager.SetIntValue(ModSettingsConstants.AUTO_BUILD_KEY_BIND, (int)keyCode, true);
        }
    }
}
