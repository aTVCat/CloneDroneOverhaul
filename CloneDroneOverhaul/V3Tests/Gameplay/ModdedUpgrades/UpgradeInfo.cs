using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public struct UpgradeInfo
    {
        public const string NAME_LOCALIZATION_PREFIX = "OUD_Name_";
        public const string DESCRIPTION_LOCALIZATION_PREFIX = "OUD_Desc_";
        public const UpgradeType DEFAULT_UPGRADE_TYPE = (UpgradeType)7000;

        private string _upgradeLocID;
        /// <summary>
        /// Get translated upgrade name string
        /// </summary>
        public string UpgradeName => OverhaulMain.GetTranslatedString(NAME_LOCALIZATION_PREFIX + _upgradeLocID);
        /// <summary>
        /// Get translated upgrade description string
        /// </summary>
        public string UpgradeDescription => OverhaulMain.GetTranslatedString(DESCRIPTION_LOCALIZATION_PREFIX + _upgradeLocID);

        private UpgradeType _upgradeType;
        /// <summary>
        /// Get upgrade type (usually it is a value that is not specified in UpgradeType enum)
        /// </summary>
        public UpgradeType UpgradeType => _upgradeType;

        private Sprite _upgradeIcon;
        /// <summary>
        /// Get upgrade icon
        /// </summary>
        public Sprite UpgradeIcon => _upgradeIcon;

        public UpgradeInfo(in string nameAndDescLocID, in UpgradeType upgradeType, in Sprite upgradeIcon)
        {
            _upgradeLocID = nameAndDescLocID;
            _upgradeType = upgradeType;
            _upgradeIcon = upgradeIcon != null ? upgradeIcon : OverhaulUpgradeDescription.MissingSpriteIcon;
        }
    }
}