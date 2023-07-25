using CDOverhaul.Gameplay.Outfits;

namespace CDOverhaul.Gameplay.WeaponSkins
{
    public class WeaponSkinItem : PersonalizationItem
    {
        public static WeaponSkinItem CreateNew(string name) => new WeaponSkinItem
        {
            Name = name,
        };

        public static WeaponSkinItem CreateNewFromOldData(WeaponSkinsImportedItemDefinition importData)
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
