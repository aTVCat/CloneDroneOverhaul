using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementWorkshopItemDisplay : OverhaulUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        [UIElement("Text")]
        private readonly Text m_titleText;

        [UIElement("Preview", false)]
        private readonly RawImage m_thumbnail;

        [UIElement("SelectedFrame", false)]
        private readonly GameObject m_selectedFrame;

        [UIElement("LoadingIndicator", true)]
        private readonly GameObject m_loadingIndicator;

        [UIElement("CompletedIndicator", false)]
        private readonly GameObject m_completedIndicator;

        private UnityWebRequest m_webRequest;

        public SteamWorkshopItem workshopItem
        {
            get;
            set;
        }

        public Texture2D thumbnailTexture
        {
            get;
            set;
        }

        public override void OnDestroy()
        {
            Texture2D texture = thumbnailTexture;
            if (texture)
                Destroy(texture);

            try
            {
                m_webRequest.Abort();
            }
            catch { }
        }

        public void Populate(SteamWorkshopItem steamWorkshopItem)
        {
            workshopItem = steamWorkshopItem;
            GetThumbnail();

            m_completedIndicator.SetActive(ChallengeManager.Instance.HasCompletedChallenge(steamWorkshopItem.WorkshopItemID.ToString()));
        }

        public void GetThumbnail()
        {
            SteamWorkshopItem steamWorkshopItem = workshopItem;
            if (steamWorkshopItem == null || steamWorkshopItem.PreviewURL.IsNullOrEmpty())
                return;

            UIElementWorkshopItemDisplay workshopItemDisplay = this;
            RepositoryManager.Instance.GetCustomTexture(steamWorkshopItem.PreviewURL, delegate (Texture2D texture)
            {
                if (!workshopItemDisplay)
                {
                    if (texture)
                        Destroy(texture);

                    return;
                }

                thumbnailTexture = texture;
                m_loadingIndicator.SetActive(false);
                m_thumbnail.gameObject.SetActive(true);
                m_thumbnail.texture = texture;
            }, delegate
            {
                if (!workshopItemDisplay)
                    return;

                m_loadingIndicator.SetActive(false);
            }, out m_webRequest, 60);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_selectedFrame.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_selectedFrame.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_selectedFrame.SetActive(false);
        }
    }
}
