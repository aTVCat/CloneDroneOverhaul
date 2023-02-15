using OverhaulAPI;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    /// <summary>
    /// Accessories that you can wear!
    /// </summary>
    public static class RobotAccessoriesController
    {
        private static readonly List<RobotAccessoryItemDefinition> _accessories = new List<RobotAccessoryItemDefinition>();
        private static bool _hasPopulatedAccessories;

        public const string AccessoryDestroyVFX_ID = "AccessoryDestroyedVFX";
        public static readonly AudioClipDefinition AccessoryDestroyedSound = AudioAPI.CreateDefinitionUsingClip(AssetController.GetAsset<AudioClip>("BallonExplosion", Enumerators.EModAssetBundlePart.Sounds));

        [OverhaulSetting("Robots.Accessories.Allow enemies to wear accessories", false, false, null)]
        public static bool EnemiesWearAccessories;

        public static RobotPlayerAccessoriesData PlayerData;

        internal static void Initialize()
        {
            OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawned_DelayEventString, RefreshRobot);

            PlayerData = RobotPlayerAccessoriesData.GetData<RobotPlayerAccessoriesData>("PlayerAccessories.json");

            if (_hasPopulatedAccessories) return;
            _hasPopulatedAccessories = true;

            PooledPrefabController.TurnObjectIntoPooledPrefab<RobotAccessoryDestroy_VFX>(AssetController.GetAsset("VFX_AccessoryDestroy", Enumerators.EModAssetBundlePart.Accessories).transform, 5, AccessoryDestroyVFX_ID);

            AddAccessory("Igrok's hat", MechBodyPartType.Head, new SerializeTransform
            {
                Position = new Vector3(0, 0.47f, 0),
                EulerAngles = Vector3.zero,
                LocalScale = new Vector3(0.77f, 0.77f, 0.77f),
            });

            AddAccessory("ImpostorHalo", MechBodyPartType.Head, new SerializeTransform
            {
                Position = new Vector3(0, 0.85f, 0),
                EulerAngles = Vector3.zero,
                LocalScale = new Vector3(0.77f, 0.77f, 0.77f),
            });

            AddAccessory("ImpostorCone", MechBodyPartType.Head, new SerializeTransform
            {
                Position = new Vector3(0, 0.52f, 0),
                EulerAngles = new Vector3(-11, 0, 0),
                LocalScale = new Vector3(0.7f, 0.77f, 0.65f),
            });
        }

        public static void AddAccessory(in string name, MechBodyPartType bodypart, in SerializeTransform transform)
        {
            RobotAccessoryItemDefinition definition = new RobotAccessoryItemDefinition
            {
                TransformInfo = transform,
                AccessoryName = name,
                PartType = bodypart
            };
            definition.SetPrefabUp();
            definition.SetTransformsUp();
            _accessories.Add(definition);
        }

        public static RobotAccessoryItemDefinition GetAccessory(in string name)
        {
            foreach (RobotAccessoryItemDefinition def in _accessories)
            {
                if (def.AccessoryName == name)
                {
                    return def;
                }
            }
            return null;
        }

        public static List<RobotAccessoryItemDefinition> GetAccessoriesForRobot(in FirstPersonMover mover)
        {
            List<RobotAccessoryItemDefinition> result = new List<RobotAccessoryItemDefinition>();
            if (mover.IsMainPlayer())
            {
                foreach (RobotAccessoryItemDefinition def in _accessories)
                {
                    if (PlayerData.Accessories.Contains(def.AccessoryName) && ((IOverhaulItemDefinition)def).IsUnlocked(OverhaulVersion.IsDebugBuild))
                    {
                        result.Add(def);
                    }
                }
            }

            // make accessories random for enemies

            return result;
        }

        public static RobotAccessoryItemDefinition GetAccessoryByPrefabName(in string prefabName)
        {
            foreach (RobotAccessoryItemDefinition def in _accessories)
            {
                if (def.PrefabName == prefabName.Replace("(Clone)", string.Empty))
                {
                    return def;
                }
            }
            return null;
        }

        public static GameObject GetAccessoryPrefab(in string name)
        {
            return GetAccessory(name).AccessoryPrefab;
        }

        public static GameObject SpawnAccessory(in RobotAccessoryItemDefinition def, in FirstPersonMover mover)
        {
            SerializeTransform targetTransform = def.GetTranformForFPM(mover);
            MechBodyPart p = mover.GetBodyPart(def.PartType);
            if (p == null || p.transform.parent == null)
            {
                return new GameObject("ERROR: Could not find transform for accessory");
            }

            Transform s = GameObject.Instantiate(def.AccessoryPrefab, p.transform.parent).transform;
            s.localPosition = targetTransform.Position;
            s.localEulerAngles = targetTransform.EulerAngles;
            s.localScale = targetTransform.LocalScale;
            return s.gameObject;
        }

        public static void RefreshRobot(FirstPersonMover mover)
        {
            RobotAccessoriesWearer w = mover.GetComponent<RobotAccessoriesWearer>();
            if (w == null)
            {
                w = mover.gameObject.AddComponent<RobotAccessoriesWearer>();
                w.Owner = mover;
            }
            w.UnregisterAllAccessories(true);

            //if (!EnemiesWearAccessories) if (GameModeManager.IsMultiplayer() ? !mover.IsMainPlayer() : !mover.IsPlayer()) return;

            foreach (RobotAccessoryItemDefinition def in GetAccessoriesForRobot(mover))
            {
                w.RegisterAccessory(def, mover);
            }
        }
    }
}
