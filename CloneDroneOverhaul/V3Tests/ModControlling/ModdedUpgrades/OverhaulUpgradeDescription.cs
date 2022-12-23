using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Base
{
    public class OverhaulUpgradeDescription : UpgradeDescription
    {
        public const string UPGRADE_ICON_ASSET_PREFIX = "UpgradeIco_";
        public static Sprite MissingSpriteIcon => OverhaulCacheManager.GetCached<Sprite>(V3_MainModController.CACHED_ASSET_PREFIX + OverhaulUpgradeDescription.UPGRADE_ICON_ASSET_PREFIX + "Unknown");

        public UpgradeInfo UpgradeInformation;
    }
}