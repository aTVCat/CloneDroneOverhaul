using OverhaulMod.Engine;

namespace OverhaulMod.Content.Personalization.Objects
{
    public class PersonalizationEditorVisibilityToggler : PersonalizationEditorComponent
    {
        public WeaponVariant2 EnableIfWeaponVariant
        {
            get => (WeaponVariant2)GetPropertyValue(nameof(PersonalizationEditorVisibilityToggler), nameof(EnableIfWeaponVariant), 0);
            set => SetPropertyValue(nameof(PersonalizationEditorVisibilityToggler), nameof(EnableIfWeaponVariant), (int)value);
        }

        private bool _hasAddedEventListeners;

        private void Start()
        {
            if (PersonalizationEditorManager.IsInEditorMode())
            {
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, RefreshVisibility);
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshVisibility);
                _hasAddedEventListeners = true;
            }
            RefreshVisibility();
        }

        private void OnDestroy()
        {
            if (_hasAddedEventListeners)
            {
                _hasAddedEventListeners = false;
                GlobalEventManager.Instance.RemoveEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, RefreshVisibility);
                GlobalEventManager.Instance.RemoveEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshVisibility);
            }
        }

        public void RefreshVisibility()
        {
            base.gameObject.SetActive(MustShowTheObject());
        }

        public bool MustShowTheObject()
        {
            if (EnableIfWeaponVariant == WeaponVariant2.None)
                return true;

            WeaponVariant2 weaponVariant1;
            if (PersonalizationEditorManager.IsInEditorMode())
                weaponVariant1 = PersonalizationEditorManager.Instance.PreviewPresetKey;
            else
                WeaponVariantManager.GetWeaponVariant(PlacedObject.SpawnInfo.Reference.GetOwner(), PlacedObject.SpawnInfo.ItemInfo.Weapon, out weaponVariant1);

            return EnableIfWeaponVariant == weaponVariant1;
        }

        public void GetWeaponVariant(out WeaponVariant2 showConditions)
        {
            WeaponVariantManager.GetWeaponVariant(PlacedObject.SpawnInfo.Reference.GetOwner(), PlacedObject.SpawnInfo.ItemInfo.Weapon, out showConditions);
        }
    }
}
