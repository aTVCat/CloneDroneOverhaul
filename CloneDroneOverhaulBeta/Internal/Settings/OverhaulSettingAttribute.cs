using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class OverhaulSettingAttribute : Attribute
    {
        public string SettingRawPath;

        public string ParentSettingRawPath;

        public string Description;

        public object DefaultValue;

        public object RequiredValue;

        public bool IsHidden;

        public OverhaulSettingAttribute(string settingPath, object defValue, bool isHidden = false, string description = null, string parentSettingPath = null)
        {
            SettingRawPath = settingPath;
            DefaultValue = defValue;
            IsHidden = isHidden;
            Description = description;
            ParentSettingRawPath = parentSettingPath;
        }
    }
}
