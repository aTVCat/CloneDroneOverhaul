using System.Collections.Generic;

namespace CDOverhaul.Gameplay.WeaponSkins
{
    /// <summary>
    /// Another new skins controller that should work better this time
    /// </summary>
    public class WeaponSkinsController : PersonalizationItemsController
    {
        public const string DEFAULT = "default";

        public const string SWORD_SKIN_SETTING = "Player.WeaponSkinsV2.Sword";
        public const string BOW_SKIN_SETTING = "Player.WeaponSkinsV2.Bow";
        public const string HAMMER_SKIN_SETTING = "Player.WeaponSkinsV2.Hammer";
        public const string SPEAR_SKIN_SETTING = "Player.WeaponSkinsV2.Spear";
        public const string SHIELD_SKIN_SETTING = "Player.WeaponSkinsV2.Shield";
        public const string ARROW_SKIN_SETTING = "Player.WeaponSkinsV2.Arrow";

        [OverhaulSettingAttribute(SWORD_SKIN_SETTING, DEFAULT, true)]
        public static string EquippedSwordSkin;
        [OverhaulSettingAttribute(BOW_SKIN_SETTING, DEFAULT, true)]
        public static string EquippedBowSkin;
        [OverhaulSettingAttribute(HAMMER_SKIN_SETTING, DEFAULT, true)]
        public static string EquippedHammerSkin;
        [OverhaulSettingAttribute(SPEAR_SKIN_SETTING, DEFAULT, true)]
        public static string EquippedSpearSkin;
        [OverhaulSettingAttribute(SHIELD_SKIN_SETTING, DEFAULT, true)]
        public static string EquippedShieldSkin;
        [OverhaulSettingAttribute(ARROW_SKIN_SETTING, DEFAULT, true)]
        public static string EquippedArrowSkin;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;

            _ = firstPersonMover.gameObject.AddComponent<WeaponSkinsWearer>();
        }

        public override string GetRepositoryFolder() => "WeaponSkins";
    }
}
