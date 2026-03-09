using OverhaulMod.Utils;
using System;

namespace OverhaulMod.Content
{
    public class UpdateInfo
    {
        public Version ModVersion;
        public string Changelog;

        public bool IsGoogleDriveLink;
        public string DownloadLink;

        public string AllowedUsers;
        public ExclusivePerkType RequireExclusivePerk;

        [NonSerialized]
        public Version DisplayVersion;

        public void FixValues()
        {
            if (ModVersion == null)
                ModVersion = new Version(0, ModBuild.VersionMajor, ModBuild.VersionMinor, ModBuild.VersionBuild);

            DisplayVersion = new Version(ModVersion.Minor, ModVersion.Build, ModVersion.Revision);
        }

        public override string ToString()
        {
            return $"Overhaul mod {ModVersion}";
        }

        public bool CanBeInstalledByLocalUser()
        {
            if (DownloadLink.IsNullOrEmpty() || DownloadLink.IsNullOrWhiteSpace())
                return false;

            if (RequireExclusivePerk != ExclusivePerkType.None && ExclusivePerkManager.Instance.HasUnlockedPerk(RequireExclusivePerk))
                return true;

            return AllowedUsers.IsNullOrEmpty() || AllowedUsers.IsNullOrWhiteSpace() || AllowedUsers.Contains(ModUserInfo.localPlayerPlayFabID) || AllowedUsers.Contains(ModUserInfo.localPlayerSteamID.ToString());
        }

        public bool IsCurrentBuild()
        {
            return ModBuild.Version == DisplayVersion;
        }

        public bool IsNewerBuild()
        {
            return ModBuild.Version < DisplayVersion;
        }

        public bool IsOlderBuild()
        {
            return ModBuild.Version > DisplayVersion;
        }
    }
}
