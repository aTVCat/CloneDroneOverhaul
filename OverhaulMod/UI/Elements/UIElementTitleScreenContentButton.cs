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

        private ContentManager m_contentManager;

        protected override void OnInitialized()
        {
            m_contentManager = ContentManager.Instance;
        }

        public override void Update()
        {
            bool isLoadingAnyContent = m_contentManager.isLoadingAnyContent;
            m_loadingIndicator.SetActive(isLoadingAnyContent);
            m_icon.SetActive(!isLoadingAnyContent);
        }
    }
}
