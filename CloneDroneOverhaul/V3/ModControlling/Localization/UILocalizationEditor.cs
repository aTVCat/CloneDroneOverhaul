using CloneDroneOverhaul.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul
{
    public class UILocalizationEditor : V3.HUD.V3_ModHUDBase
    {
        private Localization.LocalizationController _controller;

        private Button NewLocalButton;
        private Button SaveLocalsButton;
        private Button RefreshAllTranslationsButton;
        private Button RemoveButton;

        private RectTransform LocalPrefab;
        private RectTransform LocalsContainer;

        private InputField LocalNameIF;
        private RectTransform LangEntryPrefab;
        private RectTransform LangContainer;

        public string EditingLocal;
        public string EditingLang;

        void Start()
        {
            _controller = Localization.LocalizationController.GetInstance<Localization.LocalizationController>();

            NewLocalButton = MyModdedObject.GetObjectFromList<Button>(0);
            NewLocalButton.onClick.AddListener(OnNewTranslationClick);

            SaveLocalsButton = MyModdedObject.GetObjectFromList<Button>(1);
            SaveLocalsButton.onClick.AddListener(_controller.SaveAllTranslations);

            RefreshAllTranslationsButton = MyModdedObject.GetObjectFromList<Button>(7);
            RefreshAllTranslationsButton.onClick.AddListener(RefreshAvailableLocals);

            RemoveButton = MyModdedObject.GetObjectFromList<Button>(9);
            RemoveButton.onClick.AddListener(OnRemoveSelectedTranslationClick);

            LocalPrefab = MyModdedObject.GetObjectFromList<RectTransform>(2);
            LocalsContainer = MyModdedObject.GetObjectFromList<RectTransform>(3);

            LocalNameIF = MyModdedObject.GetObjectFromList<InputField>(6);
            LocalNameIF.onEndEdit.AddListener(OnLocalNameChanged);
            LangEntryPrefab = MyModdedObject.GetObjectFromList<RectTransform>(5);
            LangContainer = MyModdedObject.GetObjectFromList<RectTransform>(4);

            base.gameObject.SetActive(false);
        }

        public void RefreshAvailableLocals()
        {
            TransformUtils.DestroyAllChildren(LocalsContainer);
            foreach (Localization.TranslationEntry entry in _controller.GetAllTranslations())
            {
                RectTransform trans = Instantiate<RectTransform>(LocalPrefab, LocalsContainer);
                ModdedObject mObj = trans.GetComponent<ModdedObject>();
                mObj.GetObjectFromList<Text>(0).text = entry.ID;
                mObj.GetObjectFromList<Button>(1).gameObject.AddComponent<UILocalizationEntry>().MyID = entry.ID;
                trans.gameObject.SetActive(true);
            }
        }

        private class UILocalizationEntry : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
        {
            public string MyID;

            public void OnPointerClick(UnityEngine.EventSystems.PointerEventData data)
            {
                UILocalizationEditor.GetInstance<UILocalizationEditor>().OnEditButtonClicked(MyID);
            }
        }

        private class UITranslationEntry : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
        {
            public string MyLangID;
            public UILocalizationEditor UI;

            public void OnPointerClick(UnityEngine.EventSystems.PointerEventData data)
            {
                UILocalizationEditor.GetInstance<UILocalizationEditor>().EditingLang = MyLangID;
            }

            private void Update()
            {
                ModdedObject mObj = base.GetComponent<ModdedObject>();
                mObj.GetObjectFromList<InputField>(1).interactable = UI.EditingLang == mObj.GetObjectFromList<Text>(0).text;
            }
        }

        public void OnEditButtonClicked(string ID)
        {
            Localization.TranslationEntry entry = _controller.GetTranslation(ID);
            LocalNameIF.text = ID;
            EditingLocal = " - 1 ";

            TransformUtils.DestroyAllChildren(LangContainer);
            foreach (string str in _controller.GetAllLanguageCodes())
            {
                RectTransform trans = Instantiate<RectTransform>(LangEntryPrefab, LangContainer);
                trans.gameObject.SetActive(true);
                UITranslationEntry ui = trans.gameObject.AddComponent<UITranslationEntry>();
                ui.MyLangID = str;
                ui.UI = this;
                ModdedObject mObj = trans.GetComponent<ModdedObject>();
                mObj.GetObjectFromList<Text>(0).text = str;
                mObj.GetObjectFromList<InputField>(1).text = entry.Translations.ContainsKey(str) ? entry.Translations[str] : "...";
                mObj.GetObjectFromList<InputField>(1).onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(OnTranslationChanged));
            }
            EditingLocal = ID;
        }

        public void OnNewTranslationClick()
        {
            Localization.TranslationEntry entry = _controller.MakeNewTranslation();
            entry.ID = "Localization";
            EditingLocal = entry.ID;
            OnEditButtonClicked(entry.ID);
            RefreshAvailableLocals();
        }

        private void OnLocalNameChanged(string name)
        {
            Localization.TranslationEntry entry = _controller.GetTranslation(EditingLocal);
            entry.ID = name;
            EditingLocal = name;
            RefreshAvailableLocals();
        }

        private void OnTranslationChanged(string name)
        {
            Localization.TranslationEntry entry = _controller.GetTranslation(EditingLocal);
            if (entry != null && !string.IsNullOrEmpty(EditingLang))
            {
                entry.Translations[EditingLang] = name;
            }
        }

        private void OnRemoveSelectedTranslationClick()
        {
            _controller.GetAllTranslations().Remove(_controller.GetTranslation(EditingLocal));
            TransformUtils.DestroyAllChildren(LangContainer);
            RefreshAvailableLocals();
        }

        public void TryShow()
        {
            base.gameObject.SetActive(!base.gameObject.activeInHierarchy);
        }
    }
}
