using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementNetworkAddonDisplay : OverhaulUIBehaviour
    {
        [UIElement("ContentName")]
        private readonly Text m_addonNameText;

        [UIElement("ContentDescription")]
        private readonly Text m_addonDescriptionText;

        [UIElement("ContentSize")]
        private readonly Text m_addonSizeText;

        [UIElement("ErrorPanel", false)]
        private readonly GameObject m_errorPanelObject;

        [UIElement("ErrorLabel")]
        private readonly Text m_errorLabel;

        private AddonManager m_addonManager;

        public AddonDownloadInfo m_addonDownloadInfo;

        private Transform m_subUIParent;

        public void Initialize(AddonDownloadInfo addonDownloadInfo, Transform subUIParent)
        {
            base.InitializeElement();
            Button button = base.GetComponent<Button>();
            button.onClick.AddListener(OnClicked);

            m_addonManager = AddonManager.Instance;
            m_addonDownloadInfo = addonDownloadInfo;
            m_subUIParent = subUIParent;

            m_addonNameText.text = addonDownloadInfo.GetDisplayName();
            m_addonDescriptionText.text = addonDownloadInfo.GetDescription();
            m_addonSizeText.text = addonDownloadInfo.GetPackageSizeString();

            bool isSupported = addonDownloadInfo.Addon.IsSupported();
            if (!isSupported)
            {
                string versionString;
                if (addonDownloadInfo.Addon.MinModVersion != null)
                    versionString = addonDownloadInfo.Addon.MinModVersion.ToString();
                else
                    versionString = "N/A";

                m_errorLabel.text = $"{LocalizationManager.Instance.GetTranslatedString("addon_requires_version")} {versionString}";
            }

            m_errorPanelObject.SetActive(!isSupported);

            RectTransform rectTransform = base.transform as RectTransform;
            Vector2 size = rectTransform.sizeDelta;
            size.y = isSupported ? 80f : 110f;
            rectTransform.sizeDelta = size;
        }

        public void OnClicked()
        {
            UIAddonDetailsMenu menu = ModUIConstants.ShowAddonDetailsMenu(m_subUIParent);
            menu.SetAddon(m_addonDownloadInfo);
        }
    }
}
