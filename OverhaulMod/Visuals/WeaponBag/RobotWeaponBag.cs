using OverhaulMod.Patches;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class RobotWeaponBag : RobotAddon
    {
        private WeaponType m_lastEquippedWeapon;

        public static readonly Dictionary<WeaponType, TransformInfo> WeaponToPosition = new Dictionary<WeaponType, TransformInfo>()
        {
            { WeaponType.Sword, new TransformInfo(new Vector3(1.15f, 1.75f, -0.75f), new Vector3(60f, 260f, 260f))},
            { WeaponType.Bow, new TransformInfo(new Vector3(-0.15f, 0.35f, -0.6f), new Vector3(0f, 0f, 35f))},
            { WeaponType.Hammer, new TransformInfo(new Vector3(-0.85f, 2f, -1.25f), new Vector3(0f, 0f, 190f), Vector3.one * 0.75f)},
            { WeaponType.Spear, new TransformInfo(new Vector3(0f, 0.8f, -0.6f), new Vector3(270f, 0f, 0f))},
        };

        public Dictionary<WeaponType, GameObject> WeaponToRenderer;

        public Transform bag { get; private set; }

        public bool IsSupported;

        public override void Start()
        {
            WeaponToRenderer = new Dictionary<WeaponType, GameObject>();
            CreateContainers();
            RefreshRenderers();
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

        public void RefreshRenderers()
        {
            if (!firstPersonMoverReference || !firstPersonMoverReference.IsAttachedAndAlive())
            {
                if (bag)
                {
                    Destroy(bag.gameObject);
                }
                Destroy(this);
                return;
            }

            List<WeaponType> equippedWeapons = firstPersonMoverReference._equippedWeapons;
            WeaponModel[] equippedWeaponModels = firstPersonMoverReference.GetCharacterModel().WeaponModels;
            foreach (WeaponType weaponType in equippedWeapons)
            {
                AddRenderer(weaponType, ref equippedWeapons, ref equippedWeaponModels);
            }

            foreach (KeyValuePair<WeaponType, GameObject> keyValue in WeaponToRenderer)
            {
                bool isEquipped = firstPersonMoverReference.GetEquippedWeaponType() == keyValue.Key;
                bool shouldDisplay = !isEquipped;
                WeaponToRenderer[keyValue.Key].SetActive(shouldDisplay);
            }
        }

        public void AddRenderer(WeaponType weaponType, ref List<WeaponType> equippedWeapons, ref WeaponModel[] equippedWeaponModels)
        {
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
            return renderer.gameObject;
        }
    }
}
