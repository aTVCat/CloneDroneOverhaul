using System;
using System.Collections.Generic;
using System.IO;

namespace CDOverhaul
{
    public class AdditionalContentManager : OverhaulManager<AdditionalContentManager>
    {
        public static bool HasLoadedContent
        {
            get;
            private set;
        }

        public List<AdditionalContentControllerBase> Controllers
        {
            get;
            private set;
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
            if (!Controllers.IsNullOrEmpty())
                foreach (AdditionalContentControllerBase controllerBase in Controllers)
                    controllerBase.AddListeners();
        }

        protected override void OnAssetsLoaded()
        {
            base.OnAssetsLoaded();
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsAdditionalContentSupportEnabled || HasLoadedContent)
                return;

            Controllers = new List<AdditionalContentControllerBase>();
            try
            {
                using (AdditionalContentLoader loader = new AdditionalContentLoader())
                    loader.LoadAllContent(this);
            }
            catch (Exception exc)
            {
                OverhaulDebug.Warn("[AdditionalContentLoader]: " + exc, EDebugType.Assets);
            }
            HasLoadedContent = true;
        }

        protected override void OnDisposed()
        {
            if (!Controllers.IsNullOrEmpty())
                foreach (AdditionalContentControllerBase controllerBase in Controllers)
                {
                    controllerBase.Dispose(true);
                }
            Controllers.Clear();
            base.OnDisposed();
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
