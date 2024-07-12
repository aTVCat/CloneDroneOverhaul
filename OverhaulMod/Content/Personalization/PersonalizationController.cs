using OverhaulMod.Engine;
using OverhaulMod.UI;
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

        private PersonalizationMultiplayerPlayerInfo m_playerInfo;
        public PersonalizationMultiplayerPlayerInfo playerInfo
        {
            get
            {
                FirstPersonMover firstPersonMover = owner;
                if (!firstPersonMover || !firstPersonMover.IsAlive() || !m_hasInitialized || !m_isMultiplayer || !m_isPlayer)
                    return null;

                if (m_playerInfo == null)
                {
                    m_playerInfo = PersonalizationMultiplayerManager.Instance.GetPlayInfo(owner.GetPlayFabID());
                }
                return m_playerInfo;
            }
        }

        private Dictionary<WeaponType, Transform[]> m_weaponTypeToParts;

        private Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> m_spawnedItems;

        private Dictionary<WeaponType, WeaponVariant> m_weaponTypeToVariant;

        private bool m_spawnSkinsNextFrame;

        private bool m_isEnemy;

        private bool m_isPlayer, m_isMainPlayer;

        private bool m_isMultiplayer;

        private bool m_hasStarted, m_hasInitialized, m_hasAddedEventListeners;

        private float m_timeLeftToRefreshWeaponParts;

        private void Awake()
        {
            m_weaponTypeToParts = new Dictionary<WeaponType, Transform[]>();
            m_weaponTypeToVariant = new Dictionary<WeaponType, WeaponVariant>();
            m_spawnedItems = new Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour>();
        }

        private void Start()
        {
            m_hasStarted = true;
            FirstPersonMover firstPersonMover = owner;
            if (!firstPersonMover || !firstPersonMover.IsAlive())
            {
                base.enabled = false;
                return;
            }

            if (!base.enabled || !base.gameObject.activeInHierarchy)
                return;

            _ = base.StartCoroutine(initializeCoroutine(firstPersonMover));
        }

        private void OnEnable()
        {
            if (m_hasStarted && !m_hasInitialized)
            {
                _ = base.StartCoroutine(initializeCoroutine(owner));
            }
        }

        private void OnDestroy()
        {
            if (m_hasAddedEventListeners)
            {
                m_hasAddedEventListeners = false;
                GlobalEventManager.Instance.RemoveEventListener<string>(PersonalizationMultiplayerManager.PLAYER_INFO_UPDATED_EVENT, onPlayerInfoUpdated);
            }
        }

        private void Update()
        {
            if (m_spawnSkinsNextFrame)
            {
                m_spawnSkinsNextFrame = false;
                SpawnEquippedSkins();
            }

            if (PersonalizationEditorManager.IsInEditor())
                return;

            m_timeLeftToRefreshWeaponParts -= Time.unscaledDeltaTime;
            if (m_timeLeftToRefreshWeaponParts <= 0f)
            {
                m_timeLeftToRefreshWeaponParts = 0.5f;

                WeaponType weaponType = owner.GetEquippedWeaponType();
                string skin = GetWeaponSkinDependingOnOwner(weaponType);
                bool noSkin = skin.IsNullOrEmpty() || skin == "_";

                bool hasSpawnedSkinForWeapon = false;
                foreach (PersonalizationItemInfo key in m_spawnedItems.Keys)
                    if (key.Weapon == weaponType)
                        hasSpawnedSkinForWeapon = true;

                if (!m_isMainPlayer && !noSkin && !hasSpawnedSkinForWeapon)
                {
                    //Debug.Log("Spawned an item because we didnt earlier");
                    _ = SpawnItem(skin);
                }

                SetWeaponPartsVisible(weaponType, noSkin || !hasSpawnedSkinForWeapon);
            }
        }

        private void onPlayerInfoUpdated(string playFabId)
        {
            if (playFabId == owner.GetPlayFabID())
            {
                SpawnEquippedSkinsNextFrame();
            }
        }

        private IEnumerator initializeCoroutine(FirstPersonMover firstPersonMover)
        {
            while (firstPersonMover && firstPersonMover.IsAlive() && !firstPersonMover.IsInitialized())
                yield return null;

            if (!firstPersonMover || !firstPersonMover.IsAlive())
            {
                Destroy(this);
                yield break;
            }

            if (GameModeManager.IsMultiplayer())
            {
                m_isMultiplayer = true;
                if (firstPersonMover.state.IsAIControlled)
                {
                    m_isEnemy = true;
                    m_isPlayer = false;
                }
                else
                {
                    m_isEnemy = false;
                    m_isPlayer = true;
                    m_isMainPlayer = firstPersonMover.IsMainPlayer();

                    float timeOut = Time.time + 1f;
                    while (Time.time < timeOut && firstPersonMover.GetPlayFabID().IsNullOrEmpty())
                        yield return null;
                }
            }
            else
            {
                m_isMultiplayer = false;
                m_isEnemy = !firstPersonMover.IsMainPlayer();
                m_isPlayer = !m_isEnemy;
                m_isMainPlayer = !m_isEnemy;
            }

            m_hasInitialized = true;
            GlobalEventManager.Instance.AddEventListener<string>(PersonalizationMultiplayerManager.PLAYER_INFO_UPDATED_EVENT, onPlayerInfoUpdated);
            m_hasAddedEventListeners = true;
            SpawnEquippedSkinsNextFrame();
            yield break;
        }

        public bool ShouldRefreshSkinOfWeapon(WeaponType weaponType)
        {
            return GetWeaponVariant(weaponType) != GetWeaponVariant(weaponType, false);
        }

        public WeaponVariant GetWeaponVariant(WeaponType weaponType, bool getCached = true)
        {
            if (!getCached)
            {
                WeaponVariantManager.GetWeaponVariant(owner, weaponType, out WeaponVariant weaponVariant);
                return weaponVariant;
            }

            Dictionary<WeaponType, WeaponVariant> d = m_weaponTypeToVariant;
            if (d.ContainsKey(weaponType))
                return d[weaponType];

            return WeaponVariant.None;
        }

        public void RefreshWeaponVariant(WeaponType weaponType)
        {
            Dictionary<WeaponType, WeaponVariant> d = m_weaponTypeToVariant;
            WeaponVariantManager.GetWeaponVariant(owner, weaponType, out WeaponVariant weaponVariant);

            if (d.ContainsKey(weaponType))
                d[weaponType] = weaponVariant;
            else
                d.Add(weaponType, weaponVariant);
        }

        public void SetWeaponPartsVisible(WeaponType weaponType, bool value)
        {
            if (!m_weaponTypeToParts.IsNullOrEmpty() && m_weaponTypeToParts.TryGetValue(weaponType, out Transform[] parts))
            {
                foreach (Transform transform in parts)
                    if (transform && !(weaponType == WeaponType.Bow && transform.parent && (transform.parent.name == "BowStringUpper" || transform.parent.name == "BowStringLower")))
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

            RefreshWeaponVariant(personalizationItemInfo.Weapon);

            if (personalizationItemInfo.Weapon == WeaponType.Bow && (owner.CharacterType == EnemyType.ZombieArcher1)) // todo: check if glock 18 mod is enabled
                return null;

            Transform transform = GetParentForItem(personalizationItemInfo);
            if (!transform)
                return null;

            PersonalizationEditorObjectBehaviour behaviour = personalizationItemInfo.RootObject.Deserialize(transform, new PersonalizationControllerInfo(this, personalizationItemInfo));
            if (!behaviour)
                return null;

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

        public PersonalizationItemInfo GetItem(WeaponType weaponType)
        {
            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> keyValue in m_spawnedItems)
                if (keyValue.Key.Weapon == weaponType)
                    return keyValue.key;

            return null;
        }

        public void RefreshWeaponRenderers()
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

        public void SpawnEquippedSkinsNextFrame()
        {
            m_spawnSkinsNextFrame = true;
        }

        public void SpawnEquippedSkins()
        {
            if (PersonalizationEditorManager.IsInEditor())
                return;

            DestroyAllItems();
            RefreshWeaponRenderers();

            if (GameModeManager.IsMultiplayer() && !owner.IsMainPlayer())
                return;

            string swordSkin = GetWeaponSkinDependingOnOwner(WeaponType.Sword);
            string bowSkin = GetWeaponSkinDependingOnOwner(WeaponType.Bow);
            string hammerSkin = GetWeaponSkinDependingOnOwner(WeaponType.Hammer);
            string spearSkin = GetWeaponSkinDependingOnOwner(WeaponType.Spear);
            string shieldSkin = GetWeaponSkinDependingOnOwner(WeaponType.Shield);

            _ = SpawnItem(swordSkin);
            _ = SpawnItem(bowSkin);
            _ = SpawnItem(hammerSkin);
            _ = SpawnItem(spearSkin);
            _ = SpawnItem(shieldSkin);
        }

        public void RespawnSkinsIfRequired()
        {
            Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> d = m_spawnedItems;
            if (d.Count == 0)
            {
                SpawnEquippedSkins();
            }
            else
            {
                RefreshWeaponRenderers();

                List<PersonalizationItemInfo> skinsToRespawn = null;
                foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> kv in d)
                {
                    if (ShouldRefreshSkinOfWeapon(kv.Key.Weapon))
                    {
                        if (skinsToRespawn == null)
                            skinsToRespawn = new List<PersonalizationItemInfo>();

                        skinsToRespawn.Add(kv.Key);
                    }
                }

                if (skinsToRespawn != null)
                    foreach (PersonalizationItemInfo info in skinsToRespawn)
                    {
                        DestroyItem(info);
                        _ = SpawnItem(GetWeaponSkinDependingOnOwner(info.Weapon));
                    }
            }
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

        public string GetWeaponSkinDependingOnOwner(WeaponType weaponType)
        {
            if (!m_hasInitialized)
                return null;

            if (m_isMainPlayer && UIPersonalizationItemsBrowser.IsPreviewing)
                return GetWeaponSkin(weaponType);

            if (m_isEnemy || (m_isPlayer && !m_isMultiplayer))
                return GetWeaponSkin(weaponType);
            else if (m_isMultiplayer)
            {
                PersonalizationMultiplayerPlayerInfo multiplayerPlayerInfo = playerInfo;
                if (multiplayerPlayerInfo == null)
                    return string.Empty;

                switch (weaponType)
                {
                    case WeaponType.Sword:
                        return multiplayerPlayerInfo.SwordSkin;
                    case WeaponType.Bow:
                        return multiplayerPlayerInfo.BowSkin;
                    case WeaponType.Hammer:
                        return multiplayerPlayerInfo.HammerSkin;
                    case WeaponType.Spear:
                        return multiplayerPlayerInfo.SpearSkin;
                    case WeaponType.Shield:
                        return multiplayerPlayerInfo.ShieldSkin;
                }
            }
            return string.Empty;
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
                    ModSettingsManager.SetStringValue(ModSettingsConstants.SWORD_SKIN, itemId);
                    break;
                case WeaponType.Bow:
                    ModSettingsManager.SetStringValue(ModSettingsConstants.BOW_SKIN, itemId);
                    break;
                case WeaponType.Hammer:
                    ModSettingsManager.SetStringValue(ModSettingsConstants.HAMMER_SKIN, itemId);
                    break;
                case WeaponType.Spear:
                    ModSettingsManager.SetStringValue(ModSettingsConstants.SPEAR_SKIN, itemId);
                    break;
                case WeaponType.Shield:
                    ModSettingsManager.SetStringValue(ModSettingsConstants.SHIELD_SKIN, itemId);
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
