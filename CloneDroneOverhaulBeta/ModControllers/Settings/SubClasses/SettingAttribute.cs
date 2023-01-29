using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class OverhaulSettingAttribute : Attribute
    {
        public string MySetting;

        public object DefaultValue;

        public bool IsHidden;

        public OverhaulSettingAttribute(string settingPath, object defValue, bool isHidden = false)
        {
            MySetting = settingPath;
            DefaultValue = defValue;
            IsHidden = isHidden;
        }
    }
}
