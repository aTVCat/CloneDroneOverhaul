using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.NetworkAssets.AdditionalContent
{
    public class OverhaulAdditionalContentUI : OverhaulUI
    {
        private Button m_DoneButton;

        private Button m_BrowseContentButton;
        private Transform m_ContentBrowserPage;

        private Button m_ViewInstalledContentButton;
        private Transform m_InstalledContentPage;

        public override void Initialize()
        {
            Hide();

            m_DoneButton = MyModdedObject.GetObject<Button>(0);
            m_DoneButton.onClick.AddListener(OnDoneButtonClicked);

            m_ViewInstalledContentButton = MyModdedObject.GetObject<Button>(1);
            m_ViewInstalledContentButton.onClick.AddListener(OnViewInstalledContentClicked);
            m_InstalledContentPage = MyModdedObject.GetObject<Transform>(2);
            m_BrowseContentButton = MyModdedObject.GetObject<Button>(3);
            m_BrowseContentButton.onClick.AddListener(OnBrowseContentClicked);
            m_ContentBrowserPage = MyModdedObject.GetObject<Transform>(4);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            SetActivePageIndex(0);
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        public void SetActivePageIndex(byte index)
        {
            m_InstalledContentPage.gameObject.SetActive(false);
            m_ContentBrowserPage.gameObject.SetActive(false);

            switch (index)
            {
                case 0:
                    m_InstalledContentPage.gameObject.SetActive(true);
                    break;
                case 1:
                    m_ContentBrowserPage.gameObject.SetActive(true);
                    break;
            }
        }

        public void OnDoneButtonClicked()
        {
            Hide();
        }

        public void OnBrowseContentClicked()
        {
            SetActivePageIndex(1);
        }

        public void OnViewInstalledContentClicked()
        {
            SetActivePageIndex(0);
        }
    }
}