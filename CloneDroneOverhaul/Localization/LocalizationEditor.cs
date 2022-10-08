using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.UI;
using CloneDroneOverhaul;

namespace CloneDroneOverhaul.Localization
{
    public class OverhaulLocalizationEditor : ModGUIBase
    {
        private OverhaulLocalizationManager OurModule;

        private Button NewLocalButton;
        private Button SaveLocalsButton;
        private Button RefreshAllTranslationsButton;
        private Button RemoveButton;

        private RectTransform LocalPrefab;
        private RectTransform LocalsContainer;

        private InputField LocalNameIF;
        private RectTransform LangEntryPrefab;
        private RectTransform LangContainer;

        private Text Debug;

        public string EditingLocal;
        public string EditingLang;

        public override void OnInstanceStart()
        {
            OurModule = BaseStaticReferences.ModuleManager.GetModule<OverhaulLocalizationManager>();
            MyModdedObject = base.GetComponent<ModdedObject>();

            NewLocalButton = MyModdedObject.GetObjectFromList<Button>(0);
            NewLocalButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnNewTranslationClick));
            SaveLocalsButton = MyModdedObject.GetObjectFromList<Button>(1);
            SaveLocalsButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OurModule.SaveAllTranslations));
            RefreshAllTranslationsButton = MyModdedObject.GetObjectFromList<Button>(7);
            RefreshAllTranslationsButton.onClick.AddListener(new UnityEngine.Events.UnityAction(RefreshAvailableLocals));
            RemoveButton = MyModdedObject.GetObjectFromList<Button>(9);
            RemoveButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnRemoveSelectedTranslationClick));
            LocalPrefab = MyModdedObject.GetObjectFromList<RectTransform>(2);
            LocalsContainer = MyModdedObject.GetObjectFromList<RectTransform>(3);

            LocalNameIF = MyModdedObject.GetObjectFromList<InputField>(6);
            LocalNameIF.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(OnLocalNameChanged));
            LangEntryPrefab = MyModdedObject.GetObjectFromList<RectTransform>(5);
            LangContainer = MyModdedObject.GetObjectFromList<RectTransform>(4);

            Debug = MyModdedObject.GetObjectFromList<Text>(8);

            base.gameObject.SetActive(false);
        }

        public void RefreshAvailableLocals()
        {
            TransformUtils.DestroyAllChildren(LocalsContainer);
            List<TranslationEntry> entries = new List<TranslationEntry>(OurModule.GetAllTranslations());
            foreach (TranslationEntry entry in entries)
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
                BaseStaticReferences.ModuleManager.GetModule<GUIManagement>().GetGUI<OverhaulLocalizationEditor>().OnEditButtonClicked(this.MyID);
            }
        }

        private class UITranslationEntry : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
        {
            public string MyLangID;
            public OverhaulLocalizationEditor UI;

            public void OnPointerClick(UnityEngine.EventSystems.PointerEventData data)
            {
                BaseStaticReferences.ModuleManager.GetModule<GUIManagement>().GetGUI<OverhaulLocalizationEditor>().EditingLang = MyLangID;
            }

            void FixedUpdate()
            {
                ModdedObject mObj = base.GetComponent<ModdedObject>();
                mObj.GetObjectFromList<InputField>(1).interactable = UI.EditingLang == mObj.GetObjectFromList<Text>(0).text;
            }
        }

        public void OnEditButtonClicked(string ID)
        {
            TranslationEntry entry = OurModule.GetTranslation(ID);
            LocalNameIF.text = ID;
            EditingLocal = " - 1 ";

            TransformUtils.DestroyAllChildren(this.LangContainer);
            foreach (string str in OurModule.GetAllLanguageCodes())
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
            TranslationEntry entry = OurModule.MakeNewTranslation();
            entry.ID = "NewLocal";
            EditingLocal = entry.ID;
            OnEditButtonClicked(entry.ID);
            RefreshAvailableLocals();
        }

        private void OnLocalNameChanged(string name)
        {
            TranslationEntry entry = OurModule.GetTranslation(EditingLocal);
            entry.ID = name;
            EditingLocal = name;
            RefreshAvailableLocals();
        }

        private void OnTranslationChanged(string name)
        {
            TranslationEntry entry = OurModule.GetTranslation(EditingLocal);
            if (entry != null && !string.IsNullOrEmpty(EditingLang))
            {
                entry.Translations[EditingLang] = name;
            }
        }

        private void OnRemoveSelectedTranslationClick()
        {
            OurModule.GetAllTranslations().Remove(OurModule.GetTranslation(EditingLocal));
            TransformUtils.DestroyAllChildren(this.LangContainer);
            RefreshAvailableLocals();
        }

        public override void OnManagedUpdate()
        {
            Debug.text = "Selected localization: " + EditingLocal + System.Environment.NewLine + "Selected lang: " + EditingLang;
        }

        public void TryShow()
        {
            base.gameObject.SetActive(!base.gameObject.activeInHierarchy);
        }
    }
}
