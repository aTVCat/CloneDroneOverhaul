namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AutoBuildUpgrade
    {
        public UpgradeType Upgrade;
        public int Level;

        public AutoBuildUpgrade(UpgradeType type, int level)
        {
            Upgrade = type;
            Level = level;
        }
    }
}
