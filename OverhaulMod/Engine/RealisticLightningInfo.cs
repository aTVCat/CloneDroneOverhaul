namespace OverhaulMod.Engine
{
    public class RealisticLightningInfo
    {
        public LightningInfo Lightning;

        public string LevelPrefabName;

        public int SkyboxIndex;

        public void FixValues()
        {
            if (Lightning == null)
                Lightning = new LightningInfo();
        }
    }
}
