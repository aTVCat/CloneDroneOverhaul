using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    public static class OverhaulPatchNotes
    {
        public static readonly List<PatchInfo> AllChangelogs = new List<PatchInfo>();

        public static void Initialize()
        {
            if (!OverhaulSessionController.GetKey<bool>("HasReadChangelogs"))
            {
                OverhaulSessionController.SetKey("HasReadChangelogs", true);

                AllChangelogs.Add(new PatchInfo(new Version(0, 3, 0, 188), "TestChangelog", new string[] { "20230506203329_1.jpg" }));
                AllChangelogs.Add(new PatchInfo(new Version(0, 3, 0, 404), "TestChangelog2", new string[] { "20230503213016_1.jpg", "20230505200409_1.jpg", "20230505205829_1.jpg", "freemoney.jpg" }));
            }
        }

        public class PatchInfo
        {
            public Version TargetModVersion;

            public string Folder;
            public string DirectoryPath
            {
                get;
                private set;
            }
            public string InformationString
            {
                get;
                private set;
            }

            public string[] Art;

            public PatchInfo(Version targetVersion, string folderName, string[] art)
            {
                TargetModVersion = targetVersion;
                Folder = folderName;
                Art = art;
 
                DirectoryPath = OverhaulMod.Core.ModDirectory + "Assets/Changelogs/" + Folder + "/";
                InformationString = OverhaulCore.ReadTextFile(DirectoryPath + "Info.txt");
            }
        }
    }
}
