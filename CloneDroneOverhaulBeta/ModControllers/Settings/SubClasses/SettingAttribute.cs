using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class OverhaulSettingAttribute : Attribute
    {
        public string SettingRawPath;

        public string ParentSettingRawPath;

        public string Description;

        public string Img4_3Path;

        public string Img16_9Path;

        public object DefaultValue;

        public bool IsHidden;

        public OverhaulSettingAttribute(string settingPath, object defValue, bool isHidden = false, string description = null, string img43filename = null, string img169filename = null, string parentSettingPath = null)
        {
            SettingRawPath = settingPath;
            DefaultValue = defValue;
            IsHidden = isHidden;
            Description = description;
            Img4_3Path = img43filename;
            Img16_9Path = img169filename;
            ParentSettingRawPath = parentSettingPath;
        }
    }
}
