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
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button _saveButton;

        [UIElement("NeedsSaveIcon", false)]
        private readonly GameObject _needsSaveIcon;

        [UIElementAction(nameof(OnAddonsButtonClicked))]
        [UIElement("AddonsButton")]
        private readonly Button _addonsButton;

        [UIElementAction(nameof(OnCloseAddonsPanelButtonClicked))]
        [UIElement("CloseAddonsPanelButton")]
        private readonly Button _closeAddonsPanelButton;

        [UIElementAction(nameof(OnNewAddonButtonClicked))]
        [UIElement("NewAddonButton")]
        private readonly Button _newAddonButton;

        [UIElement("EditorBG", false)]
        private readonly GameObject _editorBG;

        [UIElement("NonEditorBG", true)]
        private readonly GameObject _nonEditorBG;

        [UIElement("AddonsPanel", false)]
        private readonly GameObject _addonsPanel;

        [UIElement("AddonDisplay", false)]
        private readonly ModdedObject _addonDisplay;

        [UIElement("Content")]
        private readonly Transform _addonsContent;

        [UIElementAction(nameof(OnDisplayNameLanguageDropdownChanged))]
        [UIElement("NameLanguageDropdown")]
        private readonly Dropdown _displayNameLanguageDropdown;

        [UIElementCallback(true)]
        [UIElementAction(nameof(OnDisplayNameFieldChanged))]
        [UIElement("NameField")]
        private readonly InputField _displayNameField;

        [UIElementAction(nameof(OnDescriptionLanguageDropdownChanged))]
        [UIElement("DescriptionLanguageDropdown")]
        private readonly Dropdown _descriptionLanguageDropdown;

        [UIElementCallback(true)]
        [UIElementAction(nameof(OnDescriptionFieldChanged))]
        [UIElement("DescriptionField")]
        private readonly InputField _descriptionField;

        [UIElementAction(nameof(OnUniqueIDFieldChanged))]
        [UIElement("UniqueIDField")]
        private readonly InputField _uniqueIDField;

        [UIElementAction(nameof(OnGenerateUniqueIDButtonClicked))]
        [UIElement("GenerateUniqueIDButton")]
        private readonly Button _generateUniqueIDButton;

        [UIElement("AddonVersionField")]
        private readonly InputField _addonVersionField;

        [UIElementAction(nameof(OnBumpAddonVersionButtonClicked))]
        [UIElement("BumpAddonVersionButton")]
        private readonly Button _bumpAddonVersionButton;

        [UIElement("MinModVersionField")]
        private readonly InputField _minModVersionField;

        [UIElementAction(nameof(OnSetCurrentModVersionButtonClicked))]
        [UIElement("SetCurrentModVersionButton")]
        private readonly Button _setCurrentModVersionButton;

        private AddonInfo _editingAddonInfo;

        private string _editingDisplayNameTranslationLangCode, _editingDescriptionTranslationLangCode;

        private bool _disableUICallbacks;

        protected override void OnInitialized()
        {
            _saveButton.interactable = false;

            _editingDisplayNameTranslationLangCode = "en";
            _editingDescriptionTranslationLangCode = "en";

            _displayNameLanguageDropdown.options = ModLocalizationManager.Instance.GetLanguageOptions(false);
            _displayNameLanguageDropdown.value = 0;
            _descriptionLanguageDropdown.options = _displayNameLanguageDropdown.options;
            _descriptionLanguageDropdown.value = 0;
        }

        private void populateAddonsPanel()
        {
            if (_addonsContent.childCount != 0)
                TransformUtils.DestroyAllChildren(_addonsContent);

            System.Collections.Generic.List<AddonInfo> addons = AddonManager.Instance.GetLoadedAddons();
            foreach (AddonInfo addon in addons)
            {
                string displayName = addon.GetDisplayName();
                if (displayName.IsNullOrEmpty())
                    displayName = "<i>No name addon</i>".AddColor(Color.gray);

                ModdedObject moddedObject = Instantiate(_addonDisplay, _addonsContent);
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
            _editingAddonInfo = addonInfo;

            _saveButton.interactable = true;

            setFieldsValue(addonInfo);

            OnCloseAddonsPanelButtonClicked();
            _editorBG.SetActive(true);
            _nonEditorBG.SetActive(false);
        }

        private void saveEditingAddon()
        {
            _needsSaveIcon.SetActive(false);

            AddonInfo addonInfo = _editingAddonInfo;
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
            _disableUICallbacks = true;

            _displayNameField.text = addonInfo.GetDisplayName(_editingDisplayNameTranslationLangCode, true);
            _descriptionField.text = addonInfo.GetDescription(_editingDescriptionTranslationLangCode, true);
            _uniqueIDField.text = _editingAddonInfo.UniqueID;

            _addonVersionField.text = _editingAddonInfo.Version.ToString();
            _minModVersionField.text = _editingAddonInfo.MinModVersion.ToString();

            _disableUICallbacks = false;
        }

        private void updateAddonInfo(AddonInfo addonInfo)
        {
            _editingAddonInfo.UniqueID = _uniqueIDField.text;

            if (!int.TryParse(_addonVersionField.text, out int addonVersion))
            {
                ModUIUtils.MessagePopupOK("Cannot parse ADDON VERSION", "please try contacting tech support that doesnt exist", true);
            }
            else
            {
                _editingAddonInfo.Version = addonVersion;
            }

            if (!Version.TryParse(_minModVersionField.text, out Version minVersion))
            {
                ModUIUtils.MessagePopupOK("Cannot parse MIN MOD VERSION", "please try contacting tech support that doesnt exist", true);
            }
            else
            {
                _editingAddonInfo.MinModVersion = minVersion;
            }
        }

        private void onAddonCreation(string folderName)
        {
            string folderPath = Path.Combine(ModDirectories.AddonsFolder, folderName);
            Directory.CreateDirectory(folderPath);

            AddonInfo addonInfo = new AddonInfo
            {
                DisplayName = new System.Collections.Generic.Dictionary<string, string>(),
                Description = new System.Collections.Generic.Dictionary<string, string>(),
                MinModVersion = ModBuild.Version,
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
            _addonsPanel.SetActive(true);
        }

        public void OnCloseAddonsPanelButtonClicked()
        {
            _addonsPanel.SetActive(false);
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
            if (_disableUICallbacks)
                return;

            _editingDisplayNameTranslationLangCode = (_displayNameLanguageDropdown.options[value] as DropdownStringOptionData).StringValue;

            _disableUICallbacks = true;
            _displayNameField.text = _editingAddonInfo.GetDisplayName(_editingDisplayNameTranslationLangCode, true);
            _disableUICallbacks = false;
        }

        public void OnDisplayNameFieldChanged(string value)
        {
            if (_disableUICallbacks)
                return;

            _needsSaveIcon.SetActive(true);

            if (_editingAddonInfo.DisplayName.ContainsKey(_editingDisplayNameTranslationLangCode))
                _editingAddonInfo.DisplayName[_editingDisplayNameTranslationLangCode] = value;
            else
                _editingAddonInfo.DisplayName.Add(_editingDisplayNameTranslationLangCode, value);
        }

        public void OnDescriptionLanguageDropdownChanged(int value)
        {
            if (_disableUICallbacks)
                return;

            _editingDescriptionTranslationLangCode = (_descriptionLanguageDropdown.options[value] as DropdownStringOptionData).StringValue;

            _disableUICallbacks = true;
            _descriptionField.text = _editingAddonInfo.GetDescription(_editingDescriptionTranslationLangCode, true);
            _disableUICallbacks = false;
        }

        public void OnDescriptionFieldChanged(string value)
        {
            if (_disableUICallbacks)
                return;

            _needsSaveIcon.SetActive(true);

            if (_editingAddonInfo.Description.ContainsKey(_editingDescriptionTranslationLangCode))
                _editingAddonInfo.Description[_editingDescriptionTranslationLangCode] = value;
            else
                _editingAddonInfo.Description.Add(_editingDescriptionTranslationLangCode, value);
        }

        public void OnUniqueIDFieldChanged(string value)
        {
            _needsSaveIcon.SetActive(true);
        }

        public void OnGenerateUniqueIDButtonClicked()
        {
            _editingAddonInfo.GenerateUniqueID();
            _uniqueIDField.text = _editingAddonInfo.UniqueID;
        }

        public void OnBumpAddonVersionButtonClicked()
        {
            if (int.TryParse(_addonVersionField.text, out int ver))
            {
                _addonVersionField.text = (ver + 1).ToString();
            }
        }

        public void OnSetCurrentModVersionButtonClicked()
        {
            _minModVersionField.text = $"0.{ModBuild.VersionString}";
        }
    }
}
