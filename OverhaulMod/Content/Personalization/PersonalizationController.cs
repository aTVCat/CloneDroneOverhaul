using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationController : MonoBehaviour
    {
        [ModSetting(ModSettingsConstants.SWORD_SKIN, null)]
        public static string SwordSkin;

        [ModSetting(ModSettingsConstants.BOW_SKIN, null)]
        public static string BowSkin;

        [ModSetting(ModSettingsConstants.HAMMER_SKIN, null)]
        public static string HammerSkin;

        [ModSetting(ModSettingsConstants.SPEAR_SKIN, null)]
        public static string SpearSkin;

        [ModSetting(ModSettingsConstants.SHIELD_SKIN, null)]
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

        private void Awake()
        {
            m_weaponTypeToParts = new Dictionary<WeaponType, Transform[]>();
            m_spawnedItems = new Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour>();
        }

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
            Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> dictionary = m_spawnedItems;
            if (dictionary == null || dictionary.Count == 0)
                return;

            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> kv in dictionary)
                DestroyItem(kv.Key, false);

            dictionary.Clear();
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

        public PersonalizationItemInfo GetItem(WeaponType weaponType)
        {
            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> keyValue in m_spawnedItems)
                if (keyValue.Key.Weapon == weaponType)
                    return keyValue.key;

            return null;
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

        public void EquipItem(PersonalizationItemInfo itemToEquip)
        {
            PersonalizationItemInfo info = null;
            foreach (PersonalizationItemInfo key in m_spawnedItems.Keys)
                if (key.Weapon == itemToEquip.Weapon)
                {
                    info = key;
                    break;
                }

            PersonalizationManager.SetIsItemEquipped(itemToEquip, true);

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

        public static void RefreshMainPlayer()
        {
            Character character = CharacterTracker.Instance.GetPlayer();
            if (character)
            {
                PersonalizationController personalizationController = character.GetComponent<PersonalizationController>();
                if (personalizationController)
                {
                    personalizationController.SpawnEquippedSkins();
                }
            }
        }

        public static void DestroyWeaponSkin(WeaponType weaponType)
        {
            Character character = CharacterTracker.Instance.GetPlayer();
            if (character)
            {
                PersonalizationController personalizationController = character.GetComponent<PersonalizationController>();
                if (personalizationController)
                {
                    personalizationController.DestroyItem(personalizationController.GetItem(weaponType));
                }
            }
        }

        public static void SetWeaponSkin(WeaponType weaponType, string itemId)
        {
            switch (weaponType)
            {
                case WeaponType.Sword:
                    SwordSkin = itemId;
                    break;
                case WeaponType.Bow:
                    BowSkin = itemId;
                    break;
                case WeaponType.Hammer:
                    HammerSkin = itemId;
                    break;
                case WeaponType.Spear:
                    SpearSkin = itemId;
                    break;
                case WeaponType.Shield:
                    ShieldSkin = itemId;
                    break;
            }
        }

        public static string GetWeaponSkin(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Sword:
                    return SwordSkin;
                case WeaponType.Bow:
                    return BowSkin;
                case WeaponType.Hammer:
                    return HammerSkin;
                case WeaponType.Spear:
                    return SpearSkin;
                case WeaponType.Shield:
                    return ShieldSkin;
            }
            return null;
        }
    }
}
