using OverhaulMod.Combat;
using OverhaulMod.Combat.Weapons;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class RobotWeaponBag : MonoBehaviour
    {
        [ModSetting(ModSettingsConstants.ENABLE_WEAPON_BAG, true)]
        public static bool EnableWeaponBag;

        public static readonly Dictionary<WeaponType, TransformInfo> WeaponPositions = new Dictionary<WeaponType, TransformInfo>()
        {
            { WeaponType.Sword, new TransformInfo(new Vector3(1.15f, 1.75f, -0.85f), new Vector3(60f, 260f, 260f))},
            { WeaponType.Bow, new TransformInfo(new Vector3(-0.15f, 0.35f, -0.6f), new Vector3(0f, 0f, 35f))},
            { WeaponType.Hammer, new TransformInfo(new Vector3(-0.85f, 2f, -0.95f), new Vector3(4f, 0f, 193f), Vector3.one * 0.75f)},
            { WeaponType.Spear, new TransformInfo(new Vector3(0f, 0.6f, -0.6f), new Vector3(283f, 46f, 120f), Vector3.one * 0.85f)},
            { ModWeaponsManager.SCYTHE_TYPE, new TransformInfo(new Vector3(0.1f, 0.5f, -0.6f), new Vector3(290f, 280f, 70f), Vector3.one)},
        };

        private FirstPersonMover _firstPersonMover;

        private WeaponType _lastEquippedWeapon;

        private Dictionary<WeaponType, GameObject> _weaponToRenderer;

        public Transform Bag;

        public bool IsSupported;

        private void Start()
        {
            _firstPersonMover = base.GetComponent<FirstPersonMover>();

            _weaponToRenderer = new Dictionary<WeaponType, GameObject>();
            CreateContainers();
            RefreshRenderers();

            GlobalEventManager.Instance.AddEventListener(ModSettingsManager.SETTING_CHANGED_EVENT, RefreshRenderers);
            GlobalEventManager.Instance.AddEventListener<FirstPersonMover>(GlobalEvents.UpgradesRefreshed, onFirstPersonMoverUpgraded);
        }

        private void Update()
        {
            if (!IsSupported)
                return;

            WeaponType currentWeapon = _firstPersonMover._currentWeapon;
            if (currentWeapon != _lastEquippedWeapon)
            {
                RefreshRenderers();
                _lastEquippedWeapon = currentWeapon;
            }
        }

        private void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener(ModSettingsManager.SETTING_CHANGED_EVENT, RefreshRenderers);
            GlobalEventManager.Instance.RemoveEventListener<FirstPersonMover>(GlobalEvents.UpgradesRefreshed, onFirstPersonMoverUpgraded);
        }

        private void onFirstPersonMoverUpgraded(FirstPersonMover firstPersonMover)
        {
            ModActionUtils.DoInFrames(delegate
            {
                if (firstPersonMover && firstPersonMover == _firstPersonMover)
                    RespawnRenderers();
            }, 10);
        }

        public void DestroySelf()
        {
            if (Bag)
            {
                Destroy(Bag.gameObject);
            }
            Destroy(this);
        }

        public void CreateContainers()
        {
            if (!Bag)
            {
                Transform torso = TransformUtils.FindChildRecursive(base.transform, "Torso");
                if (!torso)
                {
                    IsSupported = false;
                    return;
                }

                GameObject bagObject = new GameObject("WeaponBag");
                bagObject.transform.SetParent(torso, false);
                bagObject.transform.SetLocalTransform(Vector3.zero, Vector3.zero, Vector3.one * 0.75f);
                Bag = bagObject.transform;
            }
            IsSupported = true;
        }

        public void RespawnRenderers()
        {
            Dictionary<WeaponType, GameObject> keyValues = _weaponToRenderer;
            if (keyValues == null)
                return;

            foreach (GameObject obj in keyValues.Values)
                if (obj)
                    Destroy(obj);

            keyValues.Clear();
            RefreshRenderers();
        }

        public void RefreshRenderers()
        {
            FirstPersonMover firstPersonMover = _firstPersonMover;
            if (!firstPersonMover)
            {
                DestroySelf();
                return;
            }

            if (PersonalizationEditorManager.IsInEditor() || firstPersonMover.IsMindSpaceCharacter)
                return;

            List<WeaponType> equippedWeapons = firstPersonMover._equippedWeapons;
            if (equippedWeapons == null)
                return;

            List<WeaponType> droppedWeapons = firstPersonMover._droppedWeapons;
            if (droppedWeapons == null)
                return;

            WeaponModel[] equippedWeaponModels = firstPersonMover.GetCharacterModel()?.WeaponModels;
            if (equippedWeaponModels == null)
                return;

            foreach (WeaponType weaponType in equippedWeapons)
                AddRenderer(weaponType, equippedWeapons, equippedWeaponModels);

            foreach (KeyValuePair<WeaponType, GameObject> keyValue in _weaponToRenderer)
            {
                if (!keyValue.Value || keyValue.Key == WeaponType.None)
                    continue;

                bool hasConstructionFinished = (!GameModeManager.IsBattleRoyale() && !GameModeManager.IsMultiplayerDuel()) || firstPersonMover.HasConstructionFinished();
                bool isEquipped = firstPersonMover.GetEquippedWeaponType() == keyValue.Key;
                bool shouldDisplay = EnableWeaponBag && hasConstructionFinished && !isEquipped && !droppedWeapons.Contains(keyValue.Key);
                _weaponToRenderer[keyValue.Key].SetActive(shouldDisplay);
            }
        }

        public void AddRenderer(WeaponType weaponType, List<WeaponType> equippedWeapons, WeaponModel[] equippedWeaponModels)
        {
            if (!WeaponPositions.ContainsKey(weaponType) || equippedWeapons == null || equippedWeaponModels == null)
                return;

            WeaponModel weaponModel = null;
            bool hasModel = false;
            foreach (WeaponModel model in equippedWeaponModels)
                if (model && model.WeaponType == weaponType)
                {
                    weaponModel = model;
                    hasModel = true;
                    break;
                }

            bool hasWeapon = equippedWeapons.Contains(weaponType);

            if (hasModel && hasWeapon)
            {
                if (!_weaponToRenderer.ContainsKey(weaponType))
                {
                    Transform modelTransform;
                    if (weaponModel is ModWeaponModel modWeaponModel)
                    {
                        modelTransform = modWeaponModel.GetModel()?.transform;
                        if (modelTransform)
                        {
                            Renderer renderer = modelTransform.GetComponent<Renderer>();
                            if (renderer)
                                renderer.enabled = true;
                        }
                    }
                    else if (weaponType == WeaponType.Bow)
                    {
                        modelTransform = weaponModel.getExistingWeaponModel()?.parent;
                    }
                    else
                    {
                        modelTransform = weaponType == WeaponType.Spear ? (weaponModel.PartsToDrop.IsNullOrEmpty() ? null : weaponModel.PartsToDrop[0]) : weaponModel.getExistingWeaponModel();
                    }

                    if (!modelTransform)
                        return;

                    GameObject newRenderer = InstantiateNewRenderer(modelTransform, weaponType);
                    if (newRenderer)
                    {
                        _weaponToRenderer.Add(weaponType, newRenderer);
                    }
                }
            }
        }

        public GameObject InstantiateNewRenderer(Transform transform, WeaponType weaponType)
        {
            if (!WeaponPositions.TryGetValue(weaponType, out TransformInfo transformInfo))
                return null;

            Transform renderer = Instantiate(transform, Bag, false);
            renderer.SetLocalTransform(transformInfo);
            renderer.RandomizeLocalTransform(0.950f, 1.050f, false, true, false);
            return renderer.gameObject;
        }
    }
}
