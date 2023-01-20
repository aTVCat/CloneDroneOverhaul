using CloneDroneOverhaul.V3.Base;
using UnityEngine;

namespace CloneDroneOverhaul.V3.Gameplay
{
    public class OverhaulUpgradeDescription : UpgradeDescription
    {
        public const string UPGRADE_ICON_ASSET_PREFIX = "UpgradeIco_";

        /// <summary>
        /// Sprite that is used for upgrades with no icon
        /// </summary>
        public static Sprite MissingSpriteIcon => OverhaulCacheAndGarbageController.GetCached<Sprite>(V3_MainModController.CACHED_ASSET_PREFIX + OverhaulUpgradeDescription.UPGRADE_ICON_ASSET_PREFIX + "Unknown");

        public UpgradeInfo UpgradeInformation;
    }
}