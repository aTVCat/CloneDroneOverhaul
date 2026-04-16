using System.Collections.Generic;

namespace OverhaulMod.Engine.Settings
{
    public class ModSettingInfosContainer
    {
        public List<ModSettingsPage> Pages;

        public ModSettingsPage GetPage(string id)
        {
            for (int i = 0; i < Pages.Count; i++)
            {
                ModSettingsPage page = Pages[i];
                if (page.ID == id)
                    return page;
            }
            return null;
        }
    }
}