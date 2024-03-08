using OverhaulMod.Content;
using OverhaulMod.Utils;
using Steamworks;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIWorkshopItemPageWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnAuthorProfileButtonClicked))]
        [UIElement("AuthorProfileButton")]
        private readonly Button m_authorProfileButton;

        [UIElementAction(nameof(OnAuthorLevelsButtonClicked))]
        [UIElement("AuthorLevelsButton")]
        private readonly Button m_authorLevelsButton;

        [UIElementAction(nameof(OnShareButtonClicked))]
        [UIElement("ShareButton")]
        private readonly Button m_shareButton;

        [UIElementAction(nameof(OnSteamPageButtonClicked))]
        [UIElement("SteamPageButton")]
        private readonly Button m_steamPageButton;

        [UIElement("ItemTitle")]
        private readonly Text m_itemTitleText;

        [UIElement("ItemDescription")]
        private readonly Text m_itemDescriptionText;

        [UIElement("ItemAuthor")]
        private readonly Text m_itemAuthorText;

        [UIElement("ItemMainPreview")]
        private readonly RawImage m_itemPreviewImage;

        [UIElement("ItemAuthorAvatar")]
        private readonly RawImage m_authorAvatarImage;

        [UIElement("StarsFill")]
        private readonly Image m_ratingFillImage;

        [UIElement("UniqueVisitorsDetailText")]
        private readonly Text m_visitorsText;

        [UIElement("SubscribersDetailText")]
        private readonly Text m_subscribersText;

        [UIElement("FavouritesDetailText")]
        private readonly Text m_favoritesText;

        [UIElement("FileSizeDetailText")]
        private readonly Text m_fileSizeText;

        [UIElement("PostTimeDetailText")]
        private readonly Text m_postTimeText;

        [UIElement("UpdateTimeDetailText")]
        private readonly Text m_updateTimeText;

        [UIElement("TagsText")]
        private readonly Text m_tagsText;

        [UIElement("ItemImageDisplay", false)]
        private readonly ModdedObject m_additionalPreviewDisplayPrefab;

        [UIElement("ItemImageContainer")]
        private readonly Transform m_additionalPreviewDisplayContainer;

        private string m_authorProfileLink, m_itemLink;
        private CSteamID m_authorId;

        private string m_previewLink;
        private Texture2D m_previewTexture, m_authorAvatarTexture;

        private UnityWebRequest m_webRequest;

        public UIWorkshopBrowser browserUI
        {
            get;
            set;
        }

        public override void OnDisable()
        {
            dispose();
        }

        public void Populate(WorkshopItem workshopItem)
        {
            if (workshopItem == null || workshopItem.IsDisposed())
                return;

            m_itemTitleText.text = workshopItem.Name;
            m_itemDescriptionText.text = workshopItem.Description;

            if (!workshopItem.Author.IsNullOrEmpty() && workshopItem.Author != "[unknown]")
                m_itemAuthorText.text = $"By {workshopItem.Author.AddColor(Color.white)}";
            else
                m_itemAuthorText.text = $"By {"-".AddColor(Color.white)}";

            m_itemLink = $"https://steamcommunity.com/sharedfiles/filedetails/?id={workshopItem.ItemID}";
            m_authorProfileLink = $"https://steamcommunity.com/profiles/{workshopItem.AuthorID}";
            m_authorId = workshopItem.AuthorID;

            getMainPreview(workshopItem);
            getAuthorAvatar(workshopItem);
            populateDetails(workshopItem);
            populateAdditionalPreviews(workshopItem);
        }

        private void getMainPreview(WorkshopItem workshopItem)
        {
            m_itemPreviewImage.gameObject.SetActive(false);
            dispose();

            string link = workshopItem.PreviewURL;
            m_previewLink = link;

            UIWorkshopItemPageWindow itemPageWindow = this;
            RepositoryManager.Instance.GetCustomTexture(link, delegate (Texture2D texture)
            {
                if (!itemPageWindow || link != m_previewLink)
                {
                    if (texture)
                        Destroy(texture);

                    return;
                }

                m_previewTexture = texture;
                m_itemPreviewImage.gameObject.SetActive(true);
                m_itemPreviewImage.texture = texture;
                m_itemPreviewImage.rectTransform.sizeDelta = new Vector2(Mathf.Min(145f * (texture.width / (float)texture.height), 257.7778f), 145f);
            }, null, out m_webRequest, 60);
        }

        private void getAuthorAvatar(WorkshopItem workshopItem)
        {
            m_authorAvatarImage.gameObject.SetActive(false);

            int handle = SteamFriends.GetMediumFriendAvatar(workshopItem.AuthorID);
            if (SteamUtils.GetImageSize(handle, out uint width, out uint height))
            {
                byte[] bytes = new byte[width * height * 4];
                if (SteamUtils.GetImageRGBA(handle, bytes, bytes.Length))
                {
                    try
                    {
                        Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                        texture.LoadRawTextureData(bytes);
                        texture.Apply();
                        m_authorAvatarTexture = texture;
                        m_authorAvatarImage.texture = texture;
                        m_authorAvatarImage.gameObject.SetActive(true);
                    }
                    catch
                    {
                        m_authorAvatarImage.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void populateDetails(WorkshopItem workshopItem)
        {
            string tagsText = "-";
            string[] tagsArray = workshopItem.Tags;
            if (!tagsArray.IsNullOrEmpty())
            {
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                for (int i = 0; i < tagsArray.Length; i++)
                {
                    _ = stringBuilder.Append(tagsArray[i]);
                    if (i < tagsArray.Length - 1)
                        _ = stringBuilder.Append(", ");
                }
                tagsText = stringBuilder.ToString();
                _ = stringBuilder.Clear();
            }

            string postTimeText = $"{workshopItem.PostDate.ToShortDateString()}, {workshopItem.PostDate.ToShortTimeString()}";
            string updateTimeText = $"{workshopItem.UpdateDate.ToShortDateString()}, {workshopItem.UpdateDate.ToShortTimeString()}";
            string sizeText = $"{Mathf.Round(workshopItem.Size * 100f) / 100f} MBs";

            m_ratingFillImage.fillAmount = workshopItem.Rating;
            m_visitorsText.text = workshopItem.Views.ToString();
            m_subscribersText.text = workshopItem.Subscribers.ToString();
            m_favoritesText.text = workshopItem.Favorites.ToString();
            m_postTimeText.text = postTimeText;
            m_updateTimeText.text = updateTimeText;
            m_fileSizeText.text = sizeText;
            m_tagsText.text = tagsText;
        }

        private void populateAdditionalPreviews(WorkshopItem workshopItem)
        {
            if (m_additionalPreviewDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_additionalPreviewDisplayContainer);

            if (workshopItem.AdditionalPreviews.IsNullOrEmpty())
                return;

            foreach (WorkshopItemPreview preview in workshopItem.AdditionalPreviews.OrderBy(f => f.PreviewType != EItemPreviewType.k_EItemPreviewType_YouTubeVideo))
            {
                if (preview.URL.IsNullOrEmpty())
                    continue;

                ModdedObject moddedObject = Instantiate(m_additionalPreviewDisplayPrefab, m_additionalPreviewDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                UIElementWorkshopItemPreviewDisplay workshopItemPreviewDisplay = moddedObject.gameObject.AddComponent<UIElementWorkshopItemPreviewDisplay>();
                workshopItemPreviewDisplay.isVideo = preview.PreviewType != EItemPreviewType.k_EItemPreviewType_Image;
                workshopItemPreviewDisplay.link = preview.URL;
                workshopItemPreviewDisplay.InitializeElement();
            }
        }

        private void dispose()
        {
            if (m_webRequest != null)
            {
                try
                {
                    m_webRequest.Abort();
                    m_webRequest = null;
                }
                catch { }
            }

            Texture2D mp = m_previewTexture;
            if (mp)
                Destroy(mp);

            Texture2D aat = m_authorAvatarTexture;
            if (aat)
                Destroy(aat);
        }

        public void OnSteamPageButtonClicked()
        {
            string link = m_itemLink;
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
                SteamFriends.ActivateGameOverlayToWebPage(link);
            else
                Application.OpenURL(link);
        }

        public void OnShareButtonClicked()
        {
            GUIUtility.systemCopyBuffer = m_itemLink;
            ModUIUtils.MessagePopupOK("Link copied!", string.Empty, false);
        }

        public void OnAuthorProfileButtonClicked()
        {
            string link = m_authorProfileLink;
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
                SteamFriends.ActivateGameOverlayToWebPage(link);
            else
                Application.OpenURL(link);
        }

        public void OnAuthorLevelsButtonClicked()
        {
            UIWorkshopBrowser bui = browserUI;
            bui.searchLevelsByUser = m_authorId;
            bui.sourceType = 1;
            bui.Populate();
            Hide();
        }
    }
}
