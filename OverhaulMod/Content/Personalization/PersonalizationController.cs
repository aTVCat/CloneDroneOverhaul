using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationController : MonoBehaviour
    {
        [ModSetting(ModSettingsConstants.SWORD_SKIN, "")]
        public static string SwordSkin;

        [ModSetting(ModSettingsConstants.BOW_SKIN, "")]
        public static string BowSkin;

        [ModSetting(ModSettingsConstants.HAMMER_SKIN, "")]
        public static string HammerSkin;

        [ModSetting(ModSettingsConstants.SPEAR_SKIN, "")]
        public static string SpearSkin;

        [ModSetting(ModSettingsConstants.SHIELD_SKIN, "")]
        public static string ShieldSkin;

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

        private Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> m_spawnedItems;

        private void Start()
        {
            m_weaponTypeToParts = new Dictionary<WeaponType, Transform[]>();
            m_spawnedItems = new Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour>();

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
            SpawnEquippedSkins();
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

        public bool HasEquippedSkinOnWeapon(WeaponType weaponType)
        {
            if (GameModeManager.IsMultiplayer() && !owner.IsMainPlayer())
                return false;

            switch (weaponType)
            {
                case WeaponType.Sword:
                    return !SwordSkin.IsNullOrEmpty();
                case WeaponType.Bow:
                    return !BowSkin.IsNullOrEmpty();
                case WeaponType.Hammer:
                    return !HammerSkin.IsNullOrEmpty();
                case WeaponType.Spear:
                    return !SpearSkin.IsNullOrEmpty();
                case WeaponType.Shield:
                    return !ShieldSkin.IsNullOrEmpty();
            }
            return false;
        }

        public PersonalizationEditorObjectBehaviour SpawnItem(string itemId)
        {
            if (itemId.IsNullOrEmpty())
                return null;

            return SpawnItem(PersonalizationManager.Instance.itemList.GetItem(itemId));
        }

        public PersonalizationEditorObjectBehaviour SpawnItem(PersonalizationItemInfo personalizationItemInfo)
        {
            if (personalizationItemInfo == null || personalizationItemInfo.RootObject == null || HasSpawnedItem(personalizationItemInfo))
                return null;

            Transform transform = GetParentForItem(personalizationItemInfo);
            PersonalizationEditorObjectBehaviour behaviour = personalizationItemInfo.RootObject.Deserialize(transform);
            m_spawnedItems.Add(personalizationItemInfo, behaviour);

            if (personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins)
                SetWeaponPartsVisible(personalizationItemInfo.Weapon, false);

            return behaviour;
        }

        public void DestroyItem(PersonalizationItemInfo personalizationItemInfo, bool editCollection = true)
        {
            if (personalizationItemInfo == null || !HasSpawnedItem(personalizationItemInfo))
                return;

            if (personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins)
                SetWeaponPartsVisible(personalizationItemInfo.Weapon, true);

            PersonalizationEditorObjectBehaviour b = m_spawnedItems[personalizationItemInfo];
            if (b)
                Destroy(b.gameObject);

            if (editCollection)
                _ = m_spawnedItems.Remove(personalizationItemInfo);
        }

        public void DestroyAllItems()
        {
            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> kv in m_spawnedItems)
                DestroyItem(kv.Key, false);

            m_spawnedItems.Clear();
        }

        public bool HasSpawnedItem(PersonalizationItemInfo personalizationItemInfo)
        {
            return m_spawnedItems.ContainsKey(personalizationItemInfo);
        }

        public bool HasSpawnedItem(string id)
        {
            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> info in m_spawnedItems)
                if (info.Key.ItemID == id)
                    return true;

            return false;
        }

        private void getWeaponRenderers()
        {
            m_weaponTypeToParts.Clear();
            CharacterModel characterModel = ownerModel;
            if (!characterModel || characterModel.WeaponModels.IsNullOrEmpty())
                return;

            foreach (WeaponModel weaponModel in characterModel.WeaponModels)
            {
                if (!weaponModel)
                    continue;

                WeaponType weaponType = weaponModel.WeaponType;
                if (PersonalizationManager.IsWeaponCustomizationSupported(weaponType))
                    if (m_weaponTypeToParts.ContainsKey(weaponType))
                        m_weaponTypeToParts[weaponType] = weaponModel.PartsToDrop;
                    else
                        m_weaponTypeToParts.Add(weaponModel.WeaponType, weaponModel.PartsToDrop);
            }
        }

        public void SpawnEquippedSkins()
        {
            DestroyAllItems();

            if (GameModeManager.IsMultiplayer() && !owner.IsMainPlayer())
                return;

            string swordSkin = SwordSkin;
            string bowSkin = BowSkin;
            string hammerSkin = HammerSkin;
            string spearSkin = SpearSkin;
            string shieldSkin = ShieldSkin;

            _ = SpawnItem(swordSkin);
            _ = SpawnItem(bowSkin);
            _ = SpawnItem(hammerSkin);
            _ = SpawnItem(spearSkin);
            _ = SpawnItem(shieldSkin);
        }

        public void EquipSkin(PersonalizationItemInfo itemToEquip)
        {
            PersonalizationItemInfo info = null;
            foreach (PersonalizationItemInfo key in m_spawnedItems.Keys)
                if (key.Weapon == itemToEquip.Weapon)
                {
                    info = key;
                    break;
                }

            string id = itemToEquip.ItemID;
            switch (itemToEquip.Weapon)
            {
                case WeaponType.Sword:
                    SwordSkin = id;
                    break;
                case WeaponType.Bow:
                    BowSkin = id;
                    break;
                case WeaponType.Hammer:
                    HammerSkin = id;
                    break;
                case WeaponType.Spear:
                    SpearSkin = id;
                    break;
                case WeaponType.Shield:
                    ShieldSkin = id;
                    break;
            }

            DestroyItem(info);
            _ = SpawnItem(itemToEquip);
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
