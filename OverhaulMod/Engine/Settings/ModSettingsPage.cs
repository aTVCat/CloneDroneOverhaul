using System.Collections.Generic;

namespace OverhaulMod.Engine.Settings
{
    public class ModSettingsPage
    {
        public string ID;

        public string NameLocalizationID;

        public List<ModSettingInfo> Settings;

        public ModSettingInfo GetSetting(string id)
        {
            for (int i = 0; i < Settings.Count; i++)
            {
                ModSettingInfo setting = Settings[i];
                if (setting.ID == id)
                    return setting;
            }
            return null;
        }
    }
}
