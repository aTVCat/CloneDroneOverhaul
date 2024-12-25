using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAddonsEditor : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button m_saveButton;

        [UIElement("NeedsSaveIcon", false)]
        private readonly GameObject m_needsSaveIcon;

        [UIElementAction(nameof(OnAddonsButtonClicked))]
        [UIElement("AddonsButton")]
        private readonly Button m_addonsButton;

        [UIElementAction(nameof(OnCloseAddonsPanelButtonClicked))]
        [UIElement("CloseAddonsPanelButton")]
        private readonly Button m_closeAddonsPanelButton;

        [UIElementAction(nameof(OnNewAddonButtonClicked))]
        [UIElement("NewAddonButton")]
        private readonly Button m_newAddonButton;

        [UIElement("EditorBG", false)]
        private readonly GameObject m_editorBG;

        [UIElement("NonEditorBG", true)]
        private readonly GameObject m_nonEditorBG;

        [UIElement("AddonsPanel", false)]
        private readonly GameObject m_addonsPanel;

        [UIElement("AddonDisplay", false)]
        private readonly ModdedObject m_addonDisplay;

        [UIElement("Content")]
        private readonly Transform m_addonsContent;


        [UIElement("NameLanguageDropdown")]
        private readonly Dropdown m_displayNameLanguageDropdown;

        [UIElement("NameField")]
        private readonly InputField m_displayNameField;

        [UIElement("DescriptionLanguageDropdown")]
        private readonly Dropdown m_descriptionLanguageDropdown;

        [UIElement("DescriptionField")]
        private readonly InputField m_descriptionField;

        [UIElement("UniqueIDField")]
        private readonly InputField m_uniqueIDField;

        [UIElementAction(nameof(OnGenerateUniqueIDButtonClicked))]
        [UIElement("GenerateUniqueIDButton")]
        private readonly Button m_generateUniqueIDButton;

        [UIElement("AddonVersionField")]
        private readonly InputField m_addonVersionField;

        [UIElementAction(nameof(OnBumpAddonVersionButtonClicked))]
        [UIElement("BumpAddonVersionButton")]
        private readonly Button m_bumpAddonVersionButton;

        [UIElement("MinModVersionField")]
        private readonly InputField m_minModVersionField;

        [UIElementAction(nameof(OnSetCurrentModVersionButtonClicked))]
        [UIElement("SetCurrentModVersionButton")]
        private readonly Button m_setCurrentModVersionButton;

        [UIElement("PackageURLField")]
        private readonly InputField m_packageURLField;

        [UIElement("PackageSizeField")]
        private readonly InputField m_packageSizeField;

        [UIElement("PackageCompressedFilePathField")]
        private readonly InputField m_packageCompressedFilePathField;

        [UIElementAction(nameof(OnSetCurrentModVersionButtonClicked))]
        [UIElement("SetPackageCompressedFilePathButton")]
        private readonly Button m_setPackageCompressedFilePathButton;

        [UIElementAction(nameof(OnCalculatePackageSizeButtonClicked))]
        [UIElement("CalculatePackageSizeButton")]
        private readonly Button m_cCalculatePackageSizeButton;


        private AddonInfo m_editingAddonInfo;

        protected override void OnInitialized()
        {
            m_saveButton.interactable = false;
        }

        private void populateAddonsPanel()
        {
            if (m_addonsContent.childCount != 0)
                TransformUtils.DestroyAllChildren(m_addonsContent);

            System.Collections.Generic.List<AddonInfo> addons = AddonManager.Instance.GetInstalledAddons();
            foreach (AddonInfo addon in addons)
            {
                ModdedObject moddedObject = Instantiate(m_addonDisplay, m_addonsContent);
                moddedObject.gameObject.SetActive(true);
                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    editAddon(addon);
                });
            }
        }

        private void editAddon(AddonInfo addonInfo)
        {
            m_editingAddonInfo = addonInfo;

            m_saveButton.interactable = true;

            OnCloseAddonsPanelButtonClicked();
            m_editorBG.SetActive(true);
            m_nonEditorBG.SetActive(false);
        }

        private void saveEditingAddon()
        {
            m_needsSaveIcon.SetActive(false);

            AddonInfo addonInfo = m_editingAddonInfo;
            if (addonInfo != null)
            {
                updateAddonInfo(addonInfo);
            }
            else
            {
                ModUIUtils.MessagePopupOK("Error", "You're not editing any addon.", true);
            }
        }

        private void setFieldsValue(AddonInfo addonInfo)
        {

        }

        private void updateAddonInfo(AddonInfo addonInfo)
        {

        }

        public void OnAddonsButtonClicked()
        {
            populateAddonsPanel();
            m_addonsPanel.SetActive(true);
        }

        public void OnCloseAddonsPanelButtonClicked()
        {
            m_addonsPanel.SetActive(false);
        }

        public void OnNewAddonButtonClicked()
        {
        }

        public void OnSaveButtonClicked()
        {
            saveEditingAddon();
        }

        public void OnGenerateUniqueIDButtonClicked()
        {

        }

        public void OnBumpAddonVersionButtonClicked()
        {

        }

        public void OnSetCurrentModVersionButtonClicked()
        {

        }

        public void OnSetPackageCompressedFilePathButtonClicked()
        {

        }

        public void OnCalculatePackageSizeButtonClicked()
        {

        }
    }
}
