using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class OverhaulSettingAttribute_Old : Attribute
    {
        public string SettingRawPath;

        public string ParentSettingRawPath;

        public string Description;

        public object DefaultValue;

        public object RequiredValue;

        public bool IsHidden;

        public OverhaulSettingAttribute_Old(string settingPath, object defValue, bool isHidden = false, string description = null, string parentSettingPath = null)
        {
            SettingRawPath = settingPath;
            DefaultValue = defValue;
            IsHidden = isHidden;
            Description = description;
            ParentSettingRawPath = parentSettingPath;
        }
    }
}
