namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationAssetsInfo
    {
        public System.Version AssetsVersion;

        public void FixValues()
        {
            if (AssetsVersion == null)
                AssetsVersion = new System.Version(1, 0, 0, 0);
        }
    }
}
