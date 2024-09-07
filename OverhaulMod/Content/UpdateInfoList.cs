using System.Collections.Generic;

namespace OverhaulMod.Content
{
    public class UpdateInfoList
    {
        public const string RELEASE_BRANCH = "release";

        public const string PREVIEW_BRANCH = "preview";

        public const string CANARY_BRANCH = "canary";

        public UpdateInfo ModBotRelease, GitHubRelease, InternalRelease;

        public Dictionary<string, UpdateInfo> Branches;

        public void FixValues()
        {
            if(Branches == null)
                Branches = new Dictionary<string, UpdateInfo>();
        }

        public bool HasAnyNewBuildAvailable()
        {
            foreach (var item in Branches.Values)
                if (item.CanBeInstalledByLocalUser() && item.IsNewerBuild())
                    return true;

            return false;
        }
    }
}
