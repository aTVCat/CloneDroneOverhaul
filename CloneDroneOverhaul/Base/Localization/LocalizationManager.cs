using CloneDroneOverhaul.Modules;
using System;
using System.Collections.Generic;
using System.IO;

namespace CloneDroneOverhaul.Localization
{
    public class OverhaulLocalizationManager : ModuleBase
    {
        public static string TranslationsFolder
        {
            get
            {
                return OverhaulDescription.GetModFolder() + "Localizations/";
            }
        }

        List<TranslationEntry> entries = new List<TranslationEntry>();

        public override void Start()
        {
            LoadTranslations();
        }

        public void LoadTranslations()
        {
            entries = File.Exists(OverhaulLocalizationManager.TranslationsFolder + "Locals.cdoloc") ? CloneDroneOverhaul.ByteSaver.FromByteArray<List<TranslationEntry>>(File.ReadAllBytes(OverhaulLocalizationManager.TranslationsFolder + "Locals.cdoloc")) : new List<TranslationEntry>();
        }

        public TranslationEntry MakeNewTranslation()
        {
            TranslationEntry entry = new TranslationEntry();
            entry.Translations = new Dictionary<string, string>();
            foreach (string str in GetAllLanguageCodes())
            {
                entry.Translations.Add(str, "nontranslated");
            }
            entries.Add(entry);
            return entry;
        }

        public List<TranslationEntry> GetAllTranslations()
        {
            return entries;
        }

        public TranslationEntry GetTranslation(in string id)
        {
            foreach (TranslationEntry entry in entries)
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
            byte[] array = CloneDroneOverhaul.ByteSaver.ObjectToByteArray(entries);
            File.WriteAllBytes(OverhaulLocalizationManager.TranslationsFolder + "Locals.cdoloc", array);
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
