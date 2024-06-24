using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectVisibilityController : PersonalizationEditorObjectComponentBase
    {
        [PersonalizationEditorObjectProperty]
        public WeaponVariant enableIfWeaponVariant
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return (WeaponVariant)ob.GetPropertyValue(nameof(enableIfWeaponVariant), 0);
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
            if (enableIfWeaponVariant == WeaponVariant.None)
                return true;

            WeaponVariant weaponVariant1;
            if (PersonalizationEditorManager.IsInEditor())
                weaponVariant1 = PersonalizationEditorManager.Instance.previewPresetKey;
            else
                WeaponVariantManager.GetWeaponVariant(objectBehaviour.ControllerInfo.Reference.owner, objectBehaviour.ControllerInfo.ItemInfo.Weapon, out weaponVariant1);

            return enableIfWeaponVariant == weaponVariant1;
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
