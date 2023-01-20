using CloneDroneOverhaul.Modules;
using System;
using System.Collections.Generic;
using System.IO;

namespace CloneDroneOverhaul.Localization
{
    /// <summary>
    /// Mod translation controller 'n' storage
    /// </summary>
    public class LocalizationController : V3.Base.V3_ModControllerBase
    {
        public static string TranslationsFolder => OverhaulDescription.GetModFolder() + "Localizations/";

        private List<TranslationEntry> _entries = new List<TranslationEntry>();

        void Awake()
        {
            LoadTranslations();
        }

        public void LoadTranslations()
        {
            _entries = File.Exists(LocalizationController.TranslationsFolder + "Locals.cdoloc") ? V3.Utilities.OverhaulUtilities.ByteSaver.FromByteArray<List<TranslationEntry>>(File.ReadAllBytes(LocalizationController.TranslationsFolder + "Locals.cdoloc")) : new List<TranslationEntry>();
        }

        public TranslationEntry MakeNewTranslation()
        {
            TranslationEntry entry = new TranslationEntry
            {
                Translations = new Dictionary<string, string>()
            };
            foreach (string str in GetAllLanguageCodes())
            {
                entry.Translations.Add(str, "nontranslated");
            }
            _entries.Add(entry);
            return entry;
        }

        public List<TranslationEntry> GetAllTranslations()
        {
            return _entries;
        }

        public TranslationEntry GetTranslation(in string id)
        {
            foreach (TranslationEntry entry in _entries)
            {
                if (entry.ID == id)
                {
                    return entry;
                }
            }
            return null;
        }

        public void SaveAllTranslations()
        {
            byte[] array = V3.Utilities.OverhaulUtilities.ByteSaver.ObjectToByteArray(_entries);
            File.WriteAllBytes(LocalizationController.TranslationsFolder + "Locals.cdoloc", array);
        }

        public List<string> GetAllLanguageCodes()
        {
            List<string> strings = new List<string>();
            foreach (LanguageDefinition def in LocalizationManager.Instance.SupportedLanguages)
            {
                strings.Add(def.LanguageCode);
            }
            return strings;
        }
    }

    [Serializable]
    public class TranslationEntry
    {
        public string ID;
        public Dictionary<string, string> Translations;
    }
}
