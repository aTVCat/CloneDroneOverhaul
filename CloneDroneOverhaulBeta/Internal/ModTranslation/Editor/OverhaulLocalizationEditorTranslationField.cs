using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CDOverhaul.Localization
{
    public class OverhaulLocalizationEditorTranslationField : OverhaulBehaviour
    {
        public string MyLang;
        public string MyID;

        public InputField MyInputField;
        public InputField MyIDInputField;

        public void Initialize(string lang, string id, InputField field, InputField idField)
        {
            MyID = id;
            MyLang = lang;
            MyInputField = field;
            MyIDInputField = idField;

            bool success = OverhaulLocalizationController.Localization.Translations[MyLang].TryGetValue(MyID, out string text);
            if (!success)
            {
                return;
            }

            MyInputField.text = text;
            MyInputField.onEndEdit.AddListener(UpdateText);
            MyIDInputField.text = id;
            MyIDInputField.onEndEdit.AddListener(UpdateID);
        }

        protected override void OnDisposed()
        {
            MyLang = null;
            MyID = null;
            MyLang = null;
        }

        public void UpdateText(string str)
        {
            if (!OverhaulLocalizationController.Localization.Translations[MyLang].ContainsKey(MyID))
            {
                return;
            }
            OverhaulLocalizationController.Localization.Translations[MyLang][MyID] = str;
        }

        public void UpdateID(string str)
        {
            string prevString = OverhaulLocalizationController.Localization.Translations[MyLang][MyID];
            OverhaulLocalizationController.Localization.Translations[MyLang].Remove(MyID);
            OverhaulLocalizationController.Localization.Translations[MyLang].Add(str, prevString);
            MyID = str;
        }
    }
}