using CDOverhaul.HUD.Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorTypePanel : PersonalizationEditorUIElement
    {
        public static bool HasPassedTutorial
        {
            get;
            set;
        }

        [UIElementActionReference(nameof(onChangeTypeButtonClicked))]
        [UIElementReferenceAttribute("ChangeTypeButton")]
        private readonly Button m_ChangeTypeButton;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementReferenceAttribute("TypesPanel")]
        private readonly GameObject m_TypesPanel;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementReferenceAttribute("Shading")]
        private readonly GameObject m_Shading;

        [UIElementActionReference(new string[] { nameof(hidePanelAndShading), nameof(onSkinsButtonClicked) })]
        [UIElementReferenceAttribute("Button_Skins")]
        private readonly Button m_SkinsButton;
        [UIElementActionReference(new string[] { nameof(hidePanelAndShading), nameof(onOutfitsButtonClicked) })]
        [UIElementReferenceAttribute("Button_Outfits")]
        private readonly Button m_OutfitsButton;
        [UIElementActionReference(new string[] { nameof(hidePanelAndShading), nameof(onPetsButtonClicked) })]
        [UIElementReferenceAttribute("Button_Pets")]
        private readonly Button m_PetsButton;

        private Transform m_DefaultParent;

        public void SetPanelAndShadingActive(bool value)
        {
            m_TypesPanel.SetActive(value);
            m_Shading.SetActive(value);

            if (value)
            {
                m_SkinsButton.interactable = PersonalizationManager.reference?.weaponSkins;
            }
        }

        public void StartTutorial()
        {
            if (HasPassedTutorial)
                return;

            m_DefaultParent = base.transform.parent;
            OverhaulTutorialUI tutorialUI = OverhaulController.Get<OverhaulTutorialUI>();
            tutorialUI.SetUITaskActive(true);
            tutorialUI.ParentTransformToUITask(base.transform);
            tutorialUI.SetTooltipActive(true);
            tutorialUI.SetTooltipContext("Select the items type", "Click on that \"Select\" button and choose the kind of items you want to edit.");
        }

        private void hidePanelAndShading()
        {
            SetPanelAndShadingActive(false);
        }

        private void onChangeTypeButtonClicked()
        {
            SetPanelAndShadingActive(true);
            if (HasPassedTutorial)
                return;

            base.transform.SetParent(m_DefaultParent, true);
            base.transform.SetAsFirstSibling();
            base.gameObject.SetActive(false);
            OverhaulTutorialUI tutorialUI = OverhaulController.Get<OverhaulTutorialUI>();
            tutorialUI.SetUITaskActive(false);
            tutorialUI.SetTooltipActive(false);
            HasPassedTutorial = true;
        }

        private void onSkinsButtonClicked()
        {
            PersonalizationEditor.EditingCategory = EPersonalizationCategory.WeaponSkins;
            EditorUI.LoadPanel.OnLoadButtonClicked();
        }

        private void onOutfitsButtonClicked()
        {
            PersonalizationEditor.EditingCategory = EPersonalizationCategory.Outfits;
            EditorUI.LoadPanel.OnLoadButtonClicked();
        }

        private void onPetsButtonClicked()
        {
            PersonalizationEditor.EditingCategory = EPersonalizationCategory.Pets;
            EditorUI.LoadPanel.OnLoadButtonClicked();
        }
    }
}
