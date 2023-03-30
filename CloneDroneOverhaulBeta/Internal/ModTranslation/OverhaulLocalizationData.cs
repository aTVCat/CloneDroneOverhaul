using ModLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using System.IO;
using System;

namespace CDOverhaul.Localization
{
    [Serializable]
    public class OverhaulLocalizationData : OverhaulDataBase
    {
        public Dictionary<string, Dictionary<string, string>> Translations;

        public override void RepairFields()
        {
            if(Translations.IsNullOrEmpty())
            {
                Translations = new Dictionary<string, Dictionary<string, string>>();

                foreach(LanguageDefinition def in LocalizationManager.Instance.SupportedLanguages)
                {
                    AddLanguage(def.LanguageCode);
                }
            }
        }

        public void SaveLocalization()
        {
            OverhaulLocalizationController.SaveData();
        }

        public void AddLanguage(string langID)
        {
            Translations.Add(langID, new Dictionary<string, string>());
        }

        public void AddTranslation(string langID, string translationID)
        {
            if (!Translations.ContainsKey(langID))
            {
                return;
            }

            if (Translations[langID].ContainsKey(translationID))
            {
                return;
            }

            Translations[langID].Add(translationID, string.Empty);
        }

        public string GetTranslation(string langID, string translationID)
        {
            if (!Translations.ContainsKey(langID))
            {
                return string.Empty;
            }

            if (!Translations[langID].ContainsKey(translationID))
            {
                return string.Empty;
            }

            return Translations[langID][translationID];
        }

        public string GetTranslation(string translationID)
        {
            SettingsManager m = SettingsManager.Instance;
            if (m == null || m.GetPrivateField<SettingsData>("_data") == null)
            {
                return "nl: " + translationID;
            }

            string langCode = m.GetCurrentLanguageID();
            if (string.IsNullOrEmpty(langCode))
            {
                return "nl: " + translationID;
            }

            return GetTranslation(langCode, translationID);
        }

        public void GetTranslation(Text textElement, string translationID)
        {
            if(textElement == null)
            {
                return;
            }

            textElement.text = GetTranslation(translationID);
        }

        public void GetTranslation(ModdedObject mObj, bool checkComponent = true)
        {
            if (mObj == null)
            {
                return;
            }

            if(checkComponent && (string.IsNullOrEmpty(mObj.ID) || mObj.ID.Length < 5 || !mObj.ID.Contains("LID_")))
            {
                return;
            }

            Text text = mObj.GetComponent<Text>();
            if(checkComponent && text == null)
            {
                return;
            }

            text.text = mObj.ID.Substring(4);
        }
    }
}