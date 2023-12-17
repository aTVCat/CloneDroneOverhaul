using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIExclusiveContentMenu : OverhaulUIBehaviour
    {
        [UIElement("StatusBarText")]
        private readonly Text m_statusBarText;

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button m_editorButton;

        [UIElementAction(nameof(OnRetrieveButtonClicked))]
        [UIElement("RetrieveDataButton")]
        private readonly Button m_retrieveDataButton;

        [UIElementAction(nameof(OnRefreshButtonClicked))]
        [UIElement("RefreshButton")]
        private readonly Button m_refreshButton;

        [UIElement("UnlockedItemDisplayPrefab", false)]
        private readonly ModdedObject m_unlockedItemDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform m_container;

        protected override void OnInitialized()
        {
            m_statusBarText.text = "Idle";
        }

        public override void Show()
        {
            base.Show();
            SetTitleScreenButtonActive(false);
            Populate();
        }

        public override void Hide()
        {
            base.Hide();
            SetTitleScreenButtonActive(true);
        }

        public void Populate()
        {
            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            foreach (Content.ExclusiveContentInfo contentInfo in ModExclusiveContentManager.Instance.GetUnlockedContent())
            {
                ModdedObject moddedObject = Instantiate(m_unlockedItemDisplayPrefab, m_container);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = contentInfo.Name;
            }
        }

        public void OnEditorButtonClicked()
        {
            ModUIConstants.ShowExclusiveContentEditor(base.transform);
        }

        public void OnRetrieveButtonClicked()
        {
            m_statusBarText.text = "Retrieving data...";
            m_retrieveDataButton.interactable = false;
            ModExclusiveContentManager.Instance.RetrieveDataFromRepository(delegate
            {
                m_retrieveDataButton.interactable = true;
                m_statusBarText.text = "Successfully retrieved data!";
                Populate();
            }, delegate (string error)
            {
                m_retrieveDataButton.interactable = true;
                m_statusBarText.text = "Error: " + error;
            }, true);
        }

        public void OnRefreshButtonClicked()
        {
            Populate();
        }
    }
}
