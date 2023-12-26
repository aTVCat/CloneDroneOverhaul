using System;

namespace OverhaulMod.Content
{
    public class UpdateInfo
    {
        public int ModBotVersion;
        public Version ModVersion;

        public string DownloadLink; // only for public releases
        public string Changelog;

        public override string ToString()
        {
            return $"Overhaul Mod V{ModVersion} ({ModBotVersion})";
        }

        public bool IsCurrentBuild()
        {
            return ModBuildInfo.version == ModVersion && ModCore.instance.ModInfo.Version == ModBotVersion;
        }

        public bool IsNewBuild()
        {
            return ModBuildInfo.version < ModVersion || ModCore.instance.ModInfo.Version < ModBotVersion;
        }

        public bool IsOldBuild()
        {
            return ModBuildInfo.version > ModVersion || ModCore.instance.ModInfo.Version > ModBotVersion;
        }
    }
}
