﻿using System;
using System.Collections.Generic;

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

                AllChangelogs.Add(new PatchInfo(new Version(0, 2, 10, 44), "0.2.10.44", new string[] { "20230507133358_1.jpg", "20230507133331_1.jpg" }));
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
