using CDOverhaul.HUD;
using OverhaulAPI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay
{
    /// <summary>
    /// Accessories that you can wear!
    /// </summary>
    public static class RobotAccessoriesController
    {
        /// <summary>
        /// The data id <see cref="MultiplayerAPI"/> uses for data requests
        /// </summary>
        public const string DataID = "AccessoryDataID";

        /// <summary>
        /// All the accessories in da mod
        /// </summary>
        private static readonly List<RobotAccessoryItemDefinition> _accessories = new List<RobotAccessoryItemDefinition>();
        private static bool _hasPopulatedAccessories;

        /// <summary>
        /// <see cref="PooledPrefabController"/> needs IDs for pooled objects and this one is not an exception
        /// </summary>
        public const string AccessoryDestroyVFX_ID = "AccessoryDestroyedVFX";
        /// <summary>
        /// The sound that is played when someone hits an accessory
        /// </summary>
        public static readonly AudioClipDefinition AccessoryDestroyedSound = AudioAPI.CreateDefinitionUsingClip(AssetController.GetAsset<AudioClip>("BallonExplosion", Enumerators.ModAssetBundlePart.Sounds));

        /// <summary>
        /// This one is gonna be removed soon...
        /// </summary>
        [OverhaulSetting("Robots.Accessories.Allow enemies to wear accessories", false, false, null)]
        public static bool EnemiesWearAccessories;

        /// <summary>
        /// Mainly a collection of accessories that user has equiped
        /// </summary>
        public static RobotAccessorySaveData PlayerData;

        /// <summary>
        /// An editor that allows me to make every accessory fit well on every robot
        /// </summary>
        public static RobotAccessoriesEditorUI EditorUI;

        internal static void Initialize()
        {
            if (OverhaulVersion.IsDebugBuild)
            {
                EditorUI = OverhaulMod.Core.HUDController.AddHUD<RobotAccessoriesEditorUI>(OverhaulMod.Core.HUDController.HUDModdedObject.GetObject<ModdedObject>(5));
            }
            else
            {
                OverhaulMod.Core.HUDController.HUDModdedObject.GetObject<ModdedObject>(5).gameObject.SetActive(false);
            }

            PooledPrefabController.TurnObjectIntoPooledPrefab<RobotAccessoryDestroyVFX>(AssetController.GetAsset("VFX_AccessoryDestroy", Enumerators.ModAssetBundlePart.Accessories).transform, 5, AccessoryDestroyVFX_ID);
            MultiplayerAPI.RegisterRequestAndAnswerListener(DataID, OnReceivedRequest, OnReceivedAnswer);
            PlayerData = RobotAccessorySaveData.GetData<RobotAccessorySaveData>("PlayerAccessories");

            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawned_DelayEventString, RefreshRobot);
            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.PlayerSetAsFirstPersonMover, ScheduleRefresingRobot);
            _ = OverhaulEventManager.AddEventListener(GamemodeSubstatesController.SubstateChangedEventString, TryEnterEditor);

            if (_hasPopulatedAccessories)
            {
                return;
            }
            _hasPopulatedAccessories = true;

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

            AddAccessory("PussHat", MechBodyPartType.Head, new SerializeTransform
            {
                Position = new Vector3(0, 0.55f, 0),
                EulerAngles = new Vector3(3, 0, 3),
                LocalScale = new Vector3(0.46f, 0.52f, 0.46f),
            });
        }

        /// <summary>
        /// Get all accessories, including locked ones
        /// </summary>
        /// <returns></returns>
        public static List<RobotAccessoryItemDefinition> GetAllAccessories()
        {
            return _accessories;
        }

        /// <summary>
        /// Get dropdown options
        /// </summary>
        /// <returns></returns>
        public static List<Dropdown.OptionData> GetAllAccessoriesDropdownOptions()
        {
            List<Dropdown.OptionData> l = new List<Dropdown.OptionData>(_accessories.Count);
            foreach (RobotAccessoryItemDefinition def in GetAllAccessories())
            {
                l.Add(new Dropdown.OptionData(def.AccessoryName));
            }
            return l;
        }

        /// <summary>
        /// Start accessory editing mod if possible
        /// </summary>
        public static void TryEnterEditor()
        {
            if (!RobotAccessoriesEditorUI.MayUseEditor || Time.timeSinceLevelLoad < 3f)
            {
                return;
            }

            bool activate = MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate == GamemodeSubstate.EditingAccessories;
            EditorUI.SetActive(activate);
        }

        /// <summary>
        /// Register new accessory
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bodypart"></param>
        /// <param name="transform"></param>
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

        /// <summary>
        /// Get accessory by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Get accessory by prefab name
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get accessories for robot <b>(The result depends on what kind of robot is used. For ex. player)</b>
        /// </summary>
        /// <param name="mover"></param>
        /// <param name="accessories"></param>
        /// <returns></returns>
        public static List<RobotAccessoryItemDefinition> GetAccessoriesForRobot(in FirstPersonMover mover, List<string> accessories = null)
        {
            List<RobotAccessoryItemDefinition> result = new List<RobotAccessoryItemDefinition>();
            foreach (RobotAccessoryItemDefinition def in _accessories)
            {
                if (GameModeManager.IsMultiplayer())
                {
                    if (accessories.Contains(def.AccessoryName))
                    {
                        result.Add(def);
                    }
                }
                else
                {
                    if (accessories.Contains(def.AccessoryName) && ((IOverhaulItemDefinition)def).IsUnlocked(false))
                    {
                        result.Add(def);
                    }
                }
            }

            // make accessories random for enemies

            return result;
        }

        /// <summary>
        /// Spawn accessory and attach it to <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="def"></param>
        /// <param name="mover"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Multiplayer. Called when we someone needs to know, what we are wearing xd
        /// </summary>
        /// <param name="str"></param>
        public static void OnReceivedRequest(string[] str)
        {
            List<string> list = PlayerData.Accessories;
            MultiplayerAPI.Answer = FastSerialization.SerializeObject(list);

            OverhaulDebugController.Print(MultiplayerAPI.Answer);
        }

        /// <summary>
        /// Multiplayer. Called when we got other player's data
        /// </summary>
        /// <param name="str"></param>
        public static void OnReceivedAnswer(string[] str)
        {
            OverhaulDebugController.Print("Answer!!");
            OverhaulDebugController.Print(str[3]);
            OverhaulDebugController.Print(str[4]);

            List<string> list = FastSerialization.DeserializeObject<List<string>>(str[4]);
            FirstPersonMover m = (FirstPersonMover)CharacterTracker.Instance.TryGetLivingCharacterWithPlayFabID(str[3]);
            if (m != null)
            {
                UpdateRobot(m, list);
            }
        }

        /// <summary>
        /// Wait 0.2 seconds and refresh robot accessories
        /// </summary>
        /// <param name="mover"></param>
        public static void ScheduleRefresingRobot(FirstPersonMover mover)
        {
            if (!GameModeManager.IsMultiplayer())
            {
                DelegateScheduler.Instance.Schedule(delegate { RefreshRobot(mover); }, 0.2f);
            }
        }

        /// <summary>
        /// Refresh robot accessories. This method is mainly used for multiplayer
        /// </summary>
        /// <param name="mover"></param>
        public static void RefreshRobot(FirstPersonMover mover)
        {
            if (mover == null)
            {
                return;
            }

            if (GameModeManager.IsMultiplayer())
            {
                if (!mover.IsPlayer())
                {
                    return;
                }

                MultiplayerAPI.RequestDataFromPlayer(mover, DataID, OverhaulDebugController.PrintError);

                return;
            }

            UpdateRobot(mover, PlayerData.Accessories);
        }

        /// <summary>
        /// Refresh robot accessories
        /// </summary>
        /// <param name="mover"></param>
        /// <param name="accs"></param>
        public static void UpdateRobot(FirstPersonMover mover, List<string> accs)
        {
            if (mover == null || !OverhaulGamemodeManager.SupportsAccessories())
            {
                return;
            }

            RobotAccessoriesWearer w = mover.GetComponent<RobotAccessoriesWearer>();
            if (w == null)
            {
                w = mover.gameObject.AddComponent<RobotAccessoriesWearer>();
                w.Owner = mover;
            }
            w.UnregisterAllAccessories(true);

            bool fpmSupportsAccessories = RobotAccessoryItemDefinition.GetIndexOfCharacterModel(mover) != -1;
            if (!fpmSupportsAccessories)
            {
                return;
            }

            // Todo: make enemies wear different accessories
            List<RobotAccessoryItemDefinition> list = GetAccessoriesForRobot(mover, accs);
            if (list != null && list.Count != 0)
            {
                foreach (RobotAccessoryItemDefinition def in list)
                {
                    w.RegisterAccessory(def, mover);
                }
            }
        }
    }
}
