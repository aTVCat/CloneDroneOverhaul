using CDOverhaul.HUD;
using CDOverhaul.Patches;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class UIElementTitleScreenCustomizationPanel : OverhaulBehaviour, IInitializable
    {
        [UIElementActionReference(nameof(OnLevelPathFieldEdited))]
        [UIElementReference(0)]
        private readonly InputField m_LevelFileInputField;

        [UIElementActionReference(nameof(OnWorkshopLevelIDFieldEdited))]
        [UIElementReference(4)]
        private readonly InputField m_WorkshopLevelIDInputField;

        [UIElementActionReference(nameof(OnReloadButtonClicked))]
        [UIElementReference(1)]
        private readonly Button m_ReloadButton;

        [UIElementActionReference(nameof(OnUIAlignmentFieldEdited))]
        [UIElementReference("UIAlignmentDropdown")]
        private readonly UIElementDropdown m_Dropdown;

        private OverhaulUIAnchoredPanelSlider m_Slider;

        public bool opened
        {
            get;
            private set;
        }

        public void Initialize()
        {
            UIController.AssignVariables(this);
            m_LevelFileInputField.text = TitleScreenCustomizationSystem.OverrideLevelPath;
            m_Dropdown.options = new List<Dropdown.OptionData>()
            {
                new Dropdown.OptionData() { text = "Left" },
                new Dropdown.OptionData() { text = "Center" }
            };
            m_Dropdown.value = TitleScreenCustomizationSystem.UIAlignment;

            m_Slider = base.gameObject.AddComponent<OverhaulUIAnchoredPanelSlider>();
            m_Slider.StartPosition = new Vector2(280f, 0f);
            (base.transform as RectTransform).anchoredPosition = m_Slider.StartPosition;
            SetOpened(false);
        }

        public void SetOpened(bool value)
        {
            opened = value;
            m_Slider.TargetPosition = new Vector2(value ? 0f : 280f, 0f);
        }

        public void OnReloadButtonClicked()
        {
            TitleScreenOverhaulManager.reference.customization.SpawnLevel(out _);
        }

        public void OnLevelPathFieldEdited(string newValue)
        {
            newValue = newValue.Replace("\"", string.Empty);
            m_LevelFileInputField.text = newValue;

            TitleScreenCustomizationSystem.OverrideLevelPath = newValue;
            OverhaulSettingsManager.reference.SaveFieldValueOfClass(typeof(TitleScreenCustomizationSystem), nameof(TitleScreenCustomizationSystem.OverrideLevelPath));
        }

        public void OnWorkshopLevelIDFieldEdited(string newValue)
        {
            newValue = newValue.Replace("https://steamcommunity.com/workshop/filedetails/?id=", string.Empty).Replace("/", string.Empty);
            m_WorkshopLevelIDInputField.text = newValue;

            TitleScreenCustomizationSystem.OverrideWorkshopLevelID = newValue;
            OverhaulSettingsManager.reference.SaveFieldValueOfClass(typeof(TitleScreenCustomizationSystem), nameof(TitleScreenCustomizationSystem.OverrideWorkshopLevelID));
        }

        public void OnUIAlignmentFieldEdited(int newValue)
        {
            TitleScreenCustomizationSystem.UIAlignment = newValue;
            OverhaulSettingsManager.reference.SaveFieldValueOfClass(typeof(TitleScreenCustomizationSystem), nameof(TitleScreenCustomizationSystem.UIAlignment));
        }
    }
}
