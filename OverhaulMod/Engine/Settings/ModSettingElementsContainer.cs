using System.Collections.Generic;

namespace OverhaulMod.Engine.Settings
{
    public class ModSettingElementsContainer
    {
        public List<ModSettingsPage> Pages;

        public void FixValues()
        {
            if (Pages == null) Pages = new List<ModSettingsPage>();
            else
            {
                foreach (ModSettingsPage page in Pages)
                    page.FixValues();
            }
        }

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