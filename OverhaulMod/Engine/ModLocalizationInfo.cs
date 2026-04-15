using OverhaulMod.Utils;
using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class ModLocalizationInfo
    {
        public Dictionary<string, string> Translations_en;
        public Dictionary<string, string> Translations_ru;
        public Dictionary<string, string> Translations_zhcn;
        public Dictionary<string, string> Translations_zhtw;

        public Dictionary<string, string> GetDictionary(string langId)
        {
            switch (langId)
            {
                case ModConstants.LANG_CODE_RU:
                    return Translations_ru;
                case ModConstants.LANG_CODE_ZH_CN:
                    return Translations_zhcn;
                case ModConstants.LANG_CODE_ZH_TW:
                    return Translations_zhtw;
            }
            return Translations_en;
        }

        public void AddTranslation(string key)
        {
            AddKey(ModConstants.LANG_CODE_EN, key);
            AddKey(ModConstants.LANG_CODE_RU, key);
            AddKey(ModConstants.LANG_CODE_ZH_CN, key);
            AddKey(ModConstants.LANG_CODE_ZH_TW, key);
        }

        public void ChangeTranslation(string oldName, string newName)
        {
            ChangeKey(ModConstants.LANG_CODE_EN, oldName, newName);
            ChangeKey(ModConstants.LANG_CODE_RU, oldName, newName);
            ChangeKey(ModConstants.LANG_CODE_ZH_CN, oldName, newName);
            ChangeKey(ModConstants.LANG_CODE_ZH_TW, oldName, newName);
        }

        public void DeleteTranslation(string key)
        {
            DeleteKey(ModConstants.LANG_CODE_EN, key);
            DeleteKey(ModConstants.LANG_CODE_RU, key);
            DeleteKey(ModConstants.LANG_CODE_ZH_CN, key);
            DeleteKey(ModConstants.LANG_CODE_ZH_TW, key);
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

            if (Translations_zhcn == null)
                Translations_zhcn = new Dictionary<string, string>();

            if (Translations_zhtw == null)
                Translations_zhtw = new Dictionary<string, string>();
        }

        public void FillTranslations()
        {
            if (Translations_zhcn.Count == 0) Translations_zhcn = new Dictionary<string, string>(Translations_en);
            if (Translations_zhtw.Count == 0) Translations_zhtw = new Dictionary<string, string>(Translations_en);
        }
    }
}