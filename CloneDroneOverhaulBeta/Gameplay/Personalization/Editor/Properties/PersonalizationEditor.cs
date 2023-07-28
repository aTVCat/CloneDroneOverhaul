using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Gameplay.Pets;
using CDOverhaul.Gameplay.WeaponSkins;
using System.Collections.Generic;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public static class PersonalizationEditor
    {
        public static List<PersonalizationEditorPropertyAttribute> WeaponSkinItemFields;

        public static List<PersonalizationEditorPropertyAttribute> AccessoryItemFields;

        public static List<PersonalizationEditorPropertyAttribute> PetItemFields;

        public static PersonalizationItem EditingItem
        {
            get;
            set;
        }

        public static void Initialize()
        {
            if (WeaponSkinItemFields.IsNullOrEmpty())
                WeaponSkinItemFields = PersonalizationItem.GetAllFields<WeaponSkinItem>();

            if (AccessoryItemFields.IsNullOrEmpty())
                AccessoryItemFields = PersonalizationItem.GetAllFields<OutfitItem>();

            if (PetItemFields.IsNullOrEmpty())
                PetItemFields = PersonalizationItem.GetAllFields<PetItem>();
        }
    }
}
