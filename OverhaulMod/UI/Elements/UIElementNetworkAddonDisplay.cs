using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementNetworkAddonDisplay : OverhaulUIBehaviour
    {
        [UIElement("ContentName")]
        private readonly Text _addonNameText;

        [UIElement("ContentDescription")]
        private readonly Text _addonDescriptionText;

        [UIElement("ContentSize")]
        private readonly Text _addonSizeText;

        [UIElement("ErrorPanel", false)]
        private readonly GameObject _errorPanelObject;

        [UIElement("ErrorLabel")]
        private readonly Text _errorLabel;

        private AddonManager _addonManager;

        public AddonDownloadInfo _addonDownloadInfo;

        private Transform _subUIParent;

        public void Initialize(AddonDownloadInfo addonDownloadInfo, Transform subUIParent)
        {
            base.InitializeElement();
            Button button = base.GetComponent<Button>();
            button.onClick.AddListener(OnClicked);

            _addonManager = AddonManager.Instance;
            _addonDownloadInfo = addonDownloadInfo;
            _subUIParent = subUIParent;

            _addonNameText.text = addonDownloadInfo.GetDisplayName();
            _addonDescriptionText.text = addonDownloadInfo.GetDescription();
            _addonSizeText.text = addonDownloadInfo.GetPackageSizeString();

            bool isSupported = addonDownloadInfo.Addon.IsSupported();
            if (!isSupported)
            {
                string versionString;
                if (addonDownloadInfo.Addon.MinModVersion != null)
                    versionString = addonDownloadInfo.Addon.MinModVersion.ToString();
                else
                    versionString = "N/A";

                _errorLabel.text = $"{LocalizationManager.Instance.GetTranslatedString("addon_requires_version")} {versionString}";
            }

            _errorPanelObject.SetActive(!isSupported);

            RectTransform rectTransform = base.transform as RectTransform;
            Vector2 size = rectTransform.sizeDelta;
            size.y = isSupported ? 90f : 120f;
            rectTransform.sizeDelta = size;
        }

        public void OnClicked()
        {
            UIAddonDetailsMenu menu = ModUIConstants.ShowAddonDetailsMenu(_subUIParent);
            menu.SetAddon(_addonDownloadInfo);
        }
    }
}
