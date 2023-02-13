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

        internal static void Initialize()
        {
            OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawned_DelayEventString, RefreshRobot);

            if (_hasPopulatedAccessories) return;
            _hasPopulatedAccessories = true;

            PooledPrefabController.TurnObjectIntoPooledPrefab<RobotAccessoryDestroy_VFX>(AssetController.GetAsset("VFX_AccessoryDestroy", Enumerators.EModAssetBundlePart.Accessories).transform, 5, AccessoryDestroyVFX_ID);

            AddAccessory("Igrok's hat", MechBodyPartType.Head, new SerializeTransform
            {
                Position = new Vector3(0, 0.47f, 0),
                EulerAngles = Vector3.zero,
                LocalScale = new Vector3(0.77f, 0.77f, 0.77f),
            });
        }

        public static void AddAccessory(in string name, MechBodyPartType bodypart, in SerializeTransform transform)
        {
            RobotAccessoryItemDefinition definition = new RobotAccessoryItemDefinition
            {
                Transforminfo = transform,
                AccessoryName = name,
                PartType = bodypart
            };
            definition.SetPrefabUp();
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

        public static GameObject GetAccessoryPrefab(in string name)
        {
            return GetAccessory(name).AccessoryPrefab;
        }

        public static GameObject SpawnAccessory(in string name, in Transform parent)
        {
            RobotAccessoryItemDefinition g = GetAccessory(name);
            Transform s = GameObject.Instantiate(g.AccessoryPrefab, parent.parent).transform;
            s.localPosition = g.Transforminfo.Position;
            s.localEulerAngles = g.Transforminfo.EulerAngles;
            s.localScale = g.Transforminfo.LocalScale;
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

            //if (GameModeManager.IsMultiplayer() ? !mover.IsMainPlayer() : !mover.IsPlayer()) return;

            Transform head = mover.GetBodyPart(MechBodyPartType.Head).transform;
            if (head == null) return;

            w.RegisterAccessory(SpawnAccessory("Igrok's hat", head));
        }
    }
}
