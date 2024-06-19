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

            GetWeaponProperties(objectBehaviour.ControllerInfo.Reference.owner, objectBehaviour.ControllerInfo.ItemInfo.Weapon, out bool of, out bool gs);
            return isGreatSword == gs && isOnFire == of;
        }

        public void GetWeaponProperties(out PersonalizationEditorObjectShowConditions showConditions)
        {
            GetWeaponProperties(objectBehaviour.ControllerInfo.Reference.owner, objectBehaviour.ControllerInfo.ItemInfo.Weapon, out showConditions);
        }

        public void GetWeaponProperties(out bool isOnFire, out bool isGreatSword)
        {
            GetWeaponProperties(objectBehaviour.ControllerInfo.Reference.owner, objectBehaviour.ControllerInfo.ItemInfo.Weapon, out isOnFire, out isGreatSword);
        }

        public static void GetWeaponProperties(FirstPersonMover firstPersonMover, WeaponType weaponType, out bool isOnFire, out bool isGreatSword)
        {
            isGreatSword = false;
            isOnFire = false;
            switch (weaponType)
            {
                case WeaponType.Sword:
                    isGreatSword = GameModeManager.UsesMultiplayerSpeedMultiplier();
                    isOnFire = firstPersonMover.HasUpgrade(UpgradeType.FireSword);
                    break;
                case WeaponType.Hammer:
                    isOnFire = firstPersonMover.HasUpgrade(UpgradeType.FireHammer);
                    break;
                case WeaponType.Spear:
                    isOnFire = firstPersonMover.HasUpgrade(UpgradeType.FireSpear);
                    break;
            }
        }

        public static void GetWeaponProperties(FirstPersonMover firstPersonMover, WeaponType weaponType, out PersonalizationEditorObjectShowConditions showConditions)
        {
            GetWeaponProperties(firstPersonMover, weaponType, out bool of, out bool gs);
            if (!of && !gs)
            {
                showConditions = PersonalizationEditorObjectShowConditions.IsNormal;
            }
            else if (of && !gs)
            {
                showConditions = PersonalizationEditorObjectShowConditions.IsOnFire;
            }
            else if (!of && gs)
            {
                showConditions = PersonalizationEditorObjectShowConditions.IsNormalMultiplayer;
            }
            else if (of && gs)
            {
                showConditions = PersonalizationEditorObjectShowConditions.IsOnFireMultiplayer;
            }
            else
            {
                showConditions = PersonalizationEditorObjectShowConditions.None;
            }
        }
    }
}
