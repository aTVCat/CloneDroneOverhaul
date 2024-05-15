using System;

namespace OverhaulMod.Content.Personalization
{
    [AttributeUsage(AttributeTargets.Property, false, false)]
    public class PersonalizationEditorObjectPropertyAttribute : Attribute
    {
        public bool IsFileLocation;

        public string FileLocationSearchPattern;

        public float MinFloatValue, MaxFloatValue;

        public PersonalizationEditorObjectPropertyAttribute()
        {

        }

        public PersonalizationEditorObjectPropertyAttribute(bool isFileLocation, string searchPattern = "*.*")
        {
            IsFileLocation = isFileLocation;
            FileLocationSearchPattern = searchPattern;
        }

        public PersonalizationEditorObjectPropertyAttribute(float min, float max)
        {
            MinFloatValue = min;
            MaxFloatValue = max;
        }
    }
}
