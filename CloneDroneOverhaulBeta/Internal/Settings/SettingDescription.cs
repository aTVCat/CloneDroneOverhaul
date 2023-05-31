using System.IO;
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
                Image_4_3 = getSprite(img43fileName);

            else if (!string.IsNullOrEmpty(img169fileName))
                Image_16_9 = getSprite(img169fileName);
        }

        private static Sprite getSprite(string fileName)
        {
            string path = OverhaulMod.Core.ModDirectory + "Assets/Settings/" + fileName;
            Texture2D texture = new Texture2D(1, 1);
            if (File.Exists(path))
            {
                byte[] content = File.ReadAllBytes(path);
                if (!content.IsNullOrEmpty())
                {
                    texture.filterMode = FilterMode.Bilinear;
                    texture.LoadImage(content, false);
                    texture.Apply();

                    return texture.FastSpriteCreate();
                }
            }
            return null;
        }
    }
}
