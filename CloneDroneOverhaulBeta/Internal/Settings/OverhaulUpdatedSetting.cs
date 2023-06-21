using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class OverhaulUpdatedSetting : Attribute
    {
        public string RawPath;

        public OverhaulUpdatedSetting(string settingPath)
        {
            RawPath = settingPath;
        }
    }
}
