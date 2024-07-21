namespace OverhaulMod.Content.Personalization
{
    public class FavoriteColorSettings
    {
        public float SaturationMultiplier, BrightnessMultiplier, GlowPercent;

        public FavoriteColorSettings()
        {

        }

        public FavoriteColorSettings(float saturationMultiplier, float brightnessMultiplier, float glowPercent)
        {
            SaturationMultiplier = saturationMultiplier;
            BrightnessMultiplier = brightnessMultiplier;
            GlowPercent = glowPercent;
        }
    }
}
