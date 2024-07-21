namespace OverhaulMod.Engine
{
    public class RealisticLightingInfo
    {
        public LightingInfo Lighting;

        public string LevelPrefabName;

        public int SkyboxIndex;

        public void FixValues()
        {
            if (Lighting == null)
                Lighting = new LightingInfo();
        }
    }
}
