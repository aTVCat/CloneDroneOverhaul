using OverhaulMod.Combat;
using OverhaulMod.UI;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationControllerInfo
    {
        public PersonalizationController Reference;

        public PersonalizationItemInfo ItemInfo;

        public PersonalizationControllerInfo()
        {

        }

        public PersonalizationControllerInfo(PersonalizationController reference, PersonalizationItemInfo itemInfo)
        {
            Reference = reference;
            ItemInfo = itemInfo;
        }

        public Color GetFavoriteColor()
        {
            if (PersonalizationEditorManager.IsInEditorMode())
            {
                return UIPersonalizationEditor.Instance.Utilities.GetFavoriteColor();
            }

            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo != null && itemInfo.Category == PersonalizationCategory.WeaponSkins)
            {
                if (itemInfo.Weapon != ModWeaponsManager.SCYTHE_TYPE)
                {
                    CharacterModel.PatternColorSet colors = Reference.Owner.GetCharacterModel().GetFavouriteColors();
                    switch (itemInfo.Weapon)
                    {
                        case WeaponType.Sword:
                            return colors.SwordColor;
                        case WeaponType.Bow:
                            return colors.BowColor;
                        case WeaponType.Hammer:
                            return colors.HammerColor;
                        case WeaponType.Spear:
                            return colors.SpearColor;
                    }
                }
            }
            return Reference.Owner.GetCharacterModel().GetFavouriteColor();
        }
    }
}
