namespace OverhaulMod.Utils
{
    public static class ModITweenUtils
    {
        public static float ParametricBlend(float t)
        {
            float sqr = t * t;
            return sqr / ((2.0f * (sqr - t)) + 1.0f);
        }
    }
}
