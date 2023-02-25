using UnityEngine;

namespace CDOverhaul
{
    public class SettingDescription
    {
        public string Description { get; set; }
        public Sprite Image_4_3 { get; set; }
        public Sprite Image_16_9 { get; set; }

        public bool HasAnyImages => Image_16_9 != null || Image_4_3 != null;
        public bool Has43Image => Image_4_3 != null;
        public bool Has169Image => Image_16_9 != null;

        public SettingDescription(in string desc, in string img43fileName = null, in string img169fileName = null)
        {
            Description = desc;

            if (!string.IsNullOrEmpty(img43fileName))
            {
                Image_4_3 = OverhaulUtilities.TextureAndMaterialUtils.FastSpriteCreate(OverhaulUtilities.TextureAndMaterialUtils.LoadTexture(OverhaulMod.Core.ModDirectory + "Assets/Settings/" + img43fileName));
                return;
            }
            if (!string.IsNullOrEmpty(img169fileName))
            {
                Image_16_9 = OverhaulUtilities.TextureAndMaterialUtils.FastSpriteCreate(OverhaulUtilities.TextureAndMaterialUtils.LoadTexture(OverhaulMod.Core.ModDirectory + "Assets/Settings/" + img169fileName));
            }
        }
    }
}
