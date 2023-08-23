using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Gameplay.Pets;
using CDOverhaul.Gameplay.WeaponSkins;
using System.Collections.Generic;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public static class PersonalizationEditor
    {
        [OverhaulSetting("Player.P_Editor.AutoSave", true, true)]
        public static bool IsAutoSaveEnabled;

        public static List<PersonalizationEditorPropertyAttribute> WeaponSkinItemFields;

        public static List<PersonalizationEditorPropertyAttribute> AccessoryItemFields;

        public static List<PersonalizationEditorPropertyAttribute> PetItemFields;

        public static PersonalizationCategory EditingCategory
        {
            get;
            set;
        } = PersonalizationCategory.WeaponSkins;

        public static PersonalizationItem EditingItem
        {
            get;
            set;
        }

        public static void Initialize()
        {
            if (WeaponSkinItemFields.IsNullOrEmpty())
                WeaponSkinItemFields = PersonalizationItem.GetAllFields(typeof(WeaponSkinItem));

            if (AccessoryItemFields.IsNullOrEmpty())
                AccessoryItemFields = PersonalizationItem.GetAllFields(typeof(OutfitItem));

            if (PetItemFields.IsNullOrEmpty())
                PetItemFields = PersonalizationItem.GetAllFields(typeof(PetItem));
        }

        public static List<PersonalizationItem> GetPersonalizationCategorySavedItems(PersonalizationCategory category)
        {
            PersonalizationItemsController itemsController = null;
            switch (category)
            {
                case PersonalizationCategory.WeaponSkins:
                    itemsController = OverhaulController.Get<WeaponSkins.WeaponSkinsController>();
                    break;
                case PersonalizationCategory.Outfits:
                    itemsController = OverhaulController.Get<Outfits.OutfitsController>();
                    break;
                case PersonalizationCategory.Pets:
                    itemsController = OverhaulController.Get<Pets.PetsController>();
                    break;

            }
            return itemsController && itemsController.ItemsData != null ? itemsController.ItemsData.Items : null;
        }

        public static PersonalizationItemsController GetPersonalizationControllerByCategory(PersonalizationCategory category)
        {
            switch (category)
            {
                case PersonalizationCategory.WeaponSkins:
                    return OverhaulController.Get<WeaponSkins.WeaponSkinsController>();
                case PersonalizationCategory.Outfits:
                    return OverhaulController.Get<Outfits.OutfitsController>();
                case PersonalizationCategory.Pets:
                    return OverhaulController.Get<Pets.PetsController>();
            }
            return null;
        }

        public static void SaveEditingItem(bool isUICall = false)
        {
            if (EditingItem == null)
                return;

            PersonalizationItemsController controller = GetPersonalizationControllerByCategory(EditingCategory);
            if (!controller)
                return;

            OverhaulCore.WriteText(controller.GetItemsDataFile(), Newtonsoft.Json.JsonConvert.SerializeObject(controller.ItemsData, DataRepository.Instance.GetSettings()));

            if (!isUICall)
            {
                PersonalizationEditorUI editorUI = OverhaulController.Get<PersonalizationEditorUI>();
                if (editorUI)
                    editorUI.SavePanel.NeedsToSave = false;
            }
        }

        public static string GetOnlyID(string textFromEditor, out byte idType)
        {
            idType = 0;
            if (textFromEditor.Contains("steam "))
                idType = 1;
            else if (textFromEditor.Contains("discord "))
                idType = 2;
            else if (textFromEditor.Contains("playfab "))
                idType = 3;

            return textFromEditor.Replace("steam ", string.Empty).Replace("discord ", string.Empty).Replace("playfab ", string.Empty);
        }
    }
}
