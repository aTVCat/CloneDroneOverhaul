﻿using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulLocalizationEditor : OverhaulUI
    {
        private bool m_HasPopulated;
        private string m_EditingLang;

        public override void Initialize()
        {
            MyModdedObject.GetObject<Transform>(4).gameObject.SetActive(false);
            MyModdedObject.GetObject<Button>(0).onClick.AddListener(Hide);
            MyModdedObject.GetObject<Button>(6).onClick.AddListener(EndEditingLang);
            MyModdedObject.GetObject<Button>(8).onClick.AddListener(NewTranslation);
            MyModdedObject.GetObject<Button>(9).onClick.AddListener(OverhaulLocalizationController.SaveData);
            MyModdedObject.GetObject<InputField>(10).onValueChanged.AddListener(Search);

            Hide();
        }

        private void Update()
        {
            OverhaulLocalizationController.UpdateLoadingScreen();
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            bool error = OverhaulLocalizationController.Error;

            MyModdedObject.GetObject<Transform>(3).gameObject.SetActive(error);
            if (error)
                return;

            if (!m_HasPopulated)
            {
                foreach (string langCode in OverhaulLocalizationController.Localization.Translations.Keys)
                {
                    ModdedObject m = Instantiate(MyModdedObject.GetObject<ModdedObject>(1), MyModdedObject.GetObject<Transform>(2));
                    m.gameObject.SetActive(true);
                    m.GetObject<Text>(0).text = langCode;
                    m.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        EditLang(m.GetObject<Text>(0).text);
                    });
                }
                m_HasPopulated = true;
            }

            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);

            TitleScreenUI tUI = GameUIRoot.Instance.TitleScreenUI;
            if (tUI.gameObject.activeSelf)
                tUI.SetLogoAndRootButtonsVisible(true);
        }

        public void EditLang(string lang)
        {
            m_EditingLang = lang;
            MyModdedObject.GetObject<Transform>(7).gameObject.SetActive(true);

            TransformUtils.DestroyAllChildren(MyModdedObject.GetObject<Transform>(5));
            foreach (string str in OverhaulLocalizationController.Localization.Translations[lang].Keys)
            {
                ModdedObject m = Instantiate(MyModdedObject.GetObject<ModdedObject>(4), MyModdedObject.GetObject<Transform>(5));
                m.gameObject.SetActive(true);
                m.gameObject.AddComponent<OverhaulLocalizationEditorTranslationField>().Initialize(lang, str, m.GetObject<InputField>(0), m.GetObject<InputField>(1));
                m.GetObject<Button>(2).onClick.AddListener(delegate
                {
                    OverhaulLocalizationController.Localization.RemoveTranslation(str);
                    EditLang(lang);
                });
            }
        }

        public void Search(string text)
        {
            int i = 0;
            do
            {
                OverhaulLocalizationEditorTranslationField field = OverhaulLocalizationEditorTranslationField.Fields[i];
                if (field)
                {
                    field.gameObject.SetActive(string.IsNullOrEmpty(text) || field.MyID.Contains(text));
                }
                i++;
            } while (i < OverhaulLocalizationEditorTranslationField.Fields.Count);
        }

        public void EndEditingLang()
        {
            m_EditingLang = string.Empty;
            MyModdedObject.GetObject<Transform>(7).gameObject.SetActive(false);
        }

        public void NewTranslation()
        {
            if (string.IsNullOrEmpty(m_EditingLang))
                return;

            OverhaulLocalizationController.Localization.AddTranslation("CoolThingTranslation");
            EditLang(m_EditingLang);
        }
    }
}