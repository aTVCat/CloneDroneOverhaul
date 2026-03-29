using OverhaulMod.Combat;
using OverhaulMod.Combat.Weapons;
using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationController : MonoBehaviour
    {
        private FirstPersonMover _owner;
        public FirstPersonMover owner
        {
            get
            {
                if (!_owner)
                {
                    _owner = base.GetComponent<FirstPersonMover>();
                }
                return _owner;
            }
        }

        private CharacterModel _ownerModel;
        public CharacterModel ownerModel
        {
            get
            {
                if (!_ownerModel)
                {
                    _ownerModel = owner?.GetCharacterModel();
                }
                return _ownerModel;
            }
        }

        private PersonalizationMultiplayerPlayerInfo _playerInfo;
        public PersonalizationMultiplayerPlayerInfo playerInfo
        {
            get
            {
                FirstPersonMover firstPersonMover = owner;
                if (!firstPersonMover || !firstPersonMover.IsAlive() || !_hasInitialized || !_isMultiplayer || !_isPlayer)
                    return null;

                if (_playerInfo == null)
                {
                    _playerInfo = PersonalizationMultiplayerManager.Instance.GetPlayInfo(owner.GetPlayFabID());
                }
                return _playerInfo;
            }
        }

        private Dictionary<WeaponType, Transform[]> _weaponTypeToParts;

        private Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> _spawnedItems;

        private Dictionary<WeaponType, WeaponVariant2> _weaponTypeToVariant;

        private Transform _defaultArrowSpawnPoint, _arrowSpawnPoint;

        private bool _isEnemy;

        private bool _isPlayer, _isMainPlayer, _isMindSpace;

        private bool _isMultiplayer;

        private bool _hasInitialized, _hasAddedEventListeners;

        private float _timeLeftToRefreshSkins;

        private bool _hasOwnerDied;

        private void Awake()
        {
            _weaponTypeToParts = new Dictionary<WeaponType, Transform[]>();
            _weaponTypeToVariant = new Dictionary<WeaponType, WeaponVariant2>();
            _spawnedItems = new Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour>();

            if (PersonalizationEditorManager.IsInEditor())
                PersonalizationEditorManager.Instance.currentPersonalizationController = this;
        }

        private void Start()
        {
            FirstPersonMover firstPersonMover = owner;
            if (!firstPersonMover || !firstPersonMover.IsAlive())
            {
                _hasOwnerDied = true;
                return;
            }

            _hasOwnerDied = !firstPersonMover.IsAlive();
            if (!_hasOwnerDied)
            {
                firstPersonMover.AddDeathListener(delegate
                {
                    _hasOwnerDied = true;
                });
            }
        }

        private void OnEnable()
        {
            FirstPersonMover firstPersonMover = owner;
            if (!firstPersonMover || !firstPersonMover.IsAlive())
            {
                base.enabled = false;
                return;
            }

            if (!_hasInitialized)
            {
                _ = base.StartCoroutine(initializeCoroutine(firstPersonMover));
            }
        }

        private void OnDestroy()
        {
            if (_hasAddedEventListeners)
            {
                GlobalEventManager.Instance.RemoveEventListener(PersonalizationManager.ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT, onItemEquippedOrUnequipped);
                GlobalEventManager.Instance.RemoveEventListener<string>(PersonalizationMultiplayerManager.PLAYER_INFO_UPDATED_EVENT, onPlayerInfoUpdated);
                _hasAddedEventListeners = false;
            }
        }

        private void Update()
        {
            if (!_hasInitialized || _hasOwnerDied || _isMindSpace)
                return;

            _timeLeftToRefreshSkins = Mathf.Max(0f, _timeLeftToRefreshSkins - Time.deltaTime);
            if (_timeLeftToRefreshSkins == 0f)
            {
                _timeLeftToRefreshSkins = 0.5f;
                RefreshWeaponSkins();
                RefreshWeaponSkinsVisibility();
            }
        }

        private IEnumerator initializeCoroutine(FirstPersonMover firstPersonMover)
        {
            yield return null;

            while (firstPersonMover && firstPersonMover.IsAttachedAndAlive() && (!firstPersonMover.IsInitialized() || !firstPersonMover.HasCharacterModel()))
                yield return null;

            yield return null;

            if (!firstPersonMover || !firstPersonMover.IsAttachedAndAlive() || !firstPersonMover.IsInitialized() || !firstPersonMover.HasCharacterModel())
            {
                Destroy(this);
                yield break;
            }

            _defaultArrowSpawnPoint = firstPersonMover.GetCharacterModel().ArrowHolder;
            _arrowSpawnPoint = null;

            RefreshOwnerInfo();

            if (_isMultiplayer && !_isEnemy)
            {
                float timeOut = Time.time + 3f;
                while (Time.time < timeOut && firstPersonMover.GetPlayFabID().IsNullOrEmpty())
                    yield return null;
            }

            GlobalEventManager.Instance.AddEventListener(PersonalizationManager.ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT, onItemEquippedOrUnequipped);
            GlobalEventManager.Instance.AddEventListener<string>(PersonalizationMultiplayerManager.PLAYER_INFO_UPDATED_EVENT, onPlayerInfoUpdated);
            _hasAddedEventListeners = true;
            _hasInitialized = true;

            RefreshWeaponRenderers();
            RefreshWeaponSkinsNextFrame();
            SpawnEquippedAccessories();
            yield break;
        }

        public void RefreshOwnerInfo()
        {
            FirstPersonMover firstPersonMover = _owner;
            if (!firstPersonMover) return;

            _isMultiplayer = GameModeManager.IsMultiplayer();
            if (firstPersonMover.state.IsAIControlled)
            {
                _isEnemy = true;
                _isPlayer = false;
                _isMainPlayer = false;
            }
            else
            {
                _isEnemy = false;
                _isPlayer = true;
                _isMainPlayer = firstPersonMover.IsMainPlayer();
            }

            _isMindSpace = firstPersonMover.IsMindSpaceCharacter;
        }

        public void RefreshWeaponSkinsNextFrame()
        {
            _timeLeftToRefreshSkins = 0f;
        }

        public void RefreshWeaponSkins()
        {
            FirstPersonMover firstPersonMover = owner;
            if (!firstPersonMover) return;

            bool inEditor = PersonalizationEditorManager.IsInEditor();
            bool showOriginalModel = inEditor && PersonalizationEditorManager.Instance.originalModelsEnabled;

            WeaponType weaponType = firstPersonMover.GetEquippedWeaponType();
            string skin = GetWeaponSkinDependingOnOwner(weaponType);
            bool noSkin = skin.IsNullOrEmpty() || skin == "_";

            PersonalizationEditorObjectBehaviour behaviour = null;
            PersonalizationItemInfo personalizationItemInfo = null;
            bool hasSpawnedSkinForWeapon = false;
            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> kv in _spawnedItems)
            {
                PersonalizationItemInfo key = kv.Key;
                if (key.Category != PersonalizationCategory.WeaponSkins) continue;

                behaviour = kv.Value;
                if (key.Weapon == weaponType)
                {
                    personalizationItemInfo = key;
                    hasSpawnedSkinForWeapon = behaviour;
                }
            }

            if (!PersonalizationEditorManager.IsInEditor())
            {
                if (!noSkin && !hasSpawnedSkinForWeapon)
                {
                    //Debug.Log("Spawned an item because we didnt earlier");
                    behaviour = SpawnItem(skin);
                    if (behaviour)
                    {
                        personalizationItemInfo = behaviour.ControllerInfo?.ItemInfo;
                        hasSpawnedSkinForWeapon = true;
                    }
                    else
                    {
                        hasSpawnedSkinForWeapon = false;
                    }
                }
                else if (noSkin && hasSpawnedSkinForWeapon)
                {
                    DestroyItem(personalizationItemInfo);
                    hasSpawnedSkinForWeapon = false;
                }
            }

            if (inEditor && weaponType == WeaponType.Sword)
            {
                WeaponVariant2 wv = PersonalizationEditorManager.Instance.previewPresetKey;
                SetWeaponPartsVisible(WeaponType.Sword, showOriginalModel || (!hasSpawnedSkinForWeapon && !(wv == WeaponVariant2.NormalMultiplayer || wv == WeaponVariant2.OnFireMultiplayer)), false);
            }
            else
            {
                SetWeaponPartsVisible(weaponType, showOriginalModel || !hasSpawnedSkinForWeapon, (personalizationItemInfo != null && personalizationItemInfo.HideBowStrings));
            }
        }

        public void RefreshWeaponSkinsVisibility()
        {
            FirstPersonMover firstPersonMover = owner;
            if (!firstPersonMover) return;

            PersonalizationEditorObjectBehaviour weaponSkinObject = GetSpawnedWeaponSkin(WeaponType.Bow);
            if (weaponSkinObject && weaponSkinObject.ControllerInfo.ItemInfo != null && !weaponSkinObject.ControllerInfo.ItemInfo.OverrideParent.IsNullOrEmpty())
            {
                weaponSkinObject.gameObject.SetActive(firstPersonMover.GetEquippedWeaponType() == WeaponType.Bow);
            }
        }

        public void RespawnWeaponSkinsIfRequired()
        {
            Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> d = _spawnedItems;
            if (d.Count == 0) return;

            List<PersonalizationItemInfo> skinsToRespawn = null;
            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> kv in d)
            {
                if (kv.Key.Category == PersonalizationCategory.WeaponSkins && ShouldRefreshSkinOfWeapon(kv.Key.Weapon))
                {
                    if (skinsToRespawn == null)
                        skinsToRespawn = new List<PersonalizationItemInfo>() { kv.Key };
                    else
                        skinsToRespawn.Add(kv.Key);
                }
            }

            if (skinsToRespawn == null) return;
            foreach (PersonalizationItemInfo info in skinsToRespawn)
            {
                DestroyItem(info);
                RefreshRenderersOfWeapon(ownerModel.GetWeaponModel(info.Weapon));
                _ = SpawnItem(GetWeaponSkinDependingOnOwner(info.Weapon));
            }
        }

        public void RefreshArrowSpawnPoint()
        {
            if (!_isEnemy)
            {
                PersonalizationEditorObjectBehaviour item = GetSpawnedWeaponSkin(WeaponType.Bow);
                if (!item)
                {
                    _arrowSpawnPoint = null;
                }
                else
                {
                    PersonalizationEditorObjectArrowSpawnPoint spawnPoint = item.GetComponentInChildren<PersonalizationEditorObjectArrowSpawnPoint>();
                    _arrowSpawnPoint = spawnPoint ? spawnPoint.transform : null;
                }
            }
            else
            {
                _arrowSpawnPoint = null;
            }
            RefreshArrowHolderReference();
        }

        public void RefreshArrowHolderReference()
        {
            CharacterModel characterModel = ownerModel;
            if (characterModel)
            {
                characterModel.ArrowHolder = _arrowSpawnPoint ?? _defaultArrowSpawnPoint;
            }
        }

        public void RefreshWeaponRenderers()
        {
            _weaponTypeToParts.Clear();
            CharacterModel characterModel = ownerModel;
            if (!characterModel || characterModel.WeaponModels.IsNullOrEmpty()) return;

            foreach (WeaponModel weaponModel in characterModel.WeaponModels)
            {
                if (weaponModel) RefreshRenderersOfWeapon(weaponModel);
            }
        }

        public void RefreshRenderersOfWeapon(WeaponModel weaponModel)
        {
            WeaponType weaponType = weaponModel.WeaponType;
            if (PersonalizationManager.IsWeaponCustomizationSupported(weaponType))
                if (_weaponTypeToParts.ContainsKey(weaponType))
                    _weaponTypeToParts[weaponType] = weaponModel.PartsToDrop;
                else
                    _weaponTypeToParts.Add(weaponModel.WeaponType, weaponModel.PartsToDrop);
        }

        public bool ShouldRefreshSkinOfWeapon(WeaponType weaponType)
        {
            return GetWeaponVariantOfSpawnedSkin(weaponType) != GetWeaponVariant(weaponType);
        }

        public void RefreshVariantOfWeapon(WeaponType weaponType)
        {
            Dictionary<WeaponType, WeaponVariant2> d = _weaponTypeToVariant;
            WeaponVariantManager.GetWeaponVariant(owner, weaponType, out WeaponVariant2 weaponVariant);

            if (d.ContainsKey(weaponType))
                d[weaponType] = weaponVariant;
            else
                d.Add(weaponType, weaponVariant);
        }

        public WeaponVariant2 GetWeaponVariantOfSpawnedSkin(WeaponType weaponType)
        {
            Dictionary<WeaponType, WeaponVariant2> d = _weaponTypeToVariant;
            if (d.ContainsKey(weaponType))
                return d[weaponType];

            return WeaponVariant2.None;
        }

        public WeaponVariant2 GetWeaponVariant(WeaponType weaponType)
        {
            WeaponVariantManager.GetWeaponVariant(owner, weaponType, out WeaponVariant2 weaponVariant);
            return weaponVariant;
        }

        public void SetWeaponPartsVisible(WeaponType weaponType, bool value, bool hideBowStrings)
        {
            if (weaponType == ModWeaponsManager.SCYTHE_TYPE)
            {
                WeaponModel weaponModel = ownerModel.GetWeaponModel(ModWeaponsManager.SCYTHE_TYPE);
                if (weaponModel && weaponModel is ModWeaponModel modWeaponModel)
                {
                    modWeaponModel.SetIsModelActive(value);
                }
                return;
            }

            if (!_weaponTypeToParts.IsNullOrEmpty() && _weaponTypeToParts.TryGetValue(weaponType, out Transform[] parts))
            {
                bool isBow = weaponType == WeaponType.Bow;
                foreach (Transform transform in parts)
                    if (transform)
                    {
                        if (isBow)
                        {
                            if (ModSpecialUtils.IsModEnabled("ee32ba1b-8c92-4f50-bdf4-400a14da829e")) // dont make changes to bow to not break glock mod
                                continue;

                            if (transform.parent && (transform.parent.name == "BowStringUpper" || transform.parent.name == "BowStringLower"))
                            {
                                transform.gameObject.SetActive(!hideBowStrings);
                                continue;
                            }
                        }

                        transform.gameObject.SetActive(value);
                    }
            }
        }

        public void SetBowStringsWidth(float value)
        {
            if (!_weaponTypeToParts.IsNullOrEmpty() && _weaponTypeToParts.TryGetValue(WeaponType.Bow, out Transform[] parts))
            {
                foreach (Transform transform in parts)
                    if (transform && transform.parent && (transform.parent.name == "BowStringUpper" || transform.parent.name == "BowStringLower"))
                    {
                        transform.localScale = new Vector3(0.1f * value, transform.localScale.y, 0.1f * value);
                    }
            }
        }

        public void SpawnEquippedAccessories()
        {
            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.Accessories) || PersonalizationEditorManager.IsInEditor())
                return;

            DestroyItemsOfCategory(PersonalizationCategory.Accessories);

            if (!owner.IsMainPlayer() && (!PersonalizationUserInfo.AllowEnemiesUseSkins || GameModeManager.IsMultiplayer()))
                return;

            List<string> accessories = PersonalizationUserInfo.GetEquippedAccessories();
            foreach (string item in accessories)
            {
                _ = SpawnItem(item);
            }
        }

        public void EquipItem(PersonalizationItemInfo itemToEquip)
        {
            DestroyItem(GetItemInfoOfSameType(itemToEquip));
            _ = SpawnItem(itemToEquip);
        }

        public PersonalizationEditorObjectBehaviour SpawnItem(string itemId)
        {
            if (itemId.IsNullOrEmpty())
                return null;

            return SpawnItem(PersonalizationManager.Instance.itemList.GetItem(itemId));
        }

        public PersonalizationEditorObjectBehaviour SpawnItem(PersonalizationItemInfo itemInfo)
        {
            bool inEditor = PersonalizationEditorManager.IsInEditor();
            if (itemInfo == null || !owner || (!inEditor && !itemInfo.IsUnlocked(owner)) || itemInfo.RootObject == null || HasSpawnedItem(itemInfo))
                return null;

            if (itemInfo.Category == PersonalizationCategory.WeaponSkins)
                RefreshVariantOfWeapon(itemInfo.Weapon);

            if (!inEditor)
            {
                EnemyType enemyType = owner.CharacterType;
                if (owner.IsMindSpaceCharacter || enemyType == EnemyType.ZombieArcher1 || enemyType == EnemyType.FleetAnalysisBot1 || enemyType == EnemyType.FleetAnalysisBot2 || enemyType == EnemyType.FleetAnalysisBot3 || enemyType == EnemyType.FleetAnalysisBot4 || (itemInfo.Category == PersonalizationCategory.WeaponSkins && itemInfo.Weapon == WeaponType.Bow && ModSpecialUtils.IsModEnabled("ee32ba1b-8c92-4f50-bdf4-400a14da829e")))
                    return null;
            }

            Transform transform = GetParentForItem(itemInfo);
            if (!transform)
                return null;

            MechBodyPart bodyPartForAccessory = null;
            if (itemInfo.Category == PersonalizationCategory.Accessories)
            {
                List<MechBodyPart> bodyParts = owner.GetAllBodyParts();
                if (bodyParts != null && bodyParts.Count != 0)
                {
                    foreach (MechBodyPart bodyPart in bodyParts)
                    {
                        if (!bodyPart || !bodyPart.transform)
                            continue;

                        Transform parent = bodyPart.transform.parent;
                        if (parent && parent.name == itemInfo.BodyPartName)
                        {
                            bodyPartForAccessory = bodyPart;
                            break;
                        }
                    }
                }

                if (!bodyPartForAccessory)
                    return null;
            }

            PersonalizationEditorObjectBehaviour behaviour = itemInfo.RootObject.Deserialize(transform, new PersonalizationControllerInfo(this, itemInfo));
            if (!behaviour)
            {
                _spawnedItems.Add(itemInfo, null);
                return null;
            }
            _spawnedItems.Add(itemInfo, behaviour);


            if (itemInfo.Category == PersonalizationCategory.WeaponSkins)
            {
                WeaponModel weaponModel = ownerModel.GetWeaponModel(itemInfo.Weapon);
                if (weaponModel && !weaponModel.PartsToDrop.Contains(behaviour.transform))
                {
                    List<Transform> list = weaponModel.PartsToDrop.ToList();
                    list.Add(behaviour.transform);
                    weaponModel.PartsToDrop = list.ToArray();
                }

                if (itemInfo.Weapon == WeaponType.Bow)
                {
                    SetBowStringsWidth(Mathf.Clamp(itemInfo.BowStringsWidth, 0.1f, 1f));
                    RefreshArrowSpawnPoint();
                }
                RefreshWeaponSkinsNextFrame();
            }
            else if (itemInfo.Category == PersonalizationCategory.Accessories)
            {
                PersonalizationAccessoryBehaviour personalizationAccessoryBehaviour = behaviour.gameObject.AddComponent<PersonalizationAccessoryBehaviour>();
                personalizationAccessoryBehaviour.SetItemObject(behaviour);
                personalizationAccessoryBehaviour.SetBodyPart(bodyPartForAccessory);
                personalizationAccessoryBehaviour.RefreshVisibility();
                personalizationAccessoryBehaviour.Register();
            }
            return behaviour;
        }

        public void DestroyItem(PersonalizationItemInfo personalizationItemInfo, bool editCollection = true)
        {
            if (personalizationItemInfo == null || !HasSpawnedItem(personalizationItemInfo))
                return;

            PersonalizationEditorObjectBehaviour behaviour = _spawnedItems[personalizationItemInfo];
            if (!behaviour) return;

            if (personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins)
                RefreshWeaponSkinsNextFrame();

            if (personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins)
            {
                WeaponModel weaponModel = ownerModel.GetWeaponModel(behaviour.ControllerInfo.ItemInfo.Weapon);
                if (weaponModel && weaponModel.PartsToDrop.Contains(behaviour.transform))
                {
                    List<Transform> list = weaponModel.PartsToDrop.ToList();
                    list.Remove(behaviour.transform);
                    weaponModel.PartsToDrop = list.ToArray();
                }

                if (personalizationItemInfo.Weapon == WeaponType.Bow)
                {
                    SetBowStringsWidth(1f);
                    RefreshArrowSpawnPoint();
                }
            }

            Destroy(behaviour.gameObject);

            if (editCollection)
                _ = _spawnedItems.Remove(personalizationItemInfo);
        }

        public void DestroyAllItems()
        {
            Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> dictionary = _spawnedItems;
            if (dictionary == null || dictionary.Count == 0)
                return;

            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> kv in dictionary)
                DestroyItem(kv.Key, false);

            dictionary.Clear();
        }

        public void DestroyItemsOfCategory(PersonalizationCategory personalizationCategory)
        {
            Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> dictionary = _spawnedItems;
            if (dictionary == null || dictionary.Count == 0)
                return;

            List<PersonalizationItemInfo> toRemove = null;
            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> kv in dictionary)
                if (kv.Key.Category == personalizationCategory)
                {
                    if (toRemove == null)
                        toRemove = new List<PersonalizationItemInfo>() { kv.Key };
                    else
                        toRemove.Add(kv.Key);

                    DestroyItem(kv.Key, false);
                }

            if (toRemove != null)
            {
                foreach (PersonalizationItemInfo info in toRemove)
                {
                    dictionary.Remove(info);
                }
            }
        }

        public bool HasSpawnedItem(PersonalizationItemInfo personalizationItemInfo)
        {
            return _spawnedItems.ContainsKey(personalizationItemInfo);
        }

        public PersonalizationItemInfo GetItemInfoOfSameType(PersonalizationItemInfo personalizationItemInfo)
        {
            if (personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins)
            {
                return GetSpawnedWeaponSkinInfo(personalizationItemInfo.Weapon);
            }
            else if (personalizationItemInfo.Category == PersonalizationCategory.Accessories)
            {
                return GetSpawnedAccessoryInfo(personalizationItemInfo.BodyPartName);
            }
            return null;
        }

        public PersonalizationItemInfo GetSpawnedWeaponSkinInfo(WeaponType weaponType)
        {
            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> keyValue in _spawnedItems)
                if (keyValue.Key.Category == PersonalizationCategory.WeaponSkins && keyValue.Key.Weapon == weaponType)
                    return keyValue.Key;

            return null;
        }

        public PersonalizationItemInfo GetSpawnedAccessoryInfo(string bodyPart)
        {
            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> keyValue in _spawnedItems)
                if (keyValue.Key.Category == PersonalizationCategory.Accessories && keyValue.Key.BodyPartName == bodyPart)
                    return keyValue.Key;

            return null;
        }

        public PersonalizationEditorObjectBehaviour GetSpawnedItemOfSameType(PersonalizationItemInfo personalizationItemInfo)
        {
            if (personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins)
            {
                return GetSpawnedWeaponSkin(personalizationItemInfo.Weapon);
            }
            else if (personalizationItemInfo.Category == PersonalizationCategory.Accessories)
            {
                return GetSpawnedAccessory(personalizationItemInfo.BodyPartName);
            }
            return null;
        }

        public PersonalizationEditorObjectBehaviour GetSpawnedWeaponSkin(WeaponType weaponType)
        {
            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> keyValue in _spawnedItems)
                if (keyValue.Key.Category == PersonalizationCategory.WeaponSkins && keyValue.Key.Weapon == weaponType)
                    return keyValue.Value;

            return null;
        }

        public PersonalizationEditorObjectBehaviour GetSpawnedAccessory(string bodyPart)
        {
            foreach (KeyValuePair<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> keyValue in _spawnedItems)
                if (keyValue.Key.Category == PersonalizationCategory.WeaponSkins && keyValue.Key.BodyPartName == bodyPart)
                    return keyValue.Value;

            return null;
        }

        public Transform GetParentForItem(PersonalizationItemInfo personalizationItemInfo)
        {
            if (personalizationItemInfo.Category == PersonalizationCategory.Pets)
            {
                return base.transform;
            }
            else if (personalizationItemInfo.Category == PersonalizationCategory.Accessories)
            {
                Transform bodyPart = TransformUtils.FindChildRecursive(base.transform, personalizationItemInfo.BodyPartName);
                if (!bodyPart)
                {
                    if (owner.HasCharacterModel())
                    {
                        bodyPart = ownerModel.transform;
                    }
                    else
                    {
                        bodyPart = owner.transform;
                    }
                }
                return bodyPart;
            }
            else if (personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins)
            {
                Transform weaponModelTransform = ownerModel.GetWeaponModel(personalizationItemInfo.Weapon)?.transform;
                if (!personalizationItemInfo.OverrideParent.IsNullOrEmpty())
                {
                    Transform overridenParent = TransformUtils.FindChildRecursive(owner.transform, personalizationItemInfo.OverrideParent);
                    if (overridenParent)
                        weaponModelTransform = overridenParent;
                }
                return weaponModelTransform;
            }
            return null;
        }

        public string GetWeaponSkinDependingOnOwner(WeaponType weaponType)
        {
            if (!_hasInitialized || PersonalizationEditorManager.IsInEditor()) return string.Empty;

            if (_isEnemy) return PersonalizationUserInfo.AllowEnemiesUseSkins ? PersonalizationUserInfo.GetWeaponSkin(weaponType) : string.Empty;

            if (_isMainPlayer && UIPersonalizationItemBrowser.IsPreviewing) return PersonalizationUserInfo.GetWeaponSkin(weaponType);

            if (!_isPlayer) return string.Empty;

            if (_isMultiplayer)
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
                    case ModWeaponsManager.SCYTHE_TYPE:
                        return multiplayerPlayerInfo.ScytheSkin;
                }
            }
            return PersonalizationUserInfo.GetWeaponSkin(weaponType);
        }

        private void onPlayerInfoUpdated(string playFabId)
        {
            if (playFabId == owner.GetPlayFabID()) RefreshWeaponSkinsNextFrame();
        }

        private void onItemEquippedOrUnequipped()
        {
            SpawnEquippedAccessories();
        }
    }
}