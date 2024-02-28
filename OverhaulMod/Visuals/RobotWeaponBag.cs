using OverhaulMod.Engine;
using OverhaulMod.Patches.Addons;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class RobotWeaponBag : RobotAddon
    {
        private WeaponType m_lastEquippedWeapon;

        public static readonly Dictionary<WeaponType, TransformInfo> WeaponToPosition = new Dictionary<WeaponType, TransformInfo>()
        {
            { WeaponType.Sword, new TransformInfo(new Vector3(1.15f, 1.75f, -0.8f), new Vector3(60f, 260f, 260f))},
            { WeaponType.Bow, new TransformInfo(new Vector3(-0.15f, 0.35f, -0.6f), new Vector3(0f, 0f, 35f))},
            { WeaponType.Hammer, new TransformInfo(new Vector3(-0.85f, 2f, -0.95f), new Vector3(4f, 0f, 193f), Vector3.one * 0.75f)},
            { WeaponType.Spear, new TransformInfo(new Vector3(0f, 0.6f, -0.6f), new Vector3(283f, 46f, 120f), Vector3.one * 0.85f)},
        };

        public Dictionary<WeaponType, GameObject> WeaponToRenderer;

        public Transform bag { get; private set; }

        public bool IsSupported;

        public override void Start()
        {
            WeaponToRenderer = new Dictionary<WeaponType, GameObject>();
            CreateContainers();
            RefreshRenderers();

            //firstPersonMoverReference.AddDeathListener(DestroySelf);
            GlobalEventManager.Instance.AddEventListener<FirstPersonMover>(GlobalEvents.UpgradesRefreshed, onFirstPersonMoverUpgraded);
        }

        public override void Update()
        {
            if (!IsSupported)
                return;

            WeaponType currentWeapon = firstPersonMoverReference._currentWeapon;
            if (currentWeapon != m_lastEquippedWeapon)
            {
                RefreshRenderers();
                m_lastEquippedWeapon = currentWeapon;
            }
        }

        public override void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener<FirstPersonMover>(GlobalEvents.UpgradesRefreshed, onFirstPersonMoverUpgraded);
        }

        private void onFirstPersonMoverUpgraded(FirstPersonMover firstPersonMover)
        {
            ModActionUtils.DoInFrame(delegate
            {
                if (firstPersonMover && firstPersonMover == firstPersonMoverReference)
                    RespawnRenderers();
            });
        }

        public void DestroySelf()
        {
            if (bag)
            {
                Destroy(bag.gameObject);
            }
            Destroy(this);
        }

        public void CreateContainers()
        {
            if (!bag)
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
                bag = bagObject.transform;
            }
            IsSupported = true;
        }

        public void RespawnRenderers()
        {
            Dictionary<WeaponType, GameObject> keyValues = WeaponToRenderer;
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
            FirstPersonMover firstPersonMover = firstPersonMoverReference;
            if (!firstPersonMover)
            {
                DestroySelf();
                return;
            }

            if (firstPersonMover.IsMindSpaceCharacter)
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

            foreach (KeyValuePair<WeaponType, GameObject> keyValue in WeaponToRenderer)
            {
                if (!keyValue.Value || keyValue.Key == WeaponType.None)
                    continue;

                bool isEquipped = firstPersonMover.GetEquippedWeaponType() == keyValue.Key;
                bool shouldDisplay = !isEquipped && !droppedWeapons.Contains(keyValue.Key);
                WeaponToRenderer[keyValue.Key].SetActive(shouldDisplay);
            }
        }

        public void AddRenderer(WeaponType weaponType, List<WeaponType> equippedWeapons, WeaponModel[] equippedWeaponModels)
        {
            if (!WeaponToPosition.ContainsKey(weaponType) || equippedWeapons == null || equippedWeaponModels == null)
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
                if (!WeaponToRenderer.ContainsKey(weaponType))
                {
                    Transform renderer = weaponType == WeaponType.Spear ? weaponModel.PartsToDrop[0] : weaponModel.getExistingWeaponModel();
                    if (renderer)
                    {
                        if (weaponType == WeaponType.Bow)
                        {
                            renderer = renderer.parent;
                        }

                        GameObject newRenderer = InstantiateNewRenderer(renderer, weaponType);
                        if (newRenderer)
                        {
                            WeaponToRenderer.Add(weaponType, newRenderer);
                        }
                    }
                }
            }
        }

        public GameObject InstantiateNewRenderer(Transform transform, WeaponType weaponType)
        {
            if (!WeaponToPosition.TryGetValue(weaponType, out TransformInfo transformInfo))
                return null;

            Transform renderer = Instantiate(transform, bag, false);
            renderer.SetLocalTransform(transformInfo);
            renderer.RandomizeLocalTransform(0.950f, 1.050f, false, true, false);
            return renderer.gameObject;
        }
    }
}
