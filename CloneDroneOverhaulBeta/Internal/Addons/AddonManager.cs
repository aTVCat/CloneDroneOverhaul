using System;
using System.Collections.Generic;
using System.IO;

namespace CDOverhaul
{
    public class AddonManager : OverhaulManager<AddonManager>
    {
        public const string ADDONS_FOLDER = "Addons";

        public List<AddonBase> LoadedAddons;

        public static bool hasLoadedAddons
        {
            get;
            private set;
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
            if (!LoadedAddons.IsNullOrEmpty())
                foreach (AddonBase controllerBase in LoadedAddons)
                    controllerBase.AddListeners();
        }

        protected override void OnAssetsLoaded()
        {
            base.OnAssetsLoaded();
            if (hasLoadedAddons)
                return;

            LoadedAddons = new List<AddonBase>();
            try
            {
                using (AddonLoader loader = new AddonLoader())
                    loader.LoadAllContent(this);
            }
            catch (Exception exc)
            {
                OverhaulDebug.Warn("[AddonLoader]: " + exc, EDebugType.Assets);
            }
            hasLoadedAddons = true;
        }

        protected override void OnDisposed()
        {
            if (!LoadedAddons.IsNullOrEmpty())
                foreach (AddonBase controllerBase in LoadedAddons)
                {
                    if (controllerBase)
                        controllerBase.Dispose(true);
                }
            LoadedAddons.Clear();
            base.OnDisposed();
        }

        public string GetContentDirectory()
        {
            string path = OverhaulMod.Core.ModDirectory + ADDONS_FOLDER + "/";
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
