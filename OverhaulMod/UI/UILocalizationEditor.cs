using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UILocalizationEditor : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button _saveButton;

        [UIElementAction(nameof(OnLanguagesDropdownChanged))]
        [UIElement("LanguagesDropdown")]
        private readonly Dropdown _languagesDropdown;

        [UIElement("TranslationKeyDisplay", false)]
        private readonly ModdedObject _translationKeyPrefab;
        [UIElement("AddTranslationKeyButton", false)]
        private readonly Button _addTranslationKeyButtonPrefab;
        [UIElement("TextPrefab", false)]
        private readonly Text _textPrefab;
        [UIElement("TranslationKeysListContainer")]
        private readonly Transform _translationKeysContainer;
        [UIElement("LoadingIndicator", false)]
        private readonly GameObject _loadingIndicatorObject;

        [UIElementAction(nameof(OnChangedTranslationKeyField))]
        [UIElement("EditingKeyText")]
        private readonly InputField _translationKeyLabel;
        [UIElement("EditingValueInputField")]
        private readonly InputField _translationValueInputField;

        [UIElementAction(nameof(OnToLowerButtonClicked))]
        [UIElement("ToLowerButton")]
        private readonly Button _toLowerButton;
        [UIElementAction(nameof(OnDeleteTranslationButtonClicked))]
        [UIElement("DeleteButton")]
        private readonly Button _deleteButton;

        [UIElement("ArialRText")]
        private readonly Text _arialRFontPreviewText;
        [UIElement("ArialBText")]
        private readonly Text _arialBFontPreviewText;
        [UIElement("OSRText")]
        private readonly Text _openSansRFontPreviewText;
        [UIElement("OSBText")]
        private readonly Text _openSansBFontPreviewText;
        [UIElement("EditUndoText")]
        private readonly Text _editUndoFontPreviewText;
        [UIElement("PixelsSimpleText")]
        private readonly Text _pixelsSimpleFontPreviewText;
        [UIElement("TriggeringFanfaresText")]
        private readonly Text _triggeringFanfaresFontPreviewText;

        [UIElementAction(nameof(OnSearchBoxChanged))]
        [UIElement("SearchBox")]
        private readonly InputField _searchBox;

        private Dictionary<string, GameObject> _cachedInstantiatedKeyDisplays;

        public override bool hideTitleScreen => true;

        public string editingLangId
        {
            get
            {
                return _languagesDropdown.options[_languagesDropdown.value].text;
            }
        }

        public string editingTranslationKey
        {
            get;
            set;
        }

        public int siblingIndexOfTranslationKey
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            _cachedInstantiatedKeyDisplays = new Dictionary<string, GameObject>();

            _languagesDropdown.options = ModLocalizationManager.Instance.GetLanguageOptions(true);
            _languagesDropdown.value = 0;
            _translationValueInputField.onEndEdit.AddListener(delegate (string str)
            {
                if (editingLangId.IsNullOrEmpty() || editingTranslationKey.IsNullOrEmpty())
                    return;

                ModLocalizationManager.Instance.SetTranslation(editingLangId, editingTranslationKey, str);
            });
            _translationValueInputField.onValueChanged.AddListener(delegate
            {
                RefreshPreview();
            });

            PopulateTranslations();
        }

        public void EditTranslation(string key)
        {
            editingTranslationKey = key;

            _translationKeyLabel.text = key;
            _translationValueInputField.text = ModLocalizationManager.Instance.GetTranslation(editingLangId, editingTranslationKey);
            RefreshPreview();
        }

        public void ChangeTranslation(string oldName, string newName)
        {
            if (_cachedInstantiatedKeyDisplays.TryGetValue(oldName, out GameObject display))
            {
                editingTranslationKey = newName;

                ModLocalizationManager.Instance.ChangeTranslation(oldName, newName);
                try
                {
                    ModdedObject translationKey = display.GetComponent<ModdedObject>();
                    if (translationKey)
                    {
                        translationKey.GetObject<Text>(0).text = newName;
                    }

                    Button button = translationKey.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(delegate
                    {
                        siblingIndexOfTranslationKey = translationKey.transform.GetSiblingIndex();
                        EditTranslation(newName);
                    });
                }
                catch { }

                _ = _cachedInstantiatedKeyDisplays.Remove(oldName);
                _cachedInstantiatedKeyDisplays.Add(newName, display);
            }
        }

        public void DeleteTranslation(string key)
        {
            editingTranslationKey = string.Empty;

            ModLocalizationManager.Instance.DeleteTranslation(key);
            _ = _cachedInstantiatedKeyDisplays.Remove(key);

            _translationKeyLabel.text = string.Empty;
            _translationValueInputField.text = string.Empty;

            try
            {
                Transform translationKey = _translationKeysContainer.GetChild(siblingIndexOfTranslationKey);
                if (translationKey)
                {
                    Destroy(translationKey.gameObject);
                }
            }
            catch { }
        }

        public void RefreshPreview()
        {
            string text = _translationValueInputField.text;

            _arialRFontPreviewText.text = text;
            _arialBFontPreviewText.text = text;
            _openSansRFontPreviewText.text = text;
            _openSansBFontPreviewText.text = text;
            _editUndoFontPreviewText.text = text;
            _pixelsSimpleFontPreviewText.text = text;
            _triggeringFanfaresFontPreviewText.text = text;
        }

        public ModdedObject InstantiateTranslationKeyDisplay(string key)
        {
            ModdedObject translationKey = Instantiate(_translationKeyPrefab, _translationKeysContainer);
            translationKey.gameObject.SetActive(true);
            translationKey.GetObject<Text>(0).text = key;
            Button button = translationKey.GetComponent<Button>();
            button.onClick.AddListener(delegate
            {
                siblingIndexOfTranslationKey = translationKey.transform.GetSiblingIndex();
                EditTranslation(key);
            });

            _cachedInstantiatedKeyDisplays.Add(key, translationKey.gameObject);
            return translationKey;
        }

        public void PopulateTranslations()
        {
            _cachedInstantiatedKeyDisplays.Clear();
            if (_translationKeysContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_translationKeysContainer);

            ModLocalizationManager manager = ModLocalizationManager.Instance;
            if (!manager.CanLanguageBeTranslated(editingLangId))
            {
                Text text = Instantiate(_textPrefab, _translationKeysContainer);
                text.gameObject.SetActive(true);
                text.text = "This language cannot be translated now";
                return;
            }

            Dictionary<string, string> translationDictionary = manager.GetTranslationDictionary(editingLangId);
            if (translationDictionary != null && translationDictionary.Count != 0)
            {
                foreach (string key in translationDictionary.Keys)
                    _ = InstantiateTranslationKeyDisplay(key);
            }

            Button addTranslationButton = Instantiate(_addTranslationKeyButtonPrefab, _translationKeysContainer);
            addTranslationButton.gameObject.SetActive(true);
            addTranslationButton.onClick.AddListener(OnAddTranslationButtonClicked);
        }

        public void OnLanguagesDropdownChanged(int index)
        {
            PopulateTranslations();
            if (!editingTranslationKey.IsNullOrEmpty())
            {
                EditTranslation(editingTranslationKey);
            }
        }

        public void OnAddTranslationButtonClicked()
        {
            ModUIUtils.InputFieldWindow("Add translation", "Enter non-existing translation name", null, 0, 125f, delegate (string value)
            {
                ModLocalizationManager.Instance.AddTranslation(value);
                ModdedObject moddedObject = InstantiateTranslationKeyDisplay(value);
                moddedObject.transform.SetSiblingIndex(_translationKeysContainer.childCount - 2);
                EditTranslation(value);
            });
        }

        public void OnSaveButtonClicked()
        {
            ModLocalizationManager.Instance.SaveInfo();
            LocalizationManager.Instance.SetCurrentLanguage(SettingsManager.Instance.GetCurrentLanguageID());
        }

        public void OnChangedTranslationKeyField(string text)
        {
            ChangeTranslation(editingTranslationKey, text);
        }

        public void OnToLowerButtonClicked()
        {
            OnChangedTranslationKeyField(editingTranslationKey.ToLower());
        }

        public void OnDeleteTranslationButtonClicked()
        {
            ModUIUtils.MessagePopup(true, "Delete translation?", $"\"{editingTranslationKey}\" will be deleted.", 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes, delete", "No", null, delegate
            {
                DeleteTranslation(editingTranslationKey);
            });
        }

        public void OnSearchBoxChanged(string text)
        {
            string lowerText = text.ToLower();
            bool forceSetEnabled = text.IsNullOrEmpty();

            foreach (KeyValuePair<string, GameObject> keyValue in _cachedInstantiatedKeyDisplays)
            {
                if (forceSetEnabled)
                {
                    keyValue.Value.SetActive(true);
                }
                else
                {
                    keyValue.Value.SetActive(keyValue.Key.ToLower().Contains(lowerText));
                }
            }
        }
    }
}
