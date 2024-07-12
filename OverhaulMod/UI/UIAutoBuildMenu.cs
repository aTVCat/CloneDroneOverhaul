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

        private int m_upgradeUISiblingIndex;

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
            autoBuildManager.ResetUpgrades(autoBuildManager.buildInfo.GetUpgradesFromData(), autoBuildManager.buildInfo.SkillPoints);

            UpgradeUI upgradeUI = ModCache.gameUIRoot.UpgradeUI;
            upgradeUI.Show(false, true, false);
            upgradeUI.ExitButton.SetActive(false);
            upgradeUI.StoryBackButton.SetActive(false);
            upgradeUI.StoryConfirmButtonLabel.transform.parent.parent.gameObject.SetActive(false);
            upgradeUI.StoryModeHumanLabel.text = "Select upgrades...";

            isShowingUpgradeUI = true;

            m_panel.SetActive(false);
            m_closeUpgradeUIButton.gameObject.SetActive(true);
        }

        public void OnCloseUpgradeUIButtonClicked()
        {
            AutoBuildManager autoBuildManager = AutoBuildManager.Instance;
            autoBuildManager.isInAutoBuildConfigurationMode = false;
            autoBuildManager.buildInfo.SetUpgradesFromData(GameDataManager.Instance.GetAvailableSkillPoints());
            autoBuildManager.SaveBuildInfo();

            ModCache.gameUIRoot.UpgradeUI.Hide();

            isShowingUpgradeUI = false;

            m_panel.SetActive(true);
            m_closeUpgradeUIButton.gameObject.SetActive(false);
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
