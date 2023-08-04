using System;
using System.Reflection;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorPropertyAttribute : Attribute
    {
        public string Category;

        public FieldInfo FieldReference;
        public bool UseAltDisplay;
        public object AdditionalParameters;

        public PersonalizationEditorPropertyAttribute(string name, bool useAlternateVariant = false, object additParams = null)
        {
            Category = name;
            UseAltDisplay = useAlternateVariant;
            AdditionalParameters = additParams;
        }
    }
}
