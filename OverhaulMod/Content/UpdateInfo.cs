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

        private Version _displayVersion;
        public Version DisplayVersion
        {
            get
            {
                if (_displayVersion == null || !ModParseUtils.CompareVersionsWithDiffFormats(ModVersion, _displayVersion))
                {
                    _displayVersion = ModParseUtils.ConvertOldVersionFormat(ModVersion);
                }
                return _displayVersion;
            }
        }

        public void FixValues()
        {
            if (ModVersion == null)
                ModVersion = new Version(0, ModBuild.VersionMajor, ModBuild.VersionMinor, ModBuild.VersionBuild);
        }

        public override string ToString()
        {
            return $"Overhaul mod {DisplayVersion} ({ModVersion})";
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
