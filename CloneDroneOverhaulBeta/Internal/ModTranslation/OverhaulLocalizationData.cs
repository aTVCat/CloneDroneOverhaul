﻿using ModLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using System.IO;
using System;
using static Sony.NP.Core;

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

        public void AddTranslation(string translationID)
        {
            foreach(string key in Translations.Keys)
            {
                if (!Translations[key].ContainsKey(translationID))
                {
                    Translations[key].Add(translationID, string.Empty);
                }
            }
        }

        public string GetTranslation(string langID, string translationID)
        {
            if (!Translations.ContainsKey(langID))
            {
                return langID + "_" + translationID;
            }

            if (string.IsNullOrEmpty(Translations[langID][translationID]))
            {
                return Translations["en"][translationID];
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
            GetTranslation(text, mObj.ID.Substring(4));
        }

        public void ReplaceTranslation(string id, string newId)
        {
            foreach (string key in Translations.Keys)
            {
                if (Translations[key].ContainsKey(id))
                {
                    string old = Translations[key][id];
                    Translations[key].Remove(id);
                    Translations[key].Add(newId, old);
                }
            }
        }

        public void RemoveTranslation(string id)
        {
            foreach (string key in Translations.Keys)
            {
                Translations[key].Remove(id);
            }
        }
    }
}