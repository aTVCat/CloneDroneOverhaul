using System.Collections.Generic;

namespace OverhaulMod.Engine.Settings
{
    public class ModSettingsPage
    {
        public string ID;

        public string NameLocalizationID;

        public bool IsPrimaryPage;

        public List<ModSettingElementDescription> Settings;

        public void FixValues()
        {
            if (Settings == null) Settings = new List<ModSettingElementDescription>();
        }

        public ModSettingElementDescription GetSetting(string id)
        {
            for (int i = 0; i < Settings.Count; i++)
            {
                ModSettingElementDescription setting = Settings[i];
                if (setting.SettingID == id)
                    return setting;
            }
            return null;
        }
    }
}
