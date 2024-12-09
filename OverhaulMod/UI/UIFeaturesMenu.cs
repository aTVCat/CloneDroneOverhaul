using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIFeaturesMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("FeatureStateDisplay", false)]
        private readonly ModdedObject m_featureStatePrefab;

        [UIElement("Content")]
        private readonly Transform m_container;

        [UIElementAction(nameof(OnResetButtonClicked))]
        [UIElement("ResetButton")]
        private readonly Button m_resetButton;

        public override bool hideTitleScreen => true;

        public override void Hide()
        {
            base.Hide();
        }

        public void OnResetButtonClicked()
        {

        }
    }
}
