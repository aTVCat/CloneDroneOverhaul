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

        private ModdedObject m_ContentPackEntryPrefab;
        private Transform m_ContentPackContainer;

        private OverhaulAdditionalContentController m_Controller;
        private byte m_CurrentPage;

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

            m_ContentPackEntryPrefab = MyModdedObject.GetObject<ModdedObject>(5);
            m_ContentPackEntryPrefab.gameObject.SetActive(false);
            m_ContentPackContainer = MyModdedObject.GetObject<Transform>(6);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        public void Show()
        {
            m_Controller = GetController<OverhaulAdditionalContentController>();

            base.gameObject.SetActive(true);
            SetActivePageIndex(0);
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        public void SetActivePageIndex(byte index)
        {
            m_CurrentPage = index;
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

            PopulatePage();
        }

        public void PopulatePage()
        {
            if (m_Controller == null)
            {
                return;
            }

            TransformUtils.DestroyAllChildren(m_ContentPackContainer);
            if(m_CurrentPage == 0)
            {
                foreach(OverhaulAdditionalContentPackInfo info in OverhaulAdditionalContentController.AllContent)
                {
                    ModdedObject obj = Instantiate(m_ContentPackEntryPrefab, m_ContentPackContainer);
                    obj.gameObject.AddComponent<OverhaulAdditionalContentUIEntry>().Initialize(info);
                    obj.gameObject.SetActive(true);
                    obj.GetObject<Text>(0).text = info.PackName;
                    obj.GetObject<Text>(1).text = info.PackDescription;
                }
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