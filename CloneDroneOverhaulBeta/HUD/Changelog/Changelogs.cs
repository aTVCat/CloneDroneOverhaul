using System;
using System.Collections.Generic;

namespace CDOverhaul
{
    public static class Changelogs
    {
        public static readonly List<PatchInfo> AllChangelogs = new List<PatchInfo>();

        public static void Initialize()
        {
            AllChangelogs.Add(new PatchInfo(new Version(0, 4, 0, 12), "0.4.0.12", new string[] { "Img1.jpg" }));
            AllChangelogs.Add(new PatchInfo(new Version(0, 2, 13, 3), "0.2.13.3", new string[] { }));
            AllChangelogs.Add(new PatchInfo(new Version(0, 2, 12, 0), "0.2.12.0", new string[] { }));
            AllChangelogs.Add(new PatchInfo(new Version(0, 2, 11, 1), "0.2.11.1", new string[] { }));
            AllChangelogs.Add(new PatchInfo(new Version(0, 2, 11, 0), "0.2.11.0", new string[] { }));
            AllChangelogs.Add(new PatchInfo(new Version(0, 2, 10, 55), "0.2.10.54", new string[] { }));
            AllChangelogs.Add(new PatchInfo(new Version(0, 2, 10, 44), "0.2.10.44", new string[] { }));
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

            public string[] Images;

            public PatchInfo(Version targetVersion, string folderName, string[] art)
            {
                TargetModVersion = targetVersion;
                Folder = folderName;
                Images = art;

                DirectoryPath = OverhaulMod.Core.ModDirectory + "Assets/Changelogs/" + Folder + "/";
            }
        }
    }
}
