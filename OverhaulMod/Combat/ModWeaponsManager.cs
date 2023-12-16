using OverhaulMod.Combat.Weapons;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OverhaulMod
{
    public class ModWeaponsManager : Singleton<ModWeaponsManager>
    {
        private Dictionary<WeaponType, (GameObject, Type)> m_Weapons;
        private List<WeaponType> m_MeleeWeapons;

        public override void Awake()
        {
            base.Awake();

            m_Weapons = new Dictionary<WeaponType, (GameObject, Type)>();
            m_MeleeWeapons = new List<WeaponType>();

            AddWeapon<BattleAxeWeaponModel>((WeaponType)50, true, AssetBundleConstants.WEAPONS, "OverhaulBAxe");
            AddWeapon<ScytheWeaponModel>((WeaponType)51, true, AssetBundleConstants.WEAPONS, "OverhaulScythe");
            AddWeapon<HalberdWeaponModel>((WeaponType)52, true, AssetBundleConstants.WEAPONS, "OverhaulHalberd");
        }

        public void AddWeapon<T>(WeaponType weaponType, bool melee, string assetBundle, string assetName) where T : ModWeaponModel
        {
            if (m_Weapons.ContainsKey(weaponType))
            {
                return;
            }
            m_Weapons.Add(weaponType, (ModResources.GetResource<GameObject>(assetBundle, assetName), typeof(T)));

            if (melee)
            {
                m_MeleeWeapons.Add(weaponType);
            }
        }

        public bool IsMeleeWeapon(WeaponType weaponType)
        {
            return m_MeleeWeapons.Contains(weaponType);
        }

        public void AddWeaponsToRobot(FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover || firstPersonMover._equippedWeapons == null || !ModFeatures.IsEnabled(ModFeatures.FeatureType.NewWeapons) || GameModeManager.IsMultiplayer())
                return;

            CharacterModel characterModel = firstPersonMover.GetCharacterModel();
            if (!characterModel || characterModel.WeaponModels == null)
                return;

            Transform handR = TransformUtils.FindChildRecursive(characterModel.transform, "HandR");
            if (!handR)
                return;

            foreach (WeaponType weaponType in m_Weapons.Keys)
            {
                (GameObject, Type) tuple = m_Weapons[weaponType];

                GameObject gameObject = Instantiate(tuple.Item1, handR, false);
                ModWeaponModel modWeaponModel = (ModWeaponModel)gameObject.AddComponent(tuple.Item2);
                modWeaponModel.WeaponType = weaponType;
                modWeaponModel.SetOwner(firstPersonMover);
                modWeaponModel.Configure(firstPersonMover);

                List<WeaponModel> list = characterModel.WeaponModels.ToList();
                list.Add(modWeaponModel);
                characterModel.WeaponModels = list.ToArray();
                firstPersonMover._equippedWeapons.Add(weaponType);

                EnemyType[] types1 = new EnemyType[]
                {
                    (EnemyType)700,
                    (EnemyType)701,
                    (EnemyType)702,
                    (EnemyType)703,
                    (EnemyType)704
                };
                EnemyType[] types2 = new EnemyType[]
                {
                    (EnemyType)705,
                    (EnemyType)706,
                    (EnemyType)707,
                    (EnemyType)708,
                    (EnemyType)709,
                    (EnemyType)710
                };
                EnemyType[] types3 = new EnemyType[]
                {
                    (EnemyType)711,
                    (EnemyType)712,
                    (EnemyType)713,
                };

                if (modWeaponModel.equipIfEnemy)
                {
                    if (weaponType == (WeaponType)50 && types1.Contains(firstPersonMover.CharacterType))
                    {
                        firstPersonMover.SetEquippedWeaponType(weaponType, false);
                    }
                    if (weaponType == (WeaponType)51 && types2.Contains(firstPersonMover.CharacterType))
                    {
                        firstPersonMover.SetEquippedWeaponType(weaponType, false);
                    }
                    if (weaponType == (WeaponType)52 && types3.Contains(firstPersonMover.CharacterType))
                    {
                        firstPersonMover.SetEquippedWeaponType(weaponType, false);
                    }
                }

                gameObject.SetActive(firstPersonMover.GetEquippedWeaponType() == weaponType);
            }
        }
    }
}
