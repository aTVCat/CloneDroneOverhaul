using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAddonsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnLocalAddonsEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button m_localAddonsEditorButton;

        [UIElementAction(nameof(OnNetworkAddonsEditorButtonClicked))]
        [UIElement("NetworkEditorButton")]
        private readonly Button m_networkAddonsEditorButton;

        [UIElement("LocalAddons")]
        private readonly ModdedObject m_localAddonsTab;
        [UIElement("NetworkAddons")]
        private readonly ModdedObject m_networkAddonsTab;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnTabSelected))]
        private readonly TabManager m_tabs;

        public override bool hideTitleScreen => true;

        protected override void OnInitialized()
        {
            m_tabs.AddTab(m_localAddonsTab, "local addons");
            m_tabs.AddTab(m_networkAddonsTab, "network addons");
            m_tabs.SelectTab("local addons");
        }

        public void OnTabSelected(UIElementTab elementTab)
        {
            bool local = elementTab.tabId == "local addons";

            UIElementTab oldTab = m_tabs.prevSelectedTab;
            UIElementTab newTab = m_tabs.selectedTab;
            if (oldTab)
            {
                RectTransform rt = oldTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 25f;
                rt.sizeDelta = vector;
            }
            if (newTab)
            {
                RectTransform rt = newTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 30f;
                rt.sizeDelta = vector;
            }

            Debug.Log(local);
        }

        public void OnLocalAddonsEditorButtonClicked()
        {
            ModUIConstants.ShowAddonsEditor(base.transform);
        }

        public void OnNetworkAddonsEditorButtonClicked()
        {
            ModUIConstants.ShowAddonsDownloadEditor(base.transform);
        }
    }
}
