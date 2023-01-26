using System.Collections.Generic;

namespace CDOverhaul
{
    public static class SettingsController
    {
        private static readonly List<Setting> _settings = new List<Setting>();
        private static bool _hasAddedSettings;

        internal static void Initialize()
        {
            if (_hasAddedSettings)
            {
                return;
            }

            AddSetting<bool>("TestCategory.TestSection.TestSetting", false);

            _hasAddedSettings = true;
        }

        public static Setting AddSetting<T>(in string path, in T defaultValue)
        {
            Setting newSetting = new Setting();
            newSetting.SetUp<T>(path, defaultValue);
            _settings.Add(newSetting);
            return newSetting;
        }
    }
}
