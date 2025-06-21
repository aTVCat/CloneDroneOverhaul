using OverhaulMod.Content;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.IO;
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

        [UIElementAction(nameof(OnDisplayNameLanguageDropdownChanged))]
        [UIElement("NameLanguageDropdown")]
        private readonly Dropdown m_displayNameLanguageDropdown;

        [UIElementCallback(true)]
        [UIElementAction(nameof(OnDisplayNameFieldChanged))]
        [UIElement("NameField")]
        private readonly InputField m_displayNameField;

        [UIElementAction(nameof(OnDescriptionLanguageDropdownChanged))]
        [UIElement("DescriptionLanguageDropdown")]
        private readonly Dropdown m_descriptionLanguageDropdown;

        [UIElementCallback(true)]
        [UIElementAction(nameof(OnDescriptionFieldChanged))]
        [UIElement("DescriptionField")]
        private readonly InputField m_descriptionField;

        [UIElementAction(nameof(OnUniqueIDFieldChanged))]
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

        private AddonInfo m_editingAddonInfo;

        private string m_editingDisplayNameTranslationLangCode, m_editingDescriptionTranslationLangCode;

        private bool m_disableUICallbacks;

        protected override void OnInitialized()
        {
            m_saveButton.interactable = false;

            m_editingDisplayNameTranslationLangCode = "en";
            m_editingDescriptionTranslationLangCode = "en";

            m_displayNameLanguageDropdown.options = ModLocalizationManager.Instance.GetLanguageOptions(false);
            m_displayNameLanguageDropdown.value = 0;
            m_descriptionLanguageDropdown.options = m_displayNameLanguageDropdown.options;
            m_descriptionLanguageDropdown.value = 0;
        }

        private void populateAddonsPanel()
        {
            if (m_addonsContent.childCount != 0)
                TransformUtils.DestroyAllChildren(m_addonsContent);

            System.Collections.Generic.List<AddonInfo> addons = AddonManager.Instance.GetLoadedAddons();
            foreach (AddonInfo addon in addons)
            {
                string displayName = addon.GetDisplayName();
                if (displayName.IsNullOrEmpty())
                    displayName = "<i>No name addon</i>".AddColor(Color.gray);

                ModdedObject moddedObject = Instantiate(m_addonDisplay, m_addonsContent);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = displayName;
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

            setFieldsValue(addonInfo);

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
                ModJsonUtils.WriteStream(Path.Combine(addonInfo.FolderPath, AddonManager.ADDON_INFO_FILE), addonInfo);

            }
            else
            {
                ModUIUtils.MessagePopupOK("Error", "You're not editing any addon.", true);
            }
        }

        private void setFieldsValue(AddonInfo addonInfo)
        {
            m_disableUICallbacks = true;

            m_displayNameField.text = addonInfo.GetDisplayName(m_editingDisplayNameTranslationLangCode, true);
            m_descriptionField.text = addonInfo.GetDescription(m_editingDescriptionTranslationLangCode, true);
            m_uniqueIDField.text = m_editingAddonInfo.UniqueID;

            m_addonVersionField.text = m_editingAddonInfo.Version.ToString();
            m_minModVersionField.text = m_editingAddonInfo.MinModVersion.ToString();

            m_disableUICallbacks = false;
        }

        private void updateAddonInfo(AddonInfo addonInfo)
        {
            m_editingAddonInfo.UniqueID = m_uniqueIDField.text;

            if (!int.TryParse(m_addonVersionField.text, out int addonVersion))
            {
                ModUIUtils.MessagePopupOK("Cannot parse ADDON VERSION", "please try contacting tech support that doesnt exist", true);
            }
            else
            {
                m_editingAddonInfo.Version = addonVersion;
            }

            if (!Version.TryParse(m_minModVersionField.text, out Version minVersion))
            {
                ModUIUtils.MessagePopupOK("Cannot parse MIN MOD VERSION", "please try contacting tech support that doesnt exist", true);
            }
            else
            {
                m_editingAddonInfo.MinModVersion = minVersion;
            }
        }

        private void onAddonCreation(string folderName)
        {
            string folderPath = Path.Combine(ModCore.addonsFolder, folderName);
            Directory.CreateDirectory(folderPath);

            AddonInfo addonInfo = new AddonInfo
            {
                DisplayName = new System.Collections.Generic.Dictionary<string, string>(),
                Description = new System.Collections.Generic.Dictionary<string, string>(),
                MinModVersion = ModBuildInfo.version,
                FolderPath = folderPath
            };
            addonInfo.GenerateUniqueID();

            ModJsonUtils.WriteStream(Path.Combine(folderPath, AddonManager.ADDON_INFO_FILE), addonInfo);

            AddonManager.Instance.AddLoadedAddon(addonInfo);
            editAddon(addonInfo);
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
            UIAddonsEditorCreationDialog dialog = ModUIConstants.ShowAddonsEditorCreationDialog(base.transform);
            dialog.Callback = onAddonCreation;
        }

        public void OnSaveButtonClicked()
        {
            saveEditingAddon();
        }

        public void OnDisplayNameLanguageDropdownChanged(int value)
        {
            if (m_disableUICallbacks)
                return;

            m_editingDisplayNameTranslationLangCode = (m_displayNameLanguageDropdown.options[value] as DropdownStringOptionData).StringValue;

            m_disableUICallbacks = true;
            m_displayNameField.text = m_editingAddonInfo.GetDisplayName(m_editingDisplayNameTranslationLangCode, true);
            m_disableUICallbacks = false;
        }

        public void OnDisplayNameFieldChanged(string value)
        {
            if (m_disableUICallbacks)
                return;

            m_needsSaveIcon.SetActive(true);

            if (m_editingAddonInfo.DisplayName.ContainsKey(m_editingDisplayNameTranslationLangCode))
                m_editingAddonInfo.DisplayName[m_editingDisplayNameTranslationLangCode] = value;
            else
                m_editingAddonInfo.DisplayName.Add(m_editingDisplayNameTranslationLangCode, value);
        }

        public void OnDescriptionLanguageDropdownChanged(int value)
        {
            if (m_disableUICallbacks)
                return;

            m_editingDescriptionTranslationLangCode = (m_descriptionLanguageDropdown.options[value] as DropdownStringOptionData).StringValue;

            m_disableUICallbacks = true;
            m_descriptionField.text = m_editingAddonInfo.GetDescription(m_editingDescriptionTranslationLangCode, true);
            m_disableUICallbacks = false;
        }

        public void OnDescriptionFieldChanged(string value)
        {
            if (m_disableUICallbacks)
                return;

            m_needsSaveIcon.SetActive(true);

            if (m_editingAddonInfo.Description.ContainsKey(m_editingDescriptionTranslationLangCode))
                m_editingAddonInfo.Description[m_editingDescriptionTranslationLangCode] = value;
            else
                m_editingAddonInfo.Description.Add(m_editingDescriptionTranslationLangCode, value);
        }

        public void OnUniqueIDFieldChanged(string value)
        {
            m_needsSaveIcon.SetActive(true);
        }

        public void OnGenerateUniqueIDButtonClicked()
        {
            m_editingAddonInfo.GenerateUniqueID();
            m_uniqueIDField.text = m_editingAddonInfo.UniqueID;
        }

        public void OnBumpAddonVersionButtonClicked()
        {
            if (int.TryParse(m_addonVersionField.text, out int ver))
            {
                m_addonVersionField.text = (ver + 1).ToString();
            }
        }

        public void OnSetCurrentModVersionButtonClicked()
        {
            m_minModVersionField.text = ModBuildInfo.versionStringNoBranch;
        }
    }
}
