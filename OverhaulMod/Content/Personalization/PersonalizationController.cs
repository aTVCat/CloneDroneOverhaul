using OverhaulMod.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationController : MonoBehaviour
    {
        private FirstPersonMover m_owner;
        public FirstPersonMover owner
        {
            get
            {
                if (!m_owner)
                {
                    m_owner = base.GetComponent<FirstPersonMover>();
                }
                return m_owner;
            }
        }

        private CharacterModel m_ownerModel;
        public CharacterModel ownerModel
        {
            get
            {
                if (!m_ownerModel)
                {
                    m_ownerModel = owner?.GetCharacterModel();
                }
                return m_ownerModel;
            }
        }

        private Dictionary<WeaponType, Transform[]> m_weaponTypeToParts;

        private void Start()
        {
            FirstPersonMover firstPersonMover = owner;
            if (!firstPersonMover || !firstPersonMover.IsAlive())
            {
                base.enabled = false;
                return;
            }

            if (firstPersonMover.HasCharacterModel())
            {
                Initialize();
            }
            else
            {
                _ = base.StartCoroutine(waitThenInitializeCoroutine(firstPersonMover));
            }
        }

        public void Initialize()
        {
            getWeaponRenderers();
        }

        public void SetWeaponPartsVisible(WeaponType weaponType, bool value)
        {
            if (!m_weaponTypeToParts.IsNullOrEmpty() && m_weaponTypeToParts.TryGetValue(weaponType, out Transform[] parts))
            {
                foreach (Transform transform in parts)
                    if (transform)
                        transform.gameObject.SetActive(value);
            }
        }

        private void getWeaponRenderers()
        {
            if (m_weaponTypeToParts == null)
            {
                m_weaponTypeToParts = new Dictionary<WeaponType, Transform[]>();
            }
            else
            {
                m_weaponTypeToParts.Clear();
            }

            CharacterModel characterModel = ownerModel;
            if (!characterModel || characterModel.WeaponModels.IsNullOrEmpty())
                return;

            foreach (WeaponModel weaponModel in characterModel.WeaponModels)
            {
                if (weaponModel && PersonalizationManager.IsWeaponCustomizationSupported(weaponModel.WeaponType))
                {
                    m_weaponTypeToParts.Add(weaponModel.WeaponType, weaponModel.PartsToDrop);
                }
            }
        }

        private IEnumerator waitThenInitializeCoroutine(FirstPersonMover firstPersonMover)
        {
            yield return new WaitUntil(() => !firstPersonMover || !firstPersonMover.IsAlive() || firstPersonMover.HasCharacterModel());
            for (int i = 0; i < 3; i++)
                yield return null;

            if (!firstPersonMover || !firstPersonMover.IsAlive())
            {
                base.enabled = false;
                yield break;
            }
            Initialize();
            yield break;
        }

        public Transform GetParentForItem(PersonalizationItemInfo personalizationItemInfo)
        {
            if (personalizationItemInfo.Category == PersonalizationCategory.Pets)
            {
                return base.transform.transform;
            }
            else if (personalizationItemInfo.Category == PersonalizationCategory.Accessories)
            {
                _ = TransformUtils.FindChildRecursive(base.transform, personalizationItemInfo.BodyPartName);
            }
            else if (personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins)
            {
                return ownerModel.GetWeaponModel(personalizationItemInfo.Weapon)?.transform;
            }
            return null;
        }
    }
}
