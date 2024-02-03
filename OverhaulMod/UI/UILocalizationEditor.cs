using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UILocalizationEditor : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button m_saveButton;

        [UIElementAction(nameof(OnLanguagesDropdownChanged))]
        [UIElement("LanguagesDropdown")]
        private readonly Dropdown m_languagesDropdown;

        [UIElement("TranslationKeyDisplay", false)]
        private readonly ModdedObject m_translationKeyPrefab;
        [UIElement("AddTranslationKeyButton", false)]
        private readonly Button m_addTranslationKeyButtonPrefab;
        [UIElement("TextPrefab", false)]
        private readonly Text m_textPrefab;
        [UIElement("TranslationKeysListContainer")]
        private readonly Transform m_translationKeysContainer;
        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicatorObject;

        [UIElement("EditingKeyText")]
        private readonly Text m_translationKeyLabel;
        [UIElement("EditingValueInputField")]
        private readonly InputField m_translationValueInputField;

        [UIElement("ArialRText")]
        private readonly Text m_arialRFontPreviewText;
        [UIElement("ArialBText")]
        private readonly Text m_arialBFontPreviewText;
        [UIElement("OSRText")]
        private readonly Text m_openSansRFontPreviewText;
        [UIElement("OSBText")]
        private readonly Text m_openSansBFontPreviewText;
        [UIElement("EditUndoText")]
        private readonly Text m_editUndoFontPreviewText;
        [UIElement("PixelsSimpleText")]
        private readonly Text m_pixelsSimpleFontPreviewText;
        [UIElement("TriggeringFanfaresText")]
        private readonly Text m_triggeringFanfaresFontPreviewText;

        private bool m_isPopulatingTranslations;

        public override bool hideTitleScreen => true;

        public string editingLangId
        {
            get
            {
                return m_languagesDropdown.options[m_languagesDropdown.value].text;
            }
        }

        public string editingTranslationKey
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            m_languagesDropdown.options = ModLocalizationManager.Instance.GetLanguageOptions(true);
            m_languagesDropdown.value = 0;
            m_translationValueInputField.onEndEdit.AddListener(delegate (string str)
            {
                if (string.IsNullOrEmpty(editingLangId) || string.IsNullOrEmpty(editingTranslationKey))
                    return;

                ModLocalizationManager.Instance.SetTranslation(editingLangId, editingTranslationKey, str);
            });
            m_translationValueInputField.onValueChanged.AddListener(delegate
            {
                RefreshPreview();
            });

            PopulateTranslations();
        }

        public override void OnDisable()
        {
            m_isPopulatingTranslations = false;
            m_loadingIndicatorObject.SetActive(false);
        }

        public void EditTranslationKey(string key)
        {
            editingTranslationKey = key;

            m_translationKeyLabel.text = key;
            m_translationValueInputField.text = ModLocalizationManager.Instance.GetTranslation(editingLangId, editingTranslationKey);
            RefreshPreview();
        }

        public void RefreshPreview()
        {
            string text = m_translationValueInputField.text;

            m_arialRFontPreviewText.text = text;
            m_arialBFontPreviewText.text = text;
            m_openSansRFontPreviewText.text = text;
            m_openSansBFontPreviewText.text = text;
            m_editUndoFontPreviewText.text = text;
            m_pixelsSimpleFontPreviewText.text = text;
            m_triggeringFanfaresFontPreviewText.text = text;
        }

        public ModdedObject InstantiateTranslationKeyDisplay(string key)
        {
            ModdedObject translationKey = Instantiate(m_translationKeyPrefab, m_translationKeysContainer);
            translationKey.gameObject.SetActive(true);
            translationKey.GetObject<Text>(0).text = key;
            Button button = translationKey.GetComponent<Button>();
            button.onClick.AddListener(delegate
            {
                EditTranslationKey(key);
            });
            return translationKey;
        }

        public void PopulateTranslations()
        {
            if (m_isPopulatingTranslations || !visibleInHierarchy)
                return;

            m_isPopulatingTranslations = true;
            if (m_translationKeysContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_translationKeysContainer);

            _ = base.StartCoroutine(populateTranslationsCoroutine());
        }

        private IEnumerator populateTranslationsCoroutine()
        {
            ModLocalizationManager manager = ModLocalizationManager.Instance;
            if (!manager.CanLanguageBeTranslated(editingLangId))
            {
                Text text = Instantiate(m_textPrefab, m_translationKeysContainer);
                text.gameObject.SetActive(true);
                text.text = "This language cannot be translated";

                m_isPopulatingTranslations = false;
                yield break;
            }

            Button addTranslationButton = Instantiate(m_addTranslationKeyButtonPrefab, m_translationKeysContainer);
            addTranslationButton.gameObject.SetActive(true);
            addTranslationButton.onClick.AddListener(OnAddTranslationButtonClicked);

            Dictionary<string, string> translationDictionary = manager.GetTranslationDictionary(editingLangId);
            if (translationDictionary != null && translationDictionary.Count != 0)
            {
                m_translationKeysContainer.gameObject.SetActive(false);
                m_loadingIndicatorObject.SetActive(true);

                int index = 0;
                foreach (string key in translationDictionary.Keys)
                {
                    _ = InstantiateTranslationKeyDisplay(key);
                    if (index % 5 == 0)
                        yield return null;
                }

                m_translationKeysContainer.gameObject.SetActive(true);
                yield return null;

                addTranslationButton.transform.SetAsLastSibling();
                m_loadingIndicatorObject.SetActive(false);
            }
            m_isPopulatingTranslations = false;
            yield break;
        }

        public void OnLanguagesDropdownChanged(int index)
        {
            PopulateTranslations();
            if (!string.IsNullOrEmpty(editingTranslationKey))
            {
                EditTranslationKey(editingTranslationKey);
            }
        }

        public void OnAddTranslationButtonClicked()
        {
            if (m_isPopulatingTranslations)
                return;

            ModUIUtils.InputFieldWindow("Add translation", "Enter non-existing translation name", 125f, delegate (string value)
            {
                if (m_isPopulatingTranslations)
                    return;

                ModLocalizationManager.Instance.AddTranslation(value);
                ModdedObject moddedObject = InstantiateTranslationKeyDisplay(value);
                moddedObject.transform.SetSiblingIndex(m_translationKeysContainer.childCount - 2);
                EditTranslationKey(value);
            });
        }

        public void OnSaveButtonClicked()
        {
            ModLocalizationManager.Instance.SaveInfo();
            GlobalEventManager.Instance.Dispatch(GlobalEvents.UILanguageChanged);
        }
    }
}
