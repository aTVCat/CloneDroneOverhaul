﻿using System.Collections;
using BestHTTP.SocketIO;
using CDOverhaul;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Localization
{
    public class OverhaulLocalizationEditor : OverhaulUI
    {
        private bool m_HasPopulated;
        private string m_EditingLang;

        public override void Initialize()
        {
            MyModdedObject.GetObject<Button>(0).onClick.AddListener(Hide);
            MyModdedObject.GetObject<Button>(6).onClick.AddListener(EndEditingLang);
            MyModdedObject.GetObject<Button>(8).onClick.AddListener(NewTranslation);

            Hide();
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            bool error = OverhaulLocalizationController.Error;

            MyModdedObject.GetObject<Transform>(3).gameObject.SetActive(error);
            if (error)
            {
                return;
            }

            if (!m_HasPopulated)
            {
                foreach (string langCode in OverhaulLocalizationController.Localization.Translations.Keys)
                {
                    ModdedObject m = Instantiate(MyModdedObject.GetObject<ModdedObject>(1), MyModdedObject.GetObject<Transform>(2));
                    m.gameObject.SetActive(true);
                    m.GetObject<Text>(0).text = langCode;
                    m.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        EditLang(langCode);
                    });
                }

                m_HasPopulated = true;
            }

            TitleScreenUI tUI = GameUIRoot.Instance.TitleScreenUI;
            if (tUI.gameObject.activeSelf)
            {
                tUI.SetLogoAndRootButtonsVisible(false);
            }
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);

            TitleScreenUI tUI = GameUIRoot.Instance.TitleScreenUI;
            if (tUI.gameObject.activeSelf)
            {
                tUI.SetLogoAndRootButtonsVisible(true);
            }
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
            }
        }

        public void EndEditingLang()
        {
            m_EditingLang = string.Empty;
            MyModdedObject.GetObject<Transform>(7).gameObject.SetActive(false);
        }

        public void NewTranslation()
        {
            if (string.IsNullOrEmpty(m_EditingLang))
            {
                return;
            }

            OverhaulLocalizationController.Localization.AddTranslation("SampleTranslation");
            EditLang(m_EditingLang);
        }

#if DEBUG
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                OverhaulLocalizationController.SaveData();
            }
        }
#endif
    }
}