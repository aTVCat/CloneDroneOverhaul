using CDOverhaul.Gameplay.Outfits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay.WeaponSkins
{
    public class WeaponSkinItem
    {
        public string Name;
        public string Author = OutfitItem.NoAuthorString;
        public string Description = OutfitItem.NoDescProvidedString;

        public static WeaponSkinItem CreateNew(string name) => new WeaponSkinItem
        {
            Name = name,
        };

        public static WeaponSkinItem CreateNewFromData(WeaponSkinsImportedItemDefinition importData)
        {
            WeaponSkinItem skinItem = new WeaponSkinItem
            {
                Name = importData.Name,
                Author = importData.Author,
                Description = importData.Description
            };
            return skinItem;
        }
    }
}
