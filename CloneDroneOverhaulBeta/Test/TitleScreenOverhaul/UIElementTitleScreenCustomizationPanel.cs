using CDOverhaul.HUD;
using CDOverhaul.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class UIElementTitleScreenCustomizationPanel : OverhaulBehaviour
    {
        [UIElementActionReference(nameof(OnLevelPathFieldEdited))]
        [UIElementReference(0)]
        private InputField m_LevelFileInputField;

        [UIElementActionReference(nameof(OnReloadButtonClicked))]
        [UIElementReference(1)]
        private Button m_ReloadButton;

        [UIElementReference(2)]
        private Text m_ErrorLabel;

        [UIElementActionReference(nameof(OnUIAlignmentFieldEdited))]
        [UIElementReference("UIAlignmentDropdown")]
        private UIElementDropdown m_Dropdown;

        public void Initialize()
        {
            UIController.AssignVariables(this);
            m_ErrorLabel.text = string.Empty;
            m_LevelFileInputField.text = TitleScreenCustomizationSystem.OverrideLevelPath;
            m_Dropdown.options = new List<Dropdown.OptionData>()
            {
                new Dropdown.OptionData() { text = "Left" },
                new Dropdown.OptionData() { text = "Center" }
            };
            m_Dropdown.value = TitleScreenCustomizationSystem.UIAlignment;
        }

        public void OnReloadButtonClicked()
        {
            TitleScreenOverhaulManager.reference.customizationSystem.SpawnLevel(out string error);
            m_ErrorLabel.text = error;
        }

        public void OnLevelPathFieldEdited(string newValue)
        {
            newValue = newValue.Replace("\"", string.Empty);
            m_LevelFileInputField.text = newValue;

            TitleScreenCustomizationSystem.OverrideLevelPath = newValue;
            OverhaulSettingsManager.reference.SaveFieldValueOfClass(typeof(TitleScreenCustomizationSystem), nameof(TitleScreenCustomizationSystem.OverrideLevelPath));
        }

        public void OnUIAlignmentFieldEdited(int newValue)
        {
            TitleScreenCustomizationSystem.UIAlignment = newValue;
            OverhaulSettingsManager.reference.SaveFieldValueOfClass(typeof(TitleScreenCustomizationSystem), nameof(TitleScreenCustomizationSystem.UIAlignment));
        }
    }
}
