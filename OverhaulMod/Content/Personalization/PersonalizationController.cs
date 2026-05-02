using OverhaulMod.Engine;
using OverhaulMod.Gameplay;
using OverhaulMod.Gameplay.Weapons;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationController : MonoBehaviour
    {
        public const bool REFRESH_ONE_SKIN_AT_TIME = true;

        public const bool REFRESH_SKINS_ONLY_OF_AVAILABLE_WEAPONS = true;

        public const float SKINS_REFRESH_INTERVAL_FOR_PLAYER = 0.5f;

        public const float SKINS_REFRESH_INTERVAL_FOR_ENEMIES = 1f;

        private FirstPersonMover _owner;

        private CharacterModel _ownerModel;

        private CharacterModel.PatternColorSet _ownerModelColors;

        private Color _ownerModelPrimaryColor;

        private PersonalizationMultiplayerPlayerInfo _playerInfo;
        public PersonalizationMultiplayerPlayerInfo PlayerInfo
        {
            get
            {
                if (!_isMultiplayer || !_isPlayer) return null;

                FirstPersonMover firstPersonMover = _owner;
                if (!firstPersonMover || !firstPersonMover.IsAlive())
                    return null;

                if (_playerInfo == null) _playerInfo = PersonalizationMultiplayerManager.Instance.GetPlayInfo(_owner.GetPlayFabID());
                return _playerInfo;
            }
        }

        private Dictionary<WeaponType, Transform[]> _weaponTypeToParts;

        private Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour> _spawnedItems;

        private Dictionary<WeaponType, WeaponVariant2> _weaponTypeToVariant;

        private Transform _defaultArrowSpawnPoint, _arrowSpawnPoint;

        private float _timeLeftToRefreshSkins;

        private bool _isEnemy;

        private bool _isPlayer, _isMainPlayer, _isMindSpace;

        private bool _isMultiplayer;

        private bool _hasAddedMultiplayerEventListeners;

        private bool _hasOwnerDied;

        private bool _hasRobotStateChanged;

        private void Start()
        {
            FirstPersonMover firstPersonMover = _owner;
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

        private void OnDestroy()
        {
            if (_hasAddedMultiplayerEventListeners)
            {
                GlobalEventManager.Instance.RemoveEventListener<string>(PersonalizationMultiplayerManager.PLAYER_INFO_UPDATED_EVENT, onPlayerInfoUpdate);
                _hasAddedMultiplayerEventListeners = false;
            }
        }

        private void Update()
        {
            if (_hasOwnerDied || _isMindSpace) return;

            _timeLeftToRefreshSkins = Mathf.Max(0f, _timeLeftToRefreshSkins - Time.deltaTime);
            if (_timeLeftToRefreshSkins == 0f && _hasRobotStateChanged)
            {
                _hasRobotStateChanged = false;
                _timeLeftToRefreshSkins = _isPlayer ? SKINS_REFRESH_INTERVAL_FOR_PLAYER : SKINS_REFRESH_INTERVAL_FOR_ENEMIES;
                RefreshWeaponSkins();
                RefreshBowSkinVisibility();
            }
        }

        public void Initialize(FirstPersonMover firstPersonMover)
        {
            if (PersonalizationEditorManager.IsInEditorMode()) PersonalizationEditorManager.Instance.PreviewingPersonalizationController = this;

            _weaponTypeToParts = new Dictionary<WeaponType, Transform[]>();
            _weaponTypeToVariant = new Dictionary<WeaponType, WeaponVariant2>();
            _spawnedItems = new Dictionary<PersonalizationItemInfo, PersonalizationEditorObjectBehaviour>();

            _owner = firstPersonMover;
            _ownerModel = firstPersonMover.GetCharacterModel();
            _ownerModelColors = _ownerModel.GetFavouriteColors();
            _ownerModelPrimaryColor = _ownerModel.GetFavouriteColor();
            RefreshOwnerInfo();

            _defaultArrowSpawnPoint = firstPersonMover.GetCharacterModel().ArrowHolder;
            _arrowSpawnPoint = null;
            RefreshWeaponModelReferences();

            if (_isMultiplayer && _isPlayer)
            {
                GlobalEventManager.Instance.AddEventListener<string>(PersonalizationMultiplayerManager.PLAYER_INFO_UPDATED_EVENT, onPlayerInfoUpdate);
                _hasAddedMultiplayerEventListeners = true;
            }

            CharacterUpdateScheduler.Instance.UpdateCharacter(firstPersonMover, new CharacterUpdateRequest()
            {
                UpdateWeaponSkins = true,
                UpdateAccessories = true
            });
        }

        public FirstPersonMover GetOwner() => _owner;

        public CharacterModel GetOwnerModel() => _ownerModel;

        public CharacterModel.PatternColorSet GetOwnerModelColors() => _ownerModelColors;

        public Color GetOwnerModelPrimaryColor() => _ownerModelPrimaryColor;

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
            _hasRobotStateChanged = true;
            _timeLeftToRefreshSkins = 0f;
        }

        public void RefreshWeaponSkins()
        {
            CharacterModel characterModel = _ownerModel;
            if (!characterModel || characterModel.WeaponModels == null) return;

            foreach (WeaponModel weaponModel in characterModel.WeaponModels)
            {
                if (!PersonalizationManager.IsWeaponCustomizationSupported(weaponModel.WeaponType)) continue;

                if (REFRESH_SKINS_ONLY_OF_AVAILABLE_WEAPONS && !_owner._equippedWeapons.Contains(weaponModel.WeaponType)) continue;

                if (RefreshSkinOfWeapon(weaponModel.WeaponType) && REFRESH_ONE_SKIN_AT_TIME)
                {
                    _hasRobotStateChanged = true;
                    return;
                }
            }

            ModDebug.Log("Refreshed weapon skins");
        }

        public bool RefreshSkinOfWeapon(WeaponType weaponType)
        {
            bool hasRefreshed = false;
            if (!PersonalizationEditorManager.IsInEditorMode())
            {
                // handle skin changing
                PersonalizationItemInfo spawnedSkinInfo = GetSpawnedWeaponSkinInfo(weaponType);
                string currentSkinId = spawnedSkinInfo?.ItemID;
                bool noCurrentSkin = currentSkinId.IsNullOrEmpty();
                string targetSkinId = GetWeaponSkinDependingOnOwner(weaponType);
                bool noTargetSkin = targetSkinId.IsNullOrEmpty() || targetSkinId == "_";
                bool skinChanged = targetSkinId != currentSkinId;

                // handle weapon upgrades
                bool skinMatchesUpgrade = IsSkinMatchingWeaponUpgrade(weaponType);

                bool shouldDestroySkin = (skinChanged || !skinMatchesUpgrade || noTargetSkin) && spawnedSkinInfo != null;
                bool shouldSpawnSkin = (skinChanged || !skinMatchesUpgrade || noCurrentSkin) && !noTargetSkin;

                if (shouldDestroySkin)
                {
                    DestroyItem(spawnedSkinInfo);
                }
                if (shouldSpawnSkin)
                {
                    if (!skinMatchesUpgrade)
                    {
                        RefreshWeaponModelReferences(_ownerModel.GetWeaponModel(weaponType));
                    }

                    SpawnItem(targetSkinId);

                    if (shouldDestroySkin && _isMainPlayer && PersonalizationManager.Instance.IsSelectingItems()) // play vfx only if the player has switched the skin
                    {
                        WeaponModel weaponModel = _ownerModel.GetWeaponModel(weaponType);
                        if (weaponModel && weaponModel.isActiveAndEnabled) AttackManager.Instance.CreateBattleCruiserGatlingImpactVFX(weaponModel.transform.position);
                    }
                }

                hasRefreshed = shouldDestroySkin || shouldSpawnSkin;
            }

            RefreshVanillaWeaponModelVisibility(weaponType);
            return hasRefreshed;
        }

        public void RefreshVanillaWeaponModelVisibility(WeaponType weaponType)
        {
            bool inEditor = PersonalizationEditorManager.IsInEditorMode();
            bool forceShowOriginalModel = inEditor && PersonalizationEditorManager.Instance.ViewingOriginalModel;

            PersonalizationItemInfo personalizationItemInfo = GetSpawnedWeaponSkinInfo(weaponType);
            bool hasSpawnedSkinForWeapon = personalizationItemInfo != null;

            if (inEditor && weaponType == WeaponType.Sword)
            {
                WeaponVariant2 wv = PersonalizationEditorManager.Instance.PreviewPresetKey;
                SetWeaponPartsVisible(WeaponType.Sword, forceShowOriginalModel || (!hasSpawnedSkinForWeapon && !(wv == WeaponVariant2.NormalMultiplayer || wv == WeaponVariant2.OnFireMultiplayer)), false);
            }
            else
            {
                SetWeaponPartsVisible(weaponType, forceShowOriginalModel || !hasSpawnedSkinForWeapon, (personalizationItemInfo != null && personalizationItemInfo.HideBowStrings));
            }
        }

        public void RefreshBowSkinVisibility()
        {
            FirstPersonMover firstPersonMover = _owner;
            if (!firstPersonMover) return;

            PersonalizationEditorObjectBehaviour weaponSkinObject = GetSpawnedWeaponSkin(WeaponType.Bow);
            if (weaponSkinObject && weaponSkinObject.ControllerInfo.ItemInfo != null && !weaponSkinObject.ControllerInfo.ItemInfo.OverrideParent.IsNullOrEmpty())
            {
                weaponSkinObject.gameObject.SetActive(firstPersonMover.GetEquippedWeaponType() == WeaponType.Bow);
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
            CharacterModel characterModel = _ownerModel;
            if (characterModel) characterModel.ArrowHolder = _arrowSpawnPoint ?? _defaultArrowSpawnPoint;
        }

        public void RefreshWeaponModelReferences()
        {
            _weaponTypeToParts.Clear();
            CharacterModel characterModel = _ownerModel;
            if (!characterModel || characterModel.WeaponModels.IsNullOrEmpty()) return;

            foreach (WeaponModel weaponModel in characterModel.WeaponModels)
            {
                if (weaponModel) RefreshWeaponModelReferences(weaponModel);
            }
        }

        public void RefreshWeaponModelReferences(WeaponModel weaponModel)
        {
            WeaponType weaponType = weaponModel.WeaponType;
            if (PersonalizationManager.IsWeaponCustomizationSupported(weaponType))
                if (_weaponTypeToParts.ContainsKey(weaponType))
                    _weaponTypeToParts[weaponType] = weaponModel.PartsToDrop;
                else
                    _weaponTypeToParts.Add(weaponModel.WeaponType, weaponModel.PartsToDrop);
        }

        public bool IsSkinMatchingWeaponUpgrade(WeaponType weaponType)
        {
            WeaponVariantManager.GetWeaponVariant(_owner, weaponType, out WeaponVariant2 actualVariant);
            return GetWeaponVariantOfSpawnedSkin(weaponType) == actualVariant;
        }

        public void RefreshVariantOfWeapon(WeaponType weaponType)
        {
            Dictionary<WeaponType, WeaponVariant2> d = _weaponTypeToVariant;
            WeaponVariantManager.GetWeaponVariant(_owner, weaponType, out WeaponVariant2 weaponVariant);

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

        public void SetWeaponPartsVisible(WeaponType weaponType, bool value, bool hideBowStrings)
        {
            if (weaponType == ModWeaponsManager.SCYTHE_TYPE)
            {
                WeaponModel weaponModel = _ownerModel.GetWeaponModel(ModWeaponsManager.SCYTHE_TYPE);
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

        public void RefreshAccessories()
        {
            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.Accessories) || PersonalizationEditorManager.IsInEditorMode()) return;

            DestroyItemsOfCategory(PersonalizationCategory.Accessories);

            List<string> accessories = GetAccessoriesDependingOnOwner();
            if (accessories != null)
            {
                foreach (string item in accessories)
                {
                    _ = SpawnItem(item);
                }
            }

            ModDebug.Log("Refreshed accessories");
        }

        public PersonalizationEditorObjectBehaviour SpawnItem(string itemId)
        {
            if (itemId.IsNullOrEmpty()) return null;

            return SpawnItem(PersonalizationManager.Instance.ItemList.GetItem(itemId));
        }

        public PersonalizationEditorObjectBehaviour SpawnItem(PersonalizationItemInfo itemInfo)
        {
            bool inEditor = PersonalizationEditorManager.IsInEditorMode();
            if (itemInfo == null || HasSpawnedItem(itemInfo) || !_owner)
                return null;

            if (!inEditor && !itemInfo.IsUnlocked(_owner))
                return null;

            EnemyType enemyType = _owner.CharacterType;
            if (_owner.IsMindSpaceCharacter || enemyType == EnemyType.ZombieArcher1 || enemyType == EnemyType.FleetAnalysisBot1 || enemyType == EnemyType.FleetAnalysisBot2 || enemyType == EnemyType.FleetAnalysisBot3 || enemyType == EnemyType.FleetAnalysisBot4 || (itemInfo.Category == PersonalizationCategory.WeaponSkins && itemInfo.Weapon == WeaponType.Bow && ModSpecialUtils.IsModEnabled("ee32ba1b-8c92-4f50-bdf4-400a14da829e")))
                return null;

            Transform transform = GetParentForItem(itemInfo);
            if (!transform) return null;

            MechBodyPart bodyPartForAccessory = null;
            if (itemInfo.Category == PersonalizationCategory.Accessories)
            {
                List<MechBodyPart> bodyParts = _owner.GetAllBodyParts();
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

            if (itemInfo.Category == PersonalizationCategory.WeaponSkins) RefreshVariantOfWeapon(itemInfo.Weapon);

            itemInfo.LoadRootObjectIfRequired();
            PersonalizationEditorObjectBehaviour behaviour = itemInfo.RootObject.Deserialize(transform, new ItemSpawnInfo(this, itemInfo));
            if (!behaviour)
            {
                _spawnedItems.Add(itemInfo, null);
                return null;
            }
            _spawnedItems.Add(itemInfo, behaviour);

            if (itemInfo.Category == PersonalizationCategory.WeaponSkins)
            {
                WeaponModel weaponModel = _ownerModel.GetWeaponModel(itemInfo.Weapon);
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
                if (GetCharacterModelPartIndices(out int headModel, out int torsoModel, out int legsModel))
                {
                    AccessoryOffset offset = null;
                    string bodyPart = itemInfo.BodyPartName;
                    if (!bodyPart.IsNullOrEmpty())
                    {
                        if (PersonalizationManager.HeadBodyParts.Contains(bodyPart))
                        {
                            offset = itemInfo.AccessoryOffsets.GetOffsetForModel(headModel);
                        }
                        else if (PersonalizationManager.TorsoBodyParts.Contains(bodyPart))
                        {
                            offset = itemInfo.AccessoryOffsets.GetOffsetForModel(torsoModel);
                        }
                        else if (PersonalizationManager.LegsBodyParts.Contains(bodyPart))
                        {
                            offset = itemInfo.AccessoryOffsets.GetOffsetForModel(legsModel);
                        }
                    }

                    if (offset != null)
                    {
                        Transform itemTransform = behaviour.transform;
                        itemTransform.localPosition = offset.GetPosition();
                        itemTransform.localEulerAngles = offset.GetEulerAngles();
                        itemTransform.localScale = offset.GetScale();
                    }
                }

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
                WeaponModel weaponModel = _ownerModel.GetWeaponModel(behaviour.ControllerInfo.ItemInfo.Weapon);
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
                return TransformUtils.FindChildRecursive(_ownerModel.transform, personalizationItemInfo.BodyPartName);
            }
            else if (personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins)
            {
                Transform weaponModelTransform = _ownerModel.GetWeaponModel(personalizationItemInfo.Weapon)?.transform;
                if (personalizationItemInfo.OverrideParent.IsNullOrEmpty())
                {
                    Transform overridenParent = TransformUtils.FindChildRecursive(_owner.transform, personalizationItemInfo.OverrideParent);
                    if (overridenParent)
                        weaponModelTransform = overridenParent;
                }
                return weaponModelTransform;
            }
            return null;
        }

        public string GetWeaponSkinDependingOnOwner(WeaponType weaponType)
        {
            if (_isEnemy) return PersonalizationUserInfo.AllowEnemiesUseSkins ? PersonalizationUserInfo.GetWeaponSkin(weaponType) : string.Empty;

            if (_isMainPlayer && UIPersonalizationItemBrowser.IsPreviewing) return PersonalizationUserInfo.GetWeaponSkin(weaponType);

            if (!_isPlayer) return string.Empty;

            if (_isMultiplayer)
            {
                PersonalizationMultiplayerPlayerInfo multiplayerPlayerInfo = PlayerInfo;
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

        public List<string> GetAccessoriesDependingOnOwner()
        {
            if (_isEnemy) return PersonalizationUserInfo.AllowEnemiesUseSkins ? PersonalizationUserInfo.GetEquippedAccessoriesList() : null;

            if (_isMainPlayer && UIPersonalizationItemBrowser.IsPreviewing) return PersonalizationUserInfo.GetEquippedAccessoriesList();

            if (!_isPlayer) return null;

            if (_isMultiplayer)
            {
                PersonalizationMultiplayerPlayerInfo multiplayerPlayerInfo = PlayerInfo;
                if (multiplayerPlayerInfo == null)
                    return null;

                return PersonalizationUserInfo.GetItemList(multiplayerPlayerInfo.Accessories);
            }

            return PersonalizationUserInfo.GetEquippedAccessoriesList();
        }

        public bool GetCharacterModelPartIndices(out int headModelIndex, out int torsoModelIndex, out int legsModelIndex)
        {
            FirstPersonMover robot = _owner;
            if (!robot)
            {
                headModelIndex = -1;
                torsoModelIndex = -1;
                legsModelIndex = -1;
                return false;
            }

            if (_isMultiplayer)
            {
                if (_isPlayer)
                {
                    MultiplayerPlayerInfoState infoState = MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(robot.GetPlayFabID());
                    if (infoState)
                    {
                        headModelIndex = infoState.state.CharacterModelHeadIndex;
                        torsoModelIndex = infoState.state.CharacterModelTorsoIndex;
                        legsModelIndex = infoState.state.CharacterModelIndex;
                        return true;
                    }
                }
            }
            else
            {
                if (_isPlayer)
                {
                    if (PersonalizationEditorManager.IsInEditorMode())
                    {
                        int index = UIPE.Instance.Utilities.GetCharacterModelIndex();
                        headModelIndex = index;
                        torsoModelIndex = index;
                        legsModelIndex = index;
                        return true;
                    }

                    SettingsManager settingsManager = SettingsManager.Instance;
                    string slotId = settingsManager.GetSelectedMultiplayerHumanSlot();
                    MultiplayerHumanSlot? slot = settingsManager.GetMultiplayerHumanSlot(slotId);
                    headModelIndex = slot.Value.HeadModel.GetModelIndexAsInt();
                    torsoModelIndex = slot.Value.TorsoModel.GetModelIndexAsInt();
                    legsModelIndex = slot.Value.RootModel.GetModelIndexAsInt();
                    return true;
                }
            }

            headModelIndex = -1;
            torsoModelIndex = -1;
            legsModelIndex = -1;
            return false;
        }



        public void OnUpgrade()
        {
            _hasRobotStateChanged |= true;
        }

        private void onPlayerInfoUpdate(string playFabId)
        {
            if (playFabId == _owner.GetPlayFabID())
            {
                CharacterUpdateScheduler.Instance.UpdateCharacter(_owner, new CharacterUpdateRequest()
                {
                    UpdateWeaponSkins = true,
                    UpdateAccessories = true,
                    UpdatePets = true,
                });
            }
        }
    }
}