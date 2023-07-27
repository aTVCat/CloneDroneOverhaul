using System.IO;

namespace CDOverhaul
{
    public class OverhaulRepositoryController : OverhaulController
    {
        public static readonly string[] Directories = new string[]
        {
            "WeaponSkins/",
            "Outfits/",
            "Pets/"
        };

        public static string RepositoryFolder
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
            return RepositoryFolder + name + "/Local/";
        }

        private void createDirectories()
        {
            RepositoryFolder = OverhaulCore.ModDirectoryStatic + "Repository/";
            if (!Directory.Exists(RepositoryFolder))
            {
                _ = Directory.CreateDirectory(RepositoryFolder);
            }

            string txtFile = RepositoryFolder + "Repository things go here.txt";
            if (!File.Exists(txtFile))
            {
                _ = File.Create(txtFile);
            }

            foreach (string requiredDirectory in Directories)
            {
                string path = RepositoryFolder + requiredDirectory;
                if (!Directory.Exists(path))
                {
                    _ = Directory.CreateDirectory(path);
                }

                if (!Directory.Exists(path + "Web/"))
                {
                    _ = Directory.CreateDirectory(path + "Web/");
                }
                if (!Directory.Exists(path + "Local/"))
                {
                    _ = Directory.CreateDirectory(path + "Local/");
                }
            }
        }
    }
}
