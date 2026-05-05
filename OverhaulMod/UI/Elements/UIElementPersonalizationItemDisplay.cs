using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationItemDisplay : OverhaulUIBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public const string ITE_DEFAULT_FRAME_COLOR = "#333333";
        public const string ITE_UNVERIFIED_FRAME_COLOR = "#00AAFF";
        public const string ITE_UNVERIFIED_EXCLUSIVE_FRAME_COLOR = "#CC5500";
        public const string ITE_EXCLUSIVE_FRAME_COLOR = "#FFBF00";
        public const string ITE_SELECTED_FRAME_COLOR = "#02CC00";

        [UIElement("Frame")]
        private readonly Image _frame;

        [UIElement("Glow")]
        private readonly Image _glow;

        [UIElement("NewIndicator")]
        private readonly GameObject _newIndicator;

        [UIElement("FavoriteIndicator")]
        private readonly GameObject _favoriteIndicator;

        [UIElement("VerifiedIndicator")]
        private readonly GameObject _wasVerifiedIndicator;

        [UIElement("UpdatedIndicator")]
        private readonly GameObject _wasUpdatedIndicator;

        [UIElement("EquippedIndicator")]
        private readonly GameObject _equippedIndicator;

        [UIElement("PreviewImage", true)]
        private readonly RawImage _previewImage;

        private Button _button;

        private UIPersonalizationItemBrowser _browser;

        private RectTransform _rectTransform;

        private UnityWebRequest _webRequest;

        private Texture2D _texture;

        public PersonalizationItemInfo ItemInfo;

        protected override void OnInitialized()
        {
            _rectTransform = base.GetComponent<RectTransform>();
            _button = base.GetComponent<Button>();
            _button.onClick.AddListener(onClicked);

            GlobalEventManager.Instance.AddEventListener(PersonalizationManager.ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT, RefreshDisplays);
            RefreshDisplays();

            LoadIcon();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GlobalEventManager.Instance.RemoveEventListener(PersonalizationManager.ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT, RefreshDisplays);

            Texture2D texture = _texture;
            if (texture)
                Destroy(texture);

            try
            {
                _webRequest.Abort();
            }
            catch { }
        }

        public void SetBrowserUI(UIPersonalizationItemBrowser itemsBrowser)
        {
            _browser = itemsBrowser;
        }

        public void RefreshDisplays()
        {
            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo == null) return;

            PersonalizationUserInfo personalizationUserInfo = PersonalizationManager.Instance.UserInfo;

            bool equipped = itemInfo.IsEquipped();
            bool wasUpdated = personalizationUserInfo.GetItemVersion(itemInfo) != itemInfo.Version;
            bool wasVerified = personalizationUserInfo.IsItemUnverified(itemInfo) && itemInfo.IsVerified;
            bool isDiscovered = personalizationUserInfo.IsItemDiscovered(itemInfo);
            bool isFavorite = personalizationUserInfo.IsItemFavorite(itemInfo);

            _favoriteIndicator.SetActive(isFavorite);
            _newIndicator.SetActive(!equipped && !isDiscovered && !wasVerified && !wasUpdated);
            _wasVerifiedIndicator.SetActive(!equipped && wasVerified);
            _wasUpdatedIndicator.SetActive(!equipped && wasUpdated && !wasVerified && isDiscovered);
            _equippedIndicator.SetActive(equipped);

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
                colorString = ITE_SELECTED_FRAME_COLOR;
            }
            else
            {
                if (isExclusive && isVerified) colorString = ITE_EXCLUSIVE_FRAME_COLOR;
                else if (isExclusive) colorString = ITE_UNVERIFIED_EXCLUSIVE_FRAME_COLOR;
                else if (isVerified) colorString = ITE_DEFAULT_FRAME_COLOR;
                else colorString = ITE_UNVERIFIED_FRAME_COLOR;
            }

            Color frameColor = ModParseUtils.TryParseColor(colorString);

            _frame.color = frameColor;
            _glow.color = frameColor;
        }

        public void LoadIcon()
        {
            string path = PersonalizationItemInfo.GetPreviewFileFullPath(ItemInfo);
            if (!File.Exists(path))
            {
                _previewImage.texture = MultiplayerCharacterCustomizationManager.Instance.FavColorRandomSpriteOpaque.texture;
                _previewImage.color = Color.white;
                return;
            }
            loadIconCoroutine(path).Run();
        }

        private IEnumerator loadIconCoroutine(string path)
        {
            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{path}"))
            {
                _webRequest = unityWebRequest;
                yield return unityWebRequest.SendWebRequest();
                _webRequest = null;
                if (unityWebRequest.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = (unityWebRequest.downloadHandler as DownloadHandlerTexture).texture;
                    texture.filterMode = FilterMode.Bilinear;
                    _texture = texture;
                    _previewImage.texture = texture;
                    _previewImage.color = Color.white;
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
                _browser.MakeDefaultSkinButtonInteractable();
        }

        private void updateItemUserInfo()
        {
            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo == null)
                return;

            PersonalizationUserInfo userInfo = PersonalizationManager.Instance?.UserInfo;
            if (userInfo == null)
                return;

            if (!userInfo.IsItemDiscovered(itemInfo))
            {
                userInfo.SetIsItemDiscovered(itemInfo);
                _newIndicator.SetActive(false);
            }

            if (userInfo.IsItemUnverified(itemInfo) && itemInfo.IsVerified)
            {
                userInfo.SetIsItemUnverified(itemInfo, false);
                _wasVerifiedIndicator.SetActive(false);
            }

            if (userInfo.GetItemVersion(itemInfo) != itemInfo.Version)
            {
                userInfo.SetItemVersion(itemInfo, itemInfo.Version);
                _wasUpdatedIndicator.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo == null)
                return;

            if (!itemInfo.IsUnlocked()) updateItemUserInfo();

            _browser.ShowDescriptionBox(itemInfo, _rectTransform);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _browser.ShowDescriptionBox(null, null);
        }
    }
}