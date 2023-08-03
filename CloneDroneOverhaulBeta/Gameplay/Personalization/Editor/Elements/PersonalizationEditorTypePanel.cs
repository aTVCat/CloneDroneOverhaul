using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorTypePanel : PersonalizationEditorElement
    {
        [ActionReference(nameof(onChangeTypeButtonClicked))]
        [ObjectReference("ChangeTypeButton")]
        private Button m_ChangeTypeButton;

        [ObjectDefaultVisibility(false)]
        [ObjectReference("TypesPanel")]
        private GameObject m_TypesPanel;

        [ObjectDefaultVisibility(false)]
        [ObjectReference("Shading")]
        private GameObject m_Shading;

        [ActionReference(new string[] { nameof(hidePanelAndShading), nameof(onSkinsButtonClicked) })]
        [ObjectReference("Button_Skins")]
        private Button m_SkinsButton;
        [ActionReference(new string[] { nameof(hidePanelAndShading), nameof(onOutfitsButtonClicked) })]
        [ObjectReference("Button_Outfits")]
        private Button m_OutfitsButton;
        [ActionReference(new string[] { nameof(hidePanelAndShading), nameof(onPetsButtonClicked) })]
        [ObjectReference("Button_Pets")]
        private Button m_PetsButton;

        public void SetPanelAndShadingActive(bool value)
        {
            m_TypesPanel.SetActive(value);
            m_Shading.SetActive(value);
        }
        
        private void hidePanelAndShading()
        {
            SetPanelAndShadingActive(false);
        }

        private void onChangeTypeButtonClicked()
        {
            SetPanelAndShadingActive(true);
        }

        private void onSkinsButtonClicked()
        {

        }

        private void onOutfitsButtonClicked()
        {

        }

        private void onPetsButtonClicked()
        {

        }
    }
}
