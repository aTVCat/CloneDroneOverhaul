using UnityEngine;

namespace OverhaulMod.Engine
{
    public class RealisticLightingInfo
    {
        public LightingInfo Lighting;

        public string LevelPrefabName;

        public string SkyboxName;

        public Color Tint = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        public float Rotation;

        public void FixValues()
        {
            if (Lighting == null)
                Lighting = new LightingInfo();
        }

        public static Color GetDefaultTint()
        {
            return new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }
}
