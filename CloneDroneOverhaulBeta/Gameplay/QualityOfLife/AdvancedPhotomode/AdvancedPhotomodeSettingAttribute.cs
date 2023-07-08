using System;
using System.Reflection;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AdvancedPhotomodeSettingAttribute : Attribute
    {
        public string DisplayName;
        public string CategoryName;

        public FieldInfo Field;

        public AdvancedPhotomodeSliderParametersAttribute SliderParameters;

        public AdvancedPhotomodeSettingAttribute(string displayName, string category)
        {
            DisplayName = displayName;
            CategoryName = category;
        }
    }
}
