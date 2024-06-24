using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectVisibilityController : PersonalizationEditorObjectComponentBase
    {
        [PersonalizationEditorObjectProperty]
        public bool alwaysOn
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(alwaysOn), true);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(alwaysOn), value);
            }
        }

        [PersonalizationEditorObjectProperty]
        public bool isOnFire
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(isOnFire), false);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(isOnFire), value);
            }
        }

        [PersonalizationEditorObjectProperty]
        public bool isGreatSword
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(isGreatSword), false);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(isGreatSword), value);
            }
        }

        public bool MustShowInEditor()
        {
            return objectBehaviour.ControllerInfo.ItemInfo.Category == PersonalizationCategory.WeaponSkins;
        }

        public bool MustShowTheObject()
        {
            if (alwaysOn)
                return true;

            WeaponVariantManager.GetWeaponVariant(objectBehaviour.ControllerInfo.Reference.owner, objectBehaviour.ControllerInfo.ItemInfo.Weapon, out bool of, out bool gs);
            return isGreatSword == gs && isOnFire == of;
        }

        public void GetWeaponVariant(out WeaponVariant showConditions)
        {
            WeaponVariantManager.GetWeaponVariant(objectBehaviour.ControllerInfo.Reference.owner, objectBehaviour.ControllerInfo.ItemInfo.Weapon, out showConditions);
        }

        public void GetWeaponVariant(out bool isOnFire, out bool isGreatSword)
        {
            WeaponVariantManager.GetWeaponVariant(objectBehaviour.ControllerInfo.Reference.owner, objectBehaviour.ControllerInfo.ItemInfo.Weapon, out isOnFire, out isGreatSword);
        }
    }
}
