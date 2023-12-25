using Pathfinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod
{
    public class ModContentManager : Singleton<ModContentManager>
    {
        public string[] GetInstalledContent()
        {
            return Directory.GetDirectories(ModCore.contentFolder);
        }

        public string GetFolder(string contentName)
        {
            string path = ModCore.contentFolder + contentName + "/";
            if (!Directory.Exists(path))
                return null;

            return path;
        }

        public bool HasFolder(string contentName)
        {
            return Directory.Exists(ModCore.contentFolder + contentName + "/");
        }

        public bool IsFullInstallation()
        {
            return false;
        }
    }
}
