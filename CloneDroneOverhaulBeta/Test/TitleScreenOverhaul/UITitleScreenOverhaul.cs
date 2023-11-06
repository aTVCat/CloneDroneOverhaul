using CDOverhaul.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class UITitleScreenOverhaul : UIController
    {
        [UIElementComponents(new System.Type[] { typeof(OverhaulUIAnchoredPanelSlider) })]
        [UIElementReference("Buttons")]
        private OverhaulUIAnchoredPanelSlider m_ButtonsContainer;

        [UIElementReference("Buttons")]
        private RectTransform m_ButtonsRectTransform;

        [UIElementActionReference(nameof(OnCustomizeButtonClicked))]
        [UIElementReference("CustomizeTitleScreenButton")]
        private Button m_CustomizeButton;

        [UIElementDefaultVisibilityState(true)]
        [UIElementComponents(new System.Type[] { typeof(UIElementTitleScreenCustomizationPanel) })]
        [UIElementReference("CustomizationPanel")]
        private UIElementTitleScreenCustomizationPanel m_CustomizationPanel;

        [UIElementReference("Gradient")]
        private GameObject m_GradientObject;

        public override void Initialize()
        {
            base.Initialize();
            OverhaulSettingsManager.reference.AddOnSaveCallbackToField(typeof(TitleScreenCustomizationSystem), nameof(TitleScreenCustomizationSystem.UIAlignment), onAlignmentSettingUpdated);
            RefreshUI();
        }

        public void OnCustomizeButtonClicked()
        {
            m_CustomizationPanel.SetOpened(!m_CustomizationPanel.opened);
        }

        public void RefreshUI()
        {
            RectTransform rectTransform = m_ButtonsRectTransform;
            rectTransform.anchoredPosition = Vector2.zero;

            int value = TitleScreenCustomizationSystem.UIAlignment;
            switch (value)
            {
                default:
                    rectTransform.pivot = new Vector2(0f, 1f);
                    rectTransform.anchorMax = new Vector2(0f, 1f);
                    rectTransform.anchorMin = new Vector2(0f, 0f);
                    m_ButtonsContainer.StartPosition = new Vector3(-200f, -225f);
                    m_ButtonsContainer.TargetPosition = new Vector3(80f, -225f);
                    break;
                case 1:
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    m_ButtonsContainer.StartPosition = new Vector3(0f, -350f);
                    m_ButtonsContainer.TargetPosition = new Vector3(0f, 25f);
                    break;
            }
            ArenaCameraManager.Instance.updateLogoCameraRect();

            m_GradientObject.SetActive(value == 0);
        }

        private void onAlignmentSettingUpdated()
        {
            RefreshUI();
        }
    }
}
