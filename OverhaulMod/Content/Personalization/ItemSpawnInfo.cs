using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class ItemSpawnInfo
    {
        public PersonalizationController Reference;

        public PersonalizationItemInfo ItemInfo;

        public ItemSpawnInfo()
        {

        }

        public ItemSpawnInfo(PersonalizationController reference, PersonalizationItemInfo itemInfo)
        {
            Reference = reference;
            ItemInfo = itemInfo;
        }

        public Color GetFavoriteColor()
        {
            if (PersonalizationEditorManager.IsInEditorMode()) UIPersonalizationEditor.Instance.Utilities.GetFavoriteColor();

            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo != null)
            {
                CharacterModel.PatternColorSet colors = Reference.GetOwnerModelColors();
                switch (itemInfo.Category)
                {
                    case PersonalizationCategory.WeaponSkins:
                        return getColorOfWeapon(ref colors, itemInfo.Weapon);
                    case PersonalizationCategory.Accessories:
                        return getColorOfBodyPart(ref colors, itemInfo.BodyPartName);
                }
            }

            return getDefaultColor();
        }

        private Color getDefaultColor()
        {
            return Reference.GetOwnerModelPrimaryColor();
        }

        private Color getColorOfWeapon(ref CharacterModel.PatternColorSet colors, WeaponType weapon)
        {
            switch (weapon)
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
            return getDefaultColor();
        }

        private Color getColorOfBodyPart(ref CharacterModel.PatternColorSet colors, string bodyPart)
        {
            if (bodyPart.IsNullOrEmpty() || bodyPart.IsNullOrWhiteSpace()) return getDefaultColor();

            if (PersonalizationManager.HeadBodyParts.Contains(bodyPart))
            {
                return colors.HeadColor;
            }
            else if (PersonalizationManager.TorsoBodyParts.Contains(bodyPart))
            {
                return colors.BodyColor;
            }
            else if (PersonalizationManager.LegsBodyParts.Contains(bodyPart))
            {
                return colors.RootColor;
            }
            return getDefaultColor();
        }
    }
}
