using UnityEngine.UI;

namespace CDOverhaul.DevTools
{
    public class OverhaulJsonEditorEntryDisplay : OverhaulBehaviour
    {
        public InputField InputField;
        public InputField KeyInputField;

        public OverhaulJsonObject EditingObject;
        public string EditingKey;

        public override void Start()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            KeyInputField = moddedObject.GetObject<InputField>(0);
            KeyInputField.onEndEdit.AddListener(onKeyNameUpdated);
            InputField = moddedObject.GetObject<InputField>(1);
            InputField.onEndEdit.AddListener(onValueUpdated);
        }

        private void onValueUpdated(string newText)
        {
            EditingObject.Values[EditingKey] = newText;
        }

        private void onKeyNameUpdated(string newText)
        {
            string text = EditingObject.Values[EditingKey];
            _ = EditingObject.Values.Remove(EditingKey);
            EditingObject.Values.Add(newText, text);
            EditingKey = newText;
        }
    }
}
