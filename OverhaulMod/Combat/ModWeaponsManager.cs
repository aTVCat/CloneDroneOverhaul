using OverhaulMod.Combat.Weapons;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OverhaulMod.Combat
{
    public class ModWeaponsManager : Singleton<ModWeaponsManager>
    {
        public const WeaponType BATTLE_AXE_TYPE = (WeaponType)50;
        public const WeaponType SCYTHE_TYPE = (WeaponType)51;
        public const WeaponType HALBERD_TYPE = (WeaponType)52;
        public const WeaponType HANDS_TYPE = (WeaponType)53;
        public const WeaponType CLAWS_TYPE = (WeaponType)54;
        public const WeaponType PRIM_LASER_BLASTER_TYPE = (WeaponType)55;
        public const WeaponType DUAL_KNIVES_TYPE = (WeaponType)56;
        public const WeaponType BOOMERANG_TYPE = (WeaponType)57;

        private Dictionary<WeaponType, (GameObject, Type)> m_Weapons;
        private List<WeaponType> m_MeleeWeapons;

        public override void Awake()
        {
            base.Awake();

            m_Weapons = new Dictionary<WeaponType, (GameObject, Type)>();
            m_MeleeWeapons = new List<WeaponType>();

            AddWeapon<ScytheWeaponModel>(SCYTHE_TYPE, true, AssetBundleConstants.WEAPONS, "OverhaulScythe");
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.NewGameplayContent))
            {
                AddWeapon<BattleAxeWeaponModel>(BATTLE_AXE_TYPE, true, AssetBundleConstants.WEAPONS, "OverhaulBAxe");
                AddWeapon<HalberdWeaponModel>(HALBERD_TYPE, true, AssetBundleConstants.WEAPONS, "OverhaulHalberd");
                AddWeapon<HandsWeaponModel>(HANDS_TYPE, true, null, null);
                AddWeapon<ClawsWeaponModel>(CLAWS_TYPE, true, null, null);
                AddWeapon<PrimitiveLaserBlasterWeaponModel>(PRIM_LASER_BLASTER_TYPE, false, null, null);
                AddWeapon<DualKnivesWeaponModel>(DUAL_KNIVES_TYPE, true, null, null);
                AddWeapon<BoomerangWeaponModel>(BOOMERANG_TYPE, true, null, null);
            }
        }

        public void AddWeapon<T>(WeaponType weaponType, bool melee, string assetBundle, string assetName) where T : ModWeaponModel
        {
            if (m_Weapons.ContainsKey(weaponType))
                return;

            GameObject model = null;
            if (!assetBundle.IsNullOrEmpty() && !assetName.IsNullOrEmpty())
                model = ModResources.Prefab(assetBundle, assetName);

            m_Weapons.Add(weaponType, (model, typeof(T)));

            if (melee)
                m_MeleeWeapons.Add(weaponType);
        }

        public bool IsMeleeWeapon(WeaponType weaponType)
        {
            return m_MeleeWeapons.Contains(weaponType);
        }

        public void AddWeaponsToRobot(FirstPersonMover firstPersonMover)
        {
            if (GameModeManager.IsMultiplayer() || !firstPersonMover || !ModFeatures.IsEnabled(ModFeatures.FeatureType.NewWeapons))
                return;

            CharacterModel characterModel = firstPersonMover.GetCharacterModel();
            if (!characterModel)
                return;

            if (characterModel.WeaponModels == null)
                characterModel.WeaponModels = new WeaponModel[] { };

            Transform targetTransform = TransformUtils.FindChildRecursive(characterModel.transform, "HandR");
            if (!targetTransform)
            {
                targetTransform = TransformUtils.FindChildRecursive(characterModel.transform, "Head");
                if (!targetTransform)
                    return;
            }

            List<WeaponModel> list = characterModel.WeaponModels.ToList();
            foreach (WeaponType weaponType in m_Weapons.Keys)
            {
                (GameObject, Type) tuple = m_Weapons[weaponType];

                GameObject model = tuple.Item1;
                GameObject gameObject;
                if (model)
                {
                    gameObject = Instantiate(model, targetTransform, false);
                }
                else
                {
                    gameObject = new GameObject(weaponType.ToString());
                    gameObject.transform.SetParent(targetTransform, false);
                }
                gameObject.SetActive(false);

                ModWeaponModel modWeaponModel = (ModWeaponModel)gameObject.AddComponent(tuple.Item2);
                modWeaponModel.WeaponType = weaponType;
                modWeaponModel.SetOwner(firstPersonMover);
                modWeaponModel.OnInstantiated(firstPersonMover);
                list.Add(modWeaponModel);
            }
            characterModel.WeaponModels = list.ToArray();
            firstPersonMover.RefreshModWeaponModels();
        }
    }
}
