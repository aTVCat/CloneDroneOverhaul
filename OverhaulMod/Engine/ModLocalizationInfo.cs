using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class ModLocalizationInfo
    {
        public Dictionary<string, string> Translations_en;
        public Dictionary<string, string> Translations_ru;

        public Dictionary<string, string> GetDictionary(string langId)
        {
            switch (langId)
            {
                case "en":
                    return Translations_en;
                case "ru":
                    return Translations_ru;
            }
            return null;
        }

        public void AddTranslation(string key)
        {
            AddKey("en", key);
            AddKey("ru", key);
        }

        public void ChangeTranslation(string oldName, string newName)
        {
            ChangeKey("en", oldName, newName);
            ChangeKey("ru", oldName, newName);
        }

        public void DeleteTranslation(string key)
        {
            DeleteKey("en", key);
            DeleteKey("ru", key);
        }

        public void AddKey(string langId, string key)
        {
            Dictionary<string, string> d = GetDictionary(langId);
            if (d == null || d.ContainsKey(key))
                return;

            d.Add(key, string.Empty);
        }

        public void ChangeKey(string langId, string oldName, string newName)
        {
            Dictionary<string, string> d = GetDictionary(langId);
            if (d == null || !d.ContainsKey(oldName))
                return;

            string value = d[oldName];
            if (d.Remove(oldName))
            {
                d.Add(newName, value);
            }
        }

        public void DeleteKey(string langId, string key)
        {
            Dictionary<string, string> d = GetDictionary(langId);
            if (d == null || !d.ContainsKey(key))
                return;

            _ = d.Remove(key);
        }

        public void FixValues()
        {
            if (Translations_en == null)
                Translations_en = new Dictionary<string, string>();

            if (Translations_ru == null)
                Translations_ru = new Dictionary<string, string>();
        }
    }
}
