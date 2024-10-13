using OverhaulMod.Engine;

namespace OverhaulMod.Content.Personalization
{
    public class CVMModelPreset
    {
        public string CvmFilePath;

        public WeaponType Weapon;

        public WeaponVariant Variant;

        public bool ReplaceColors = true;

        public bool ShowFireParticles = true;

        public CVMModelPreset()
        {

        }

        public CVMModelPreset(bool defaultValues)
        {
            if (defaultValues)
            {
                Weapon = WeaponType.Sword;
                Variant = WeaponVariant.Normal;
            }
        }
    }
}
