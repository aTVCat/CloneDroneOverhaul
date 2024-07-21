﻿namespace OverhaulMod.Engine
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
            }
        }

        public static void GetWeaponVariant(FirstPersonMover firstPersonMover, WeaponType weaponType, out WeaponVariant showConditions)
        {
            GetWeaponVariant(firstPersonMover, weaponType, out bool of, out bool gs);
            if (!of && !gs)
            {
                showConditions = WeaponVariant.Normal;
            }
            else if (of && !gs)
            {
                showConditions = WeaponVariant.OnFire;
            }
            else if (!of && gs)
            {
                showConditions = WeaponVariant.NormalMultiplayer;
            }
            else if (of && gs)
            {
                showConditions = WeaponVariant.OnFireMultiplayer;
            }
            else
            {
                showConditions = WeaponVariant.None;
            }
        }
    }
}
