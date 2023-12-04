using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIElementSettingsMenuSettingInfoDisplay : OverhaulBehaviour
    {
        [UIElementReference("Name")]
        private readonly Text m_NameLabel;

        [UIElementReference("Description")]
        private readonly Text m_DescriptionLabel;

        [UIElementReference("Outline")]
        private readonly Graphic m_Outline;

        [UIElementActionReference(nameof(OnResetValueButtonClicked))]
        [UIElementReference("ResetValueButton")]
        private readonly Button m_ResetValueButton;

        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("Dropdown")]
        private readonly UIElementDropdown m_Dropdown;

        [UIElementDefaultVisibilityState(false)]
        [UIElementComponents(new System.Type[] { typeof(UIElementSettingsMenuToggle) })]
        [UIElementReference("Toggle")]
        private readonly UIElementSettingsMenuToggle m_Toggle;

        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("Slider")]
        private readonly Slider m_Slider;

        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("InputField")]
        private readonly InputField m_InputField;

        [UIElementDefaultVisibilityState(false)]
        [UIElementComponents(new System.Type[] { typeof(UIElementSettingsMenuKeyBinder) })]
        [UIElementReference("KeyCode")]
        private readonly UIElementSettingsMenuKeyBinder m_KeyCode;

        private UISettingsMenu m_SettingsMenu;

        private bool m_Initialized;

        public OverhaulSettingInfo settingInfo
        {
            get;
            set;
        }

        public override void Awake()
        {
            m_SettingsMenu = OverhaulUIManager.reference?.GetUI<UISettingsMenu>(UIConstants.UI_SETTINGS_MENU);
            if (!m_SettingsMenu)
            {
                OverhaulDebug.Error("SettingInfoDisplay - m_SettingsMenu is NULL! Manager existence: " + OverhaulUIManager.reference, EDebugType.UI);
                base.enabled = false;
                return;
            }

            UIController.AssignVariables(this);
        }

        public override void Start()
        {
            m_NameLabel.text = settingInfo.name;
            m_DescriptionLabel.text = "WORK IN PROGRESS";

            base.gameObject.AddComponent<OverhaulUISelectionOutline>().SetGraphic(m_Outline);
            InitializeDisplay();
        }

        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public void InitializeDisplay()
        {
            if (m_Initialized)
                return;

            EOverhaulSettingType settingType = settingInfo.settingType;
            switch (settingType)
            {
                case EOverhaulSettingType.Bool:
                    m_Toggle.gameObject.SetActive(true);
                    m_Toggle.isOn = settingInfo.GetValue<bool>();
                    m_Toggle.onValueChanged = OnToggleValueChanged;
                    break;
                case EOverhaulSettingType.Int:
                    m_Dropdown.gameObject.SetActive(true);
                    OverhaulSettingDropdownParameters dropdownParameters = settingInfo.GetAttribute<OverhaulSettingDropdownParameters>();
                    if (dropdownParameters != null)
                    {
                        m_Dropdown.options = dropdownParameters.Options;
                        m_Dropdown.value = settingInfo.GetValue<int>();
                        m_Dropdown.onValueChanged = OnDropdownValueChanged;
                        m_Dropdown.targetYSize = dropdownParameters.Height;
                    }
                    break;
                case EOverhaulSettingType.Float:
                    m_Slider.gameObject.SetActive(true);
                    m_Slider.wholeNumbers = false;
                    OverhaulSettingSliderParameters sliderParameters = settingInfo.GetAttribute<OverhaulSettingSliderParameters>();
                    if (sliderParameters != null)
                    {
                        m_Slider.minValue = sliderParameters.Min;
                        m_Slider.maxValue = sliderParameters.Max;
                        m_Slider.value = settingInfo.GetValue<float>();
                        m_Slider.onValueChanged.AddListener(OnSliderValueChanged);
                    }
                    break;
                case EOverhaulSettingType.String:
                    m_InputField.gameObject.SetActive(true);
                    m_InputField.text = settingInfo.GetValue<string>();
                    m_InputField.onValueChanged.AddListener(OnInputFieldValueChanged);
                    break;
                case EOverhaulSettingType.KeyCode:
                    m_KeyCode.gameObject.SetActive(true);
                    m_KeyCode.onValueChanged = OnKeyCodeValueChanged;
                    m_KeyCode.RefreshLabel(settingInfo.GetValue<KeyCode>());
                    break;
            }
            m_Initialized = true;
        }

        public void OnResetValueButtonClicked()
        {
            settingInfo.ResetValue();
            UISettingsMenu settingsMenu = OverhaulUIManager.reference?.GetUI<UISettingsMenu>(UIConstants.UI_SETTINGS_MENU);
            if (settingsMenu)
            {
                settingsMenu.Populate();
            }
        }

        public void OnToggleValueChanged(bool newValue)
        {
            SetValue(newValue);
        }

        public void OnSliderValueChanged(float newValue)
        {
            SetValue(newValue);
        }

        public void OnDropdownValueChanged(int newValue)
        {
            SetValue(newValue);
        }

        public void OnInputFieldValueChanged(string newValue)
        {
            SetValue(newValue);
        }

        public void OnKeyCodeValueChanged(KeyCode newValue)
        {
            SetValue(newValue);
        }

        public void SetValue(object value)
        {
            settingInfo.SetValue(value, true);
        }
    }
}
