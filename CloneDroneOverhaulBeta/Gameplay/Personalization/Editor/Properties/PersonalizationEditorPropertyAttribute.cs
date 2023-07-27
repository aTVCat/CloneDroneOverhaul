using System;
using System.Reflection;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorPropertyAttribute : Attribute
    {
        public string Category;

        public FieldInfo FieldReference;

        public PersonalizationEditorPropertyAttribute(string name)
        {
            Category = name;
        }
    }
}
