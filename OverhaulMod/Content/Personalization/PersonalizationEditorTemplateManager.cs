using OverhaulMod.Utils;
using System.IO;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorTemplateManager : Singleton<PersonalizationEditorTemplateManager>
    {
        private static string s_templatesFolder;
        public static string templatesFolder
        {
            get
            {
                if (s_templatesFolder == null)
                {
                    s_templatesFolder = Path.Combine(ModCore.dataFolder, "customizationTemplates");
                }
                return s_templatesFolder;
            }
        }

        private bool _hasLoadedTemplates;

        private bool _noTemplates;

        private PersonalizationItemInfo[] _templates;

        public void LoadTemplates()
        {
            _hasLoadedTemplates = true;

            string path = templatesFolder;
            if (!Directory.Exists(path))
            {
                _noTemplates = true;
                return;
            }

            string[] files = Directory.GetFiles(path);
            if (files.IsNullOrEmpty())
            {
                _noTemplates = true;
                return;
            }

            PersonalizationItemInfo[] array = new PersonalizationItemInfo[files.Length];

            int i = 0;
            foreach (string file in files)
            {
                PersonalizationItemInfo itemInfo;
                try
                {
                    itemInfo = ModJsonUtils.DeserializeStream<PersonalizationItemInfo>(file);
                }
                catch
                {
                    itemInfo = new PersonalizationItemInfo()
                    {
                        Corrupted = true
                    };
                }

                array[i] = itemInfo;

                i++;
            }

            _templates = array;
        }

        public PersonalizationItemInfo[] GetTemplates()
        {
            if (!_hasLoadedTemplates || _noTemplates)
                return null;

            return _templates;
        }
    }
}
