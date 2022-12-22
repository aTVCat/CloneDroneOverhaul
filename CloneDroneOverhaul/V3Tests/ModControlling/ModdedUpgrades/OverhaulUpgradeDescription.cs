using UnityEngine;
using ModLibrary;

namespace CloneDroneOverhaul.V3Tests.Base
{
    public class OverhaulUpgradeDescription : UpgradeDescription
    {
        public const string UPGRADE_ICON_ASSET_PREFIX = "UpgradeIco_";        
        public static Sprite MissingSpriteIcon
        {
            get
            {
                return OverhaulCacheManager.GetCached<Sprite>(V3_MainModController.CACHED_ASSET_PREFIX + OverhaulUpgradeDescription.UPGRADE_ICON_ASSET_PREFIX + "Unknown");
            }
        }

        public UpgradeInfo UpgradeInformation;
    }
}