using System;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationAssetsInfo
    {
        [Obsolete]
        public Version AssetsVersion;

        public int AssetVersionNumber = -1;

        public void SetAssetVersionForOldBuilds()
        {
            AssetsVersion = new Version(2, 0, 2, AssetVersionNumber);
        }
    }
}
