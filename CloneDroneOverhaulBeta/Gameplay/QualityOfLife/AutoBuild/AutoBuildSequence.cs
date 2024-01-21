namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AutoBuildSequence
    {
        public AutoBuildVariant BuildVariant;
        public AutoBuildUpgrade[] Upgrades;

        public AutoBuildSequence(AutoBuildVariant buildVariant)
        {
            Upgrades = new AutoBuildUpgrade[4];

            switch (buildVariant)
            {
                case AutoBuildVariant.Random:
                    break;
                case AutoBuildVariant.FullSword:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.SwordUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.FireSword, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.FireSword, 2);
                    break;
                case AutoBuildVariant.FullBow:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.BowUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.FireArrow, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.FireArrow, 2);
                    Upgrades[3] = new AutoBuildUpgrade(UpgradeType.ArrowWidth, 1);
                    break;
                case AutoBuildVariant.FullHammer:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.Hammer, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.Hammer, 2);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.Hammer, 3);
                    break;
                case AutoBuildVariant.FullSpear:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.SpearUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.ShieldSize, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.FireSpear, 1);
                    break;

                case AutoBuildVariant.Kicker:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.KickUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.KickPower, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 1);
                    Upgrades[3] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 2);
                    break;
                case AutoBuildVariant.LightKicker:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.KickUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.GetUp, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 1);
                    Upgrades[3] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 2);
                    break;
                case AutoBuildVariant.ArmoredKicker:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.KickUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.Armor, 0);
                    break;
                case AutoBuildVariant.KickNDash:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.KickUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 2);
                    Upgrades[3] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 3);
                    break;
                case AutoBuildVariant.KickNRun:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.KickUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.Jetpack, 1);
                    break;

                case AutoBuildVariant.TacticalSword:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.SwordUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.KickUnlock, 1);
                    break;
                case AutoBuildVariant.TacticalFireSword:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.SwordUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.FireSword, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.KickUnlock, 1);
                    break;
                case AutoBuildVariant.JetPackSword:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.SwordUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.Jetpack, 1);
                    break;

                case AutoBuildVariant.TacticalBow:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.BowUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.ArrowWidth, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 1);
                    Upgrades[3] = new AutoBuildUpgrade(UpgradeType.KickUnlock, 1);
                    break;
                case AutoBuildVariant.TacticalFireBow:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.BowUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.FireArrow, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 1);
                    Upgrades[3] = new AutoBuildUpgrade(UpgradeType.KickUnlock, 1);
                    break;

                case AutoBuildVariant.TacticalHammer:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.Hammer, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.Hammer, 2);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 1);
                    break;
                case AutoBuildVariant.TacticalHammerKicker:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.Hammer, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.Hammer, 2);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.KickUnlock, 1);
                    break;

                case AutoBuildVariant.TacticalFireSpear:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.SpearUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.FireSpear, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 1);
                    break;
                case AutoBuildVariant.TacticalFireSpearKicker:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.SpearUnlock, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.FireSpear, 1);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.KickUnlock, 1);
                    break;

                case AutoBuildVariant.Spectator:
                    Upgrades[0] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 1);
                    Upgrades[1] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 2);
                    Upgrades[2] = new AutoBuildUpgrade(UpgradeType.EnergyCapacity, 3);
                    Upgrades[3] = new AutoBuildUpgrade(UpgradeType.EnergyRecharge, 1);
                    break;

                default:
                    throw new System.ArgumentException("Unsupported build variant");
            }
        }
    }
}
