using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationItemDisplay : OverhaulUIBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public const string ITEM_DEFAULT_FRAME_COLOR = "#333333";
        public const string ITEM_UNVERIFIED_FRAME_COLOR = "#00AAFF";
        public const string ITEM_UNVERIFIED_EXCLUSIVE_FRAME_COLOR = "#CC5500";
        public const string ITEM_EXCLUSIVE_FRAME_COLOR = "#FFBF00";
        public const string ITEM_SELECTED_FRAME_COLOR = "#02CC00";

        [UIElement("Frame")]
        private readonly Image m_frame;

        [UIElement("Glow")]
        private readonly Image m_glow;

        [UIElement("NewIndicator")]
        private readonly GameObject m_newIndicator;

        [UIElement("FavoriteIndicator")]
        private readonly GameObject m_favoriteIndicator;

        [UIElement("VerifiedIndicator")]
        private readonly GameObject m_wasVerifiedIndicator;

        [UIElement("UpdatedIndicator")]
        private readonly GameObject m_wasUpdatedIndicator;

        [UIElement("PreviewImage", true)]
        private readonly RawImage m_previewImage;

        private Button m_button;

        private UIPersonalizationItemBrowser m_browser;

        private RectTransform m_rectTransform;

        private UnityWebRequest m_webRequest;

        private Texture2D m_texture;

        public PersonalizationItemInfo ItemInfo;

        protected override void OnInitialized()
        {
            m_rectTransform = base.GetComponent<RectTransform>();
            m_button = base.GetComponent<Button>();
            m_button.onClick.AddListener(onClicked);

            GlobalEventManager.Instance.AddEventListener(PersonalizationManager.ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT, RefreshDisplays);
            RefreshDisplays();

            LoadIcon();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GlobalEventManager.Instance.RemoveEventListener(PersonalizationManager.ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT, RefreshDisplays);

            Texture2D texture = m_texture;
            if (texture)
                Destroy(texture);

            try
            {
                m_webRequest.Abort();
            }
            catch { }
        }

        public void SetBrowserUI(UIPersonalizationItemBrowser itemsBrowser)
        {
            m_browser = itemsBrowser;
        }

        public void RefreshDisplays()
        {
            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo == null) return;

            PersonalizationUserInfo personalizationUserInfo = PersonalizationManager.Instance.userInfo;

            bool wasUpdated = personalizationUserInfo.GetItemVersion(itemInfo) != itemInfo.Version;
            bool wasVerified = personalizationUserInfo.IsItemUnverified(itemInfo) && itemInfo.IsVerified;
            bool isDiscovered = personalizationUserInfo.IsItemDiscovered(itemInfo);
            bool isFavorite = personalizationUserInfo.IsItemFavorite(itemInfo);

            m_favoriteIndicator.SetActive(isFavorite);
            m_newIndicator.SetActive(!isDiscovered && !wasVerified && !wasUpdated);
            m_wasVerifiedIndicator.SetActive(wasVerified);
            m_wasUpdatedIndicator.SetActive(wasUpdated && !wasVerified && isDiscovered);

            RefreshColor();
        }

        public void RefreshColor()
        {
            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo == null)
                return;

            bool equipped = itemInfo.IsEquipped();
            bool isExclusive = itemInfo.IsExclusive();
            bool isVerified = itemInfo.IsVerified;

            string colorString;
            if (equipped)
            {
                colorString = ITEM_SELECTED_FRAME_COLOR;
            }
            else
            {
                if (isExclusive && isVerified) colorString = ITEM_EXCLUSIVE_FRAME_COLOR;
                else if (isExclusive) colorString = ITEM_UNVERIFIED_EXCLUSIVE_FRAME_COLOR;
                else if (isVerified) colorString = ITEM_DEFAULT_FRAME_COLOR;
                else colorString = ITEM_UNVERIFIED_FRAME_COLOR;
            }

            Color frameColor = ModParseUtils.TryParseToColor(colorString);

            m_frame.color = frameColor;
            m_glow.color = frameColor;
        }

        public void LoadIcon()
        {
            string path = PersonalizationItemInfo.GetPreviewFileFullPath(ItemInfo);
            if (!File.Exists(path))
            {
                // todo: placeholder image
                return;
            }
            loadIconCoroutine(path).Run();
        }

        private IEnumerator loadIconCoroutine(string path)
        {
            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{path}"))
            {
                m_webRequest = unityWebRequest;
                yield return unityWebRequest.SendWebRequest();
                m_webRequest = null;
                if (!unityWebRequest.isHttpError && !unityWebRequest.isNetworkError && unityWebRequest.isDone)
                {
                    Texture2D texture = (unityWebRequest.downloadHandler as DownloadHandlerTexture).texture;
                    texture.filterMode = FilterMode.Bilinear;
                    m_texture = texture;
                    m_previewImage.texture = texture;
                    m_previewImage.color = Color.white;
                }
            }
            yield break;
        }

        private void onClicked()
        {
            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo == null || !itemInfo.IsUnlocked()) return;

            updateItemUserInfo();

            PersonalizationManager.Instance.EquipItem(itemInfo);

            if (itemInfo.Category == PersonalizationCategory.WeaponSkins)
                m_browser.MakeDefaultSkinButtonInteractable();
        }

        private void updateItemUserInfo()
        {
            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo == null)
                return;

            PersonalizationUserInfo userInfo = PersonalizationManager.Instance?.userInfo;
            if (userInfo == null)
                return;

            if (!userInfo.IsItemDiscovered(itemInfo))
            {
                userInfo.SetIsItemDiscovered(itemInfo);
                m_newIndicator.SetActive(false);
            }

            if (userInfo.IsItemUnverified(itemInfo) && itemInfo.IsVerified)
            {
                userInfo.SetIsItemUnverified(itemInfo, false);
                m_wasVerifiedIndicator.SetActive(false);
            }

            if (userInfo.GetItemVersion(itemInfo) != itemInfo.Version)
            {
                userInfo.SetItemVersion(itemInfo, itemInfo.Version);
                m_wasUpdatedIndicator.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo == null)
                return;

            if (!itemInfo.IsUnlocked()) updateItemUserInfo();

            m_browser.ShowDescriptionBox(itemInfo, m_rectTransform);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_browser.ShowDescriptionBox(null, null);
        }
    }
}