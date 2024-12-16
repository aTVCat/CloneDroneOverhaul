using OverhaulMod.Content;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class UIElementTitleScreenContentButton : OverhaulUIBehaviour
    {
        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;

        [UIElement("Image", true)]
        private readonly GameObject m_icon;

        private AddonManager m_addonManager;

        protected override void OnInitialized()
        {
            m_addonManager = AddonManager.Instance;
        }

        public override void Update()
        {
            bool isLoadingAnyContent = m_addonManager.IsLoadingAddons();
            m_loadingIndicator.SetActive(isLoadingAnyContent);
            m_icon.SetActive(!isLoadingAnyContent);
        }
    }
}
