using OverhaulMod.Combat;

namespace OverhaulMod.Engine
{
    public class WeaponVariantManager
    {
        public static void GetWeaponVariant(FirstPersonMover firstPersonMover, WeaponType weaponType, out bool isOnFire, out bool isGreatSword)
        {
            isGreatSword = false;
            isOnFire = false;
            switch (weaponType)
            {
                case WeaponType.Sword:
                    isGreatSword = GameModeManager.UsesMultiplayerSpeedMultiplier();
                    isOnFire = firstPersonMover.HasUpgrade(UpgradeType.FireSword);
                    break;
                case WeaponType.Hammer:
                    isOnFire = firstPersonMover.HasUpgrade(UpgradeType.FireHammer);
                    break;
                case WeaponType.Spear:
                    isOnFire = firstPersonMover.HasUpgrade(UpgradeType.FireSpear);
                    break;
                case ModWeaponsManager.SCYTHE_TYPE:
                    isOnFire = firstPersonMover.HasUpgrade(ModUpgradesManager.SCYTHE_FIRE_UPGRADE);
                    break;
            }
        }

        public static void GetWeaponVariant(FirstPersonMover firstPersonMover, WeaponType weaponType, out WeaponVariant2 showConditions)
        {
            GetWeaponVariant(firstPersonMover, weaponType, out bool of, out bool gs);
            if (!of && !gs)
            {
                showConditions = WeaponVariant2.Normal;
            }
            else if (of && !gs)
            {
                showConditions = WeaponVariant2.OnFire;
            }
            else if (!of && gs)
            {
                showConditions = WeaponVariant2.NormalMultiplayer;
            }
            else if (of && gs)
            {
                showConditions = WeaponVariant2.OnFireMultiplayer;
            }
            else
            {
                showConditions = WeaponVariant2.None;
            }
        }

        public static string GetWeaponVariantString(WeaponVariant2 weaponVariant)
        {
            switch (weaponVariant)
            {
                case WeaponVariant2.Normal:
                    return "Normal";
                case WeaponVariant2.OnFire:
                    return "Fire";
                case WeaponVariant2.NormalMultiplayer:
                    return "Normal (Multiplayer)";
                case WeaponVariant2.OnFireMultiplayer:
                    return "Fire (Multiplayer)";
            }
            return weaponVariant.ToString();
        }
    }
}
