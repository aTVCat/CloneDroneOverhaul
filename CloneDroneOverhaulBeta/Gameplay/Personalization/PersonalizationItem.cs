using CDOverhaul.Gameplay.Editors.Personalization;
using System.Collections.Generic;
using System.Reflection;

namespace CDOverhaul.Gameplay
{
    public abstract class PersonalizationItem : OverhaulDisposable
    {
        public const string NO_DESCRIPTION_PROVIDED = "No description provided";
        public const string NO_AUTHOR_SPECIFIED = "N/A";
        public const string NO_NAME_SPECIFIED = "Unknown item";

        [PersonalizationEditorProperty("Generic Info")]
        public string Name = NO_NAME_SPECIFIED;

        [PersonalizationEditorProperty("Generic Info")]
        public string Description = NO_DESCRIPTION_PROVIDED;

        [PersonalizationEditorProperty("Generic Info")]
        public string Author = NO_AUTHOR_SPECIFIED;

        public static List<PersonalizationEditorPropertyAttribute> GetAllFields<T>() where T : PersonalizationItem
        {
            List<PersonalizationEditorPropertyAttribute> result = new List<PersonalizationEditorPropertyAttribute>();
            foreach (FieldInfo info in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                PersonalizationEditorPropertyAttribute attribute = info.GetCustomAttribute<PersonalizationEditorPropertyAttribute>();
                if (attribute != null)
                {
                    attribute.FieldReference = info;
                    result.Add(attribute);
                }
            }
            return result;
        }
    }
}
