using System.Collections.Generic;

namespace OverhaulMod.Content.Personalization
{
    public class VolumeSettingsPreset
    {
        public string VoxFilePath;

        public float VoxelSize;

        public bool CenterPivot;

        public string ColorReplacements;

        public Dictionary<string, FavoriteColorSettings> ReplaceWithFavoriteColors;
    }
}
