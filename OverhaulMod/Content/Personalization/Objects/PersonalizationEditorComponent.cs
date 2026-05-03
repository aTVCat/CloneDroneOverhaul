using OverhaulMod.UI;
using UnityEngine;

namespace OverhaulMod.Content.Personalization.Objects
{
    public class PersonalizationEditorComponent : MonoBehaviour
    {
        private PersonalizationEditorPlacedObject _placedObject;
        public PersonalizationEditorPlacedObject PlacedObject
        {
            get
            {
                if (!_placedObject) _placedObject = base.GetComponent<PersonalizationEditorPlacedObject>();
                return _placedObject;
            }
        }

        public void SetPropertyValue(string className, string fieldName, object value) => PlacedObject.SetPropertyValue(className, fieldName, value);

        public T GetPropertyValue<T>(string className, string fieldName, T defaultValue) => PlacedObject.GetPropertyValue(className, fieldName, defaultValue);

        public virtual string GetDisplayName() => GetType().Name;

        public virtual Sprite GetIcon() => null;

        public virtual void InstantiateSettingsForInspector(UIElementPEInspectorGroup group, UIElementPEInspectorFieldsStore fieldsStore) { }
    }
}