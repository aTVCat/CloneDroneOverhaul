using System;
using System.Reflection;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AdvancedPhotomodeSettingAttribute : Attribute
    {
        public string DisplayName;
        public string CategoryName;
        public bool IsAvailable;

        public FieldInfo Field;
        public MethodInfo Method;

        public bool IsAction() => Method != null;

        public AdvancedPhotomodeSliderParametersAttribute SliderParameters;
        public AdvancedPhotomodeRequireContentAttribute ContentParameters;

        public AdvancedPhotomodeSettingAttribute(string displayName, string category, bool debugOnly = false)
        {
            DisplayName = displayName;
            CategoryName = category;

            IsAvailable = OverhaulVersion.IsDebugBuild || !debugOnly;
        }
    }
}
