using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class SettingFormelyKnownAs : Attribute
    {
        public string RawPath;

        public SettingFormelyKnownAs(string settingPath)
        {
            RawPath = settingPath;
        }
    }
}
