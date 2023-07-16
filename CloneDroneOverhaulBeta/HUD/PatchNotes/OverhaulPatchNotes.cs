using System;
using System.Collections.Generic;

namespace CDOverhaul
{
    public static class OverhaulPatchNotes
    {
        public static readonly List<PatchInfo> AllChangelogs = new List<PatchInfo>();

        public static void Initialize()
        {
            if (!OverhaulSessionController.GetKey<bool>("HasAddedChangelogs"))
            {
                OverhaulSessionController.SetKey("HasAddedChangelogs", true);

                AllChangelogs.Add(new PatchInfo(new Version(0, 2, 12, 0), "0.2.12.0", new string[] { }));
                AllChangelogs.Add(new PatchInfo(new Version(0, 2, 11, 1), "0.2.11.1", new string[] { }));
                AllChangelogs.Add(new PatchInfo(new Version(0, 2, 11, 0), "0.2.11.0", new string[] { }));
                AllChangelogs.Add(new PatchInfo(new Version(0, 2, 10, 55), "0.2.10.54", new string[] { }));
                AllChangelogs.Add(new PatchInfo(new Version(0, 2, 10, 44), "0.2.10.44", new string[] { }));
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

            public string[] Art;

            public PatchInfo(Version targetVersion, string folderName, string[] art)
            {
                TargetModVersion = targetVersion;
                Folder = folderName;
                Art = art;

                DirectoryPath = OverhaulMod.Core.ModDirectory + "Assets/Changelogs/" + Folder + "/";
            }
        }
    }
}
