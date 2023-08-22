using System;
using System.IO;
using UnityEngine;

namespace CDOverhaul
{
    public class AdditionalContentController : OverhaulManager<AdditionalContentController>
    {
        public static bool HasLoadedContent
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsAdditionalContentSupportEnabled || HasLoadedContent)
                return;

            HasLoadedContent = true;

            try
            {
                AdditionalContentLoader.LoadAllContent();
            }
            catch (Exception exc)
            {
                Debug.LogWarning("ADDITIONAL CONTENT: " + exc);
            }
        }

        public string GetContentDirectory()
        {
            string path = OverhaulMod.Core.ModDirectory + "Content/";
            if (!Directory.Exists(path))
                _ = Directory.CreateDirectory(path);

            return path;
        }

        public string[] GetContentDirectories()
        {
            string[] files = Directory.GetDirectories(GetContentDirectory());
            return files;
        }

        public string[] GetZipFiles()
        {
            string[] files = Directory.GetFiles(GetContentDirectory(), "*.zip");
            return files;
        }
    }
}
