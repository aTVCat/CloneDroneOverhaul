using ModLibrary;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Gameplay;
using OverhaulMod.Gameplay.Weapons;
using OverhaulMod.Utils;
using PicaVoxel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class RobotWeaponBag : MonoBehaviour
    {
        public const bool DEBUG_CENTERS = false;

        public const float BAG_SCALE = 0.7f;

        public const float DEFAULT_OFFSET = -0.375f;

        public const float ADDITIONAL_OFFSET = -0.1f;

        [ModSetting(ModSettingsConstants.ENABLE_WEAPON_BAG, true)]
        public static bool EnableWeaponBag;

        public static readonly WeaponType[] SupportedWeapons = new WeaponType[]
        {
            WeaponType.Sword,
            WeaponType.Bow,
            WeaponType.Hammer,
            WeaponType.Spear,
            ModWeaponsManager.SCYTHE_TYPE
        };

        public static readonly Dictionary<WeaponType, TransformInfo> PositionsWhenLonely = new Dictionary<WeaponType, TransformInfo>()
        {
            { WeaponType.Sword, new TransformInfo(new Vector3(0f, 0.5f, -0.025f), new Vector3(80f, 270f, 90f), Vector3.one)},
            { WeaponType.Bow, new TransformInfo(new Vector3(0f, -0.3f, -0.025f), new Vector3(0f, 0f, 20f), Vector3.one)},
            { WeaponType.Hammer, new TransformInfo(new Vector3(-0.4f, 0.4f, -0.025f), new Vector3(0f, 0f, 310f), Vector3.one)},
            { WeaponType.Spear, new TransformInfo(new Vector3(0.1f, 0.6f, -0.025f), new Vector3(5f, 90f, 270f), Vector3.one)},
            { ModWeaponsManager.SCYTHE_TYPE, new TransformInfo(new Vector3(0f, 0f, -0.025f), new Vector3(290f, 270f, 90f), Vector3.one)}
        };

        public static readonly Dictionary<WeaponType, TransformInfo> WeaponPositionsWhenMultiple = new Dictionary<WeaponType, TransformInfo>()
        {
            { WeaponType.Sword, new TransformInfo(new Vector3(0.5f, 0.5f, -0.125f), new Vector3(50f, 270f, 90f), Vector3.one)},
            { WeaponType.Bow, new TransformInfo(new Vector3(0f, -0.3f, -0.075f), new Vector3(0f, 0f, 50f), Vector3.one)},
            { WeaponType.Hammer, new TransformInfo(new Vector3(-0.4f, 0.4f, -0.2f), new Vector3(0f, 0f, 310f), Vector3.one)},
            { WeaponType.Spear, new TransformInfo(new Vector3(0.2f, 0.6f, 0f), new Vector3(20f, 90f, 270f), Vector3.one)},
            { ModWeaponsManager.SCYTHE_TYPE, new TransformInfo(new Vector3(0f, 0.2f, -0.025f), new Vector3(305f, 270f, 90f), Vector3.one)}
        };

        private FirstPersonMover _firstPersonMover;

        private Dictionary<WeaponType, Transform> _weaponToHolder;

        private Dictionary<WeaponType, GameObject> _weaponToRenderer;

        private PersonalizationController _personalizationController;

        private Transform _bag;

        private float _calculatedBagOffset;

        private bool _isSupportedByRobot;

        private bool _hasAddedEventListeners;

        private bool _hasScheduledRespawningRenderers;

        private WeaponType _lastEquippedWeapon;

        private void Update()
        {
            if (!_isSupportedByRobot) return;

            WeaponType currentWeapon = _firstPersonMover._currentWeapon;
            if (currentWeapon != _lastEquippedWeapon)
            {
                RefreshVisibilityOfRenderers();
                _lastEquippedWeapon = currentWeapon;
            }
        }

        private void OnDestroy()
        {
            DestroyBag();
            if (_hasAddedEventListeners)
            {
                GlobalEventManager.Instance.RemoveEventListener<string>(PersonalizationMultiplayerManager.PLAYER_INFO_UPDATED_EVENT, onPlayedInfoUpdate);
                _hasAddedEventListeners = false;
            }
        }

        public void Initialize(FirstPersonMover firstPersonMover, PersonalizationController personalizationController)
        {
            _firstPersonMover = firstPersonMover;
            _personalizationController = personalizationController;

            _weaponToHolder = new Dictionary<WeaponType, Transform>();
            _weaponToRenderer = new Dictionary<WeaponType, GameObject>();
            InstantiateBag();

            _firstPersonMover.AddDeathListener(DropVisibleWeapons);

            GlobalEventManager.Instance.AddEventListener<string>(PersonalizationMultiplayerManager.PLAYER_INFO_UPDATED_EVENT, onPlayedInfoUpdate);
            _hasAddedEventListeners = true;

            ScheduleRespawningRenderers();
        }

        public void CalculateOffset()
        {
            Bounds bounds = default;
            MechBodyPart torsoBodyPart = _firstPersonMover.GetBodyPart(MechBodyPartType.Torso);
            if (torsoBodyPart)
            {
                MeshFilter[] meshes = torsoBodyPart.GetComponentsInChildren<MeshFilter>(true);
                foreach (MeshFilter mesh in meshes)
                {
                    if (!mesh.mesh) continue;

                    if (bounds == default)
                    {
                        bounds = mesh.mesh.bounds;
                    }
                    else
                    {
                        bounds.Encapsulate(mesh.mesh.bounds);
                    }
                }

                _calculatedBagOffset = -bounds.extents.z + ADDITIONAL_OFFSET;
            }
            else
            {
                _calculatedBagOffset = DEFAULT_OFFSET;
            }
        }

        public void InstantiateBag()
        {
            if (!_bag)
            {
                Transform torso = TransformUtils.FindChildRecursive(base.transform, "Torso");
                if (!torso)
                {
                    _isSupportedByRobot = false;
                    return;
                }

                CalculateOffset();

                GameObject bagObject = new GameObject("WeaponBag");
                bagObject.transform.SetParent(torso, false);
                bagObject.transform.SetLocalTransform(new Vector3(0f, 0.4f, _calculatedBagOffset), Vector3.zero, Vector3.one * BAG_SCALE);
                if (DEBUG_CENTERS)
                {
                    Transform debugCubeTransform = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                    debugCubeTransform.SetParent(bagObject.transform, false);
                    debugCubeTransform.localPosition = Vector3.zero;
                    debugCubeTransform.localEulerAngles = Vector3.zero;
                    debugCubeTransform.localScale = Vector3.one * 0.1f;
                }

                for (int i = 0; i < SupportedWeapons.Length; i++)
                {
                    GameObject holder = new GameObject($"{SupportedWeapons[i]} Holder");
                    holder.transform.SetParent(bagObject.transform, false);
                    _weaponToHolder.Add(SupportedWeapons[i], holder.transform);

                    if (DEBUG_CENTERS)
                    {
                        Transform debugCubeTransform = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
                        debugCubeTransform.SetParent(holder.transform, false);
                        debugCubeTransform.localPosition = Vector3.zero;
                        debugCubeTransform.localEulerAngles = Vector3.zero;
                        debugCubeTransform.localScale = Vector3.one * 0.1f;
                    }
                }

                _bag = bagObject.transform;
            }
            _isSupportedByRobot = true;
        }

        public void DestroyBag()
        {
            if (_bag && _bag.gameObject) Destroy(_bag.gameObject);
        }

        public void ScheduleRespawningRenderers()
        {
            if (_hasScheduledRespawningRenderers) return;
            _hasScheduledRespawningRenderers = true;

            CharacterUpdateScheduler.Instance.UpdateCharacter(_firstPersonMover, new CharacterUpdateRequest()
            {
                UpdateWeaponBag = true
            });
        }

        public void RespawnRenderers()
        {
            _hasScheduledRespawningRenderers = false;

            DestroyRenderers();
            InstantiateRenderers();
        }

        public void InstantiateRenderers()
        {
            if (PersonalizationEditorManager.IsInEditorMode()) return;

            FirstPersonMover firstPersonMover = _firstPersonMover;
            if (!firstPersonMover) return;

            List<WeaponType> equippedWeapons = firstPersonMover._equippedWeapons;
            if (equippedWeapons == null) return;

            List<WeaponType> droppedWeapons = firstPersonMover._droppedWeapons;
            if (droppedWeapons == null) return;

            WeaponModel[] availableWeaponModels = firstPersonMover.GetCharacterModel()?.WeaponModels;
            if (availableWeaponModels == null) return;

            List<WeaponModel> filteredWeapons = new List<WeaponModel>();
            for (int i = 0; i < availableWeaponModels.Length; i++)
            {
                WeaponModel weaponModel = availableWeaponModels[i];
                WeaponType weaponType = weaponModel.WeaponType;
                if (SupportedWeapons.Contains(weaponType) && equippedWeapons.Contains(weaponType) && !droppedWeapons.Contains(weaponType))
                    filteredWeapons.Add(weaponModel);
            }

            if (filteredWeapons.Count <= 1) return; // if we have only one weapon available, it'll be never shown

            foreach (WeaponModel weapon in filteredWeapons)
            {
                InstantiateRendererOfWeapon(weapon, filteredWeapons.Count < 3);
            }
        }

        public void InstantiateRendererOfWeapon(WeaponModel weapon, bool willBeLonely)
        {
            TransformInfo transformInfo = null;
            if (willBeLonely)
            {
                if (PositionsWhenLonely.ContainsKey(weapon.WeaponType))
                    transformInfo = PositionsWhenLonely[weapon.WeaponType];
                else if (WeaponPositionsWhenMultiple.ContainsKey(weapon.WeaponType))
                    transformInfo = WeaponPositionsWhenMultiple[weapon.WeaponType];
            }
            else
            {
                if (WeaponPositionsWhenMultiple.ContainsKey(weapon.WeaponType))
                    transformInfo = WeaponPositionsWhenMultiple[weapon.WeaponType];
                else if (PositionsWhenLonely.ContainsKey(weapon.WeaponType))
                    transformInfo = PositionsWhenLonely[weapon.WeaponType];
            }

            if (transformInfo == null) transformInfo = new TransformInfo(Vector3.zero, Vector3.zero, Vector3.one);

            string overhaulSkinId = _personalizationController.GetWeaponSkinDependingOnOwner(weapon.WeaponType);

            Transform parent = _weaponToHolder[weapon.WeaponType];
            parent.SetLocalTransform(transformInfo);

            Transform renderer = null;
            if (overhaulSkinId.IsNullOrEmpty())
            {
                if (weapon is ModWeaponModel modWeapon)
                {
                    GameObject model = modWeapon.GetModel();
                    if (!model) return;

                    renderer = Instantiate(model.transform, parent, false);
                    if (renderer)
                    {
                        Renderer rendererComponent = renderer.GetComponent<Renderer>();
                        if (rendererComponent)
                            rendererComponent.enabled = true;
                    }
                }
                else
                {
                    PhysicalWeaponModelType weaponModelReplacementPrefab = WeaponManager.Instance.GetWeaponModelReplacementPrefab(weapon.WeaponType, weapon._hasReplacedWithFireVariant, weapon._hasReplacedWithMultiplayerVariant, weapon._hasReplacedWithEMPVariant);
                    Transform prefab = WeaponManager.Instance.GetDefaultWeaponModel(weaponModelReplacementPrefab);
                    OverrideWeaponModel overrideWeaponModel = weapon.GetComponent<OverrideWeaponModel>();
                    if (overrideWeaponModel) prefab = overrideWeaponModel.GetModelForVariant(weaponModelReplacementPrefab.WeaponVariant);
                    if (!prefab) return;

                    renderer = Instantiate(prefab, parent, false);
                    weapon.replaceWeaponGlowColor(renderer.gameObject, _firstPersonMover._characterModel.GetFavouriteColors().GetWeaponColor(weapon.WeaponType));
                }
            }
            else
            {
                PersonalizationItemInfo itemInfo = PersonalizationManager.Instance.ItemList.GetItem(overhaulSkinId);
                if (itemInfo != null)
                {
                    itemInfo.LoadRootObjectIfRequired();
                    PersonalizationEditorObjectBehaviour rootObject = itemInfo.RootObject.Deserialize(parent, new ItemSpawnInfo(_personalizationController, itemInfo));
                    renderer = rootObject.transform;
                }
            }

            if (renderer)
            {
                if (_weaponToRenderer.ContainsKey(weapon.WeaponType))
                    _weaponToRenderer[weapon.WeaponType] = renderer.gameObject;
                else
                    _weaponToRenderer.Add(weapon.WeaponType, renderer.gameObject);
            }
        }

        public void DestroyRenderers()
        {
            Dictionary<WeaponType, GameObject> keyValues = _weaponToRenderer;
            foreach (GameObject obj in _weaponToRenderer.Values)
                if (obj) Destroy(obj);

            keyValues.Clear();
        }

        public void RefreshVisibilityOfRenderers()
        {
            if (!_firstPersonMover) return;

            List<WeaponType> droppedWeapons = _firstPersonMover._droppedWeapons;

            foreach (KeyValuePair<WeaponType, GameObject> keyValue in _weaponToRenderer)
            {
                if (!keyValue.Value) continue;

                bool hasConstructionFinished = (!GameModeManager.IsBattleRoyale() && !GameModeManager.IsMultiplayerDuel()) || _firstPersonMover.HasConstructionFinished();
                bool isEquipped = _firstPersonMover.GetEquippedWeaponType() == keyValue.Key;
                bool shouldDisplay = EnableWeaponBag && hasConstructionFinished && !isEquipped && !droppedWeapons.Contains(keyValue.Key);
                _weaponToRenderer[keyValue.Key].SetActive(shouldDisplay);
            }
        }

        public void DropVisibleWeapons()
        {
            foreach (KeyValuePair<WeaponType, GameObject> keyValue in _weaponToRenderer)
            {
                if (!keyValue.Value) continue;
                if (keyValue.Value.activeInHierarchy) dropWeapon(keyValue.Value, keyValue.Key);
            }
        }

        private void dropWeapon(GameObject weaponObject, WeaponType weaponType)
        {
            Transform weaponTransform = weaponObject.transform;
            weaponTransform.SetParent(GarbageWorldRoot.Instance.transform, true);

            if (weaponType == ModWeaponsManager.SCYTHE_TYPE)
            {
                weaponObject.AddComponent<MeshCollider>().convex = true;
                weaponObject.layer = Layers.BodyPart;
            }

            Rigidbody rigidbody = weaponObject.AddComponent<Rigidbody>();
            weaponObject.AddComponent<PlaySoundOnHitGround>().SetClip(AudioLibrary.Instance.SwordHitGround);

            GarbageTarget garbageTarget = weaponObject.AddComponent<GarbageTarget>();
            garbageTarget.MarkReadyToCollect();
            garbageTarget.SortingWeaponType = weaponType;
            garbageTarget.IsInterestingGarbage = true;

            Volume[] volumes = weaponObject.GetComponentsInChildren<Volume>();
            for (int i = 0; i < volumes.Length; i++)
            {
                volumes[i].CollisionTrigger = false;
                volumes[i].ChangeCollisionMode(CollisionMode.MeshColliderConvex);
            }

            Collider[] colliders = weaponObject.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].isTrigger = false;
            }
        }

        public void OnUpgrade()
        {
            if (!_firstPersonMover) return;

            ScheduleRespawningRenderers();
        }

        private void onPlayedInfoUpdate(string playfabId)
        {
            if (!_firstPersonMover || playfabId != _firstPersonMover.GetPlayFabID()) return;

            ScheduleRespawningRenderers();
        }
    }
}