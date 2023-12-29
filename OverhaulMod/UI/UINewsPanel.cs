using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UINewsPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button m_editorButton;

        [UIElementAction(nameof(OnRefreshButtonClicked))]
        [UIElement("RefreshButton")]
        private readonly Button m_refreshButton;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;

        [UIElement("NewsDisplay", false)]
        private readonly ModdedObject m_newsDisplay;

        [UIElement("Content")]
        private readonly Transform m_newsContainer;

        private bool m_isPopulating;
        private bool m_hasEverSuccessfullyPopulatedList;

        public override void Show()
        {
            base.Show();
            SetTitleScreenButtonActive(false);

            if (!m_hasEverSuccessfullyPopulatedList)
                Populate();
        }

        public override void Hide()
        {
            base.Hide();
            SetTitleScreenButtonActive(true);
        }

        public void Populate()
        {
            if (m_isPopulating)
                return;

            clearList();
            setIsPopulating(true);
            NewsManager.Instance.DownloadNewsInfoFile(delegate (NewsInfoList newsInfoList)
            {
                m_hasEverSuccessfullyPopulatedList = true;
                setIsPopulating(false);
                populateList(newsInfoList);
            }, delegate(string error)
            {
                ModUIUtility.MessagePopupOK("Error", error, 200f);
                setIsPopulating(false);
            }, true);
        }

        private void setIsPopulating(bool value)
        {
            m_isPopulating = value;
            m_refreshButton.interactable = !value;
            m_loadingIndicator.SetActive(value);
        }

        private void populateList(NewsInfoList newsInfoList)
        {
            int index = 0;
            foreach(var news in newsInfoList.News)
            {
                ModdedObject moddedObject = Instantiate(m_newsDisplay, m_newsContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = news.Title;
                if(index == 0)
                {
                    RectTransform rectTransform = moddedObject.transform as RectTransform;
                    Vector2 vector = rectTransform.sizeDelta;
                    vector.y += 7.5f;
                    rectTransform.sizeDelta = vector;
                }
                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    ShowNewsDetails(news);
                });
                index++;
            }
        }

        private void clearList()
        {
            if (m_newsContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_newsContainer);
        }

        public void ShowNewsDetails(NewsInfo newsInfo)
        {
            ModUIConstants.ShowNewsDetailsPanel(base.transform, newsInfo);
        }

        public void OnEditorButtonClicked()
        {
            ModUIConstants.ShowNewsInfoEditor(base.transform);
        }

        public void OnRefreshButtonClicked()
        {
            Populate();
        }
    }
}
