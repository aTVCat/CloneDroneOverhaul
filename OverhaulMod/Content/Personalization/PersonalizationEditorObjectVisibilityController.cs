using OverhaulMod.Engine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectVisibilityController : PersonalizationEditorObjectComponentBase
    {
        public WeaponVariant2 enableIfWeaponVariant
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return (WeaponVariant2)ob.GetPropertyValue(nameof(enableIfWeaponVariant), 0);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(enableIfWeaponVariant), (int)value);
            }
        }

        private bool m_hasAddedEventListeners;

        private void Start()
        {
            if (PersonalizationEditorManager.IsInEditor())
            {
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, RefreshVisibility);
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshVisibility);
                m_hasAddedEventListeners = true;
            }
            RefreshVisibility();
        }

        private void OnDestroy()
        {
            if (m_hasAddedEventListeners)
            {
                m_hasAddedEventListeners = false;
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
            if (enableIfWeaponVariant == WeaponVariant2.None)
                return true;

            WeaponVariant2 weaponVariant1;
            if (PersonalizationEditorManager.IsInEditor())
                weaponVariant1 = PersonalizationEditorManager.Instance.previewPresetKey;
            else
                WeaponVariantManager.GetWeaponVariant(objectBehaviour.ControllerInfo.Reference.owner, objectBehaviour.ControllerInfo.ItemInfo.Weapon, out weaponVariant1);

            return enableIfWeaponVariant == weaponVariant1;
        }

        public void GetWeaponVariant(out WeaponVariant2 showConditions)
        {
            WeaponVariantManager.GetWeaponVariant(objectBehaviour.ControllerInfo.Reference.owner, objectBehaviour.ControllerInfo.ItemInfo.Weapon, out showConditions);
        }

        public void GetWeaponVariant(out bool isOnFire, out bool isGreatSword)
        {
            WeaponVariantManager.GetWeaponVariant(objectBehaviour.ControllerInfo.Reference.owner, objectBehaviour.ControllerInfo.ItemInfo.Weapon, out isOnFire, out isGreatSword);
        }
    }
}
