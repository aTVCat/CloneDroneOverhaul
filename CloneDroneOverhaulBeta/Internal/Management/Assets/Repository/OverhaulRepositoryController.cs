using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Directory.CreateDirectory(RepositoryFolder);
            }

            string txtFile = RepositoryFolder + "Repository things go here.txt";
            if (!File.Exists(txtFile))
            {
                File.Create(txtFile);
            }

            foreach(string requiredDirectory in Directories)
            {
                string path = RepositoryFolder + requiredDirectory;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if(!Directory.Exists(path + "Web/"))
                {
                    Directory.CreateDirectory(path + "Web/");
                }
                if (!Directory.Exists(path + "Local/"))
                {
                    Directory.CreateDirectory(path + "Local/");
                }
            }
        }
    }
}
