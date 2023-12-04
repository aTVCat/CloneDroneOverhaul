using System.IO;

namespace CDOverhaul
{
    public class OverhaulContentManager : OverhaulManager<OverhaulContentManager>
    {
        public const string CONTENT_FOLDER = "Content";

        public readonly string[] Directories = new string[]
        {
            "WeaponSkins/",
            "Outfits/",
            "Pets/"
        };

        public string folder
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            createDirectories();
        }

        public string GetFolder(string name)
        {
            return folder + name + "/";
        }

        private void createDirectories()
        {
            folder = OverhaulMod.Core.ModDirectory + CONTENT_FOLDER + "/";
            if (!Directory.Exists(folder))
            {
                _ = Directory.CreateDirectory(folder);
            }

            string txtFile = folder + "Content goes here.txt";
            if (!File.Exists(txtFile))
            {
                _ = File.Create(txtFile);
            }

            foreach (string requiredDirectory in Directories)
            {
                string path = folder + requiredDirectory;
                if (!Directory.Exists(path))
                {
                    _ = Directory.CreateDirectory(path);
                }
            }
        }
    }
}
