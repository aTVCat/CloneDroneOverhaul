using UnityEngine.UI;

namespace CDOverhaul
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

            if (!OverhaulLocalizationController.Localization.Translations[MyLang].TryGetValue(MyID, out string text))
                return;

            MyInputField = field;
            MyInputField.text = text;
            MyInputField.onEndEdit.AddListener(UpdateText);
            MyIDInputField = idField;
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
                return;

            OverhaulLocalizationController.Localization.Translations[MyLang][MyID] = str;
        }

        public void UpdateID(string str)
        {
            OverhaulLocalizationController.Localization.ReplaceTranslation(MyID, str);
            MyID = str;
        }
    }
}