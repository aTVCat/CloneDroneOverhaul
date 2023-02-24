using CDOverhaul.LevelEditor;
using CDOverhaul.Shared;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinsController : OverhaulController
    {
        public static readonly GameMode EditorGamemode = (GameMode)(MainGameplayController.GamemodeStartIndex + 1);
        public const string SaveDataFileName = "SkinSelection";
        public const string DefaultWeaponSkinName = "VanillaGame";

        public const bool SkinsOnlyForPlayer = false;

        public static readonly Vector3 UsualScale = new Vector3(0.55f, 0.55f, 0.55f);
        public static readonly Vector3 UsualRotation = new Vector3(0, 270, 270);
        public static readonly Vector3 UsualPosition = new Vector3(0, 0, -0.7f);

        private static readonly Dictionary<string, WeaponSkinModels> _weaponSkins = new Dictionary<string, WeaponSkinModels>();
        private static readonly Dictionary<string, WeaponSkinPlacement> _modelPlacements = new Dictionary<string, WeaponSkinPlacement>();

        public PlayerSkinsData PlayerSelectedSkinsData;

        public LevelEditorSkinSpawnpoint CustomSkinSpawnpointInLevel;
        private Transform _prevWeaponTransform;

        private PlayerCameraMover _currentCameraMover;

        public const bool UseThisSystem = false;

        public override void Initialize()
        {
            if (!UseThisSystem)
            {
                return;
            }
            PlayerSelectedSkinsData = PlayerSkinsData.GetData<PlayerSkinsData>(SaveDataFileName);

            if (SkinsOnlyForPlayer)
            {
                _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.PlayerSetAsFirstPersonMover, onPlayerFound);
            }
            else
            {
                _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawned_DelayEventString, onPlayerFound);
            }

            _ = AddWeaponSkin(null, WeaponType.Sword, DefaultWeaponSkinName, default);
            _ = AddWeaponSkin(null, WeaponType.Hammer, DefaultWeaponSkinName, default);
            _ = AddWeaponSkin(null, WeaponType.Bow, DefaultWeaponSkinName, default);
            _ = AddWeaponSkin(null, WeaponType.Spear, DefaultWeaponSkinName, default);
            _ = AddWeaponSkin(null, WeaponType.Shield, DefaultWeaponSkinName, default);

            WeaponSkinModels model1 = AddWeaponSkin(AssetController.GetAsset("SwordSkinDetailedOne", Enumerators.OverhaulAssetsPart.WeaponSkins), WeaponType.Sword, "DetailedOne", new WeaponSkinPlacement(UsualPosition, UsualRotation, UsualScale));
            model1.SetFireModel(AssetController.GetAsset("SwordSkinDetailedOne_Fire", Enumerators.OverhaulAssetsPart.WeaponSkins), false);
            model1.SetFireModel(AssetController.GetAsset("SwordSkinDetailedOne_Fire", Enumerators.OverhaulAssetsPart.WeaponSkins), true);
            model1.SetNormalModel(model1.Normal.Item1, true);

            WeaponSkinModels model2 = AddWeaponSkin(AssetController.GetAsset("SwordSkinDarkPast", Enumerators.OverhaulAssetsPart.WeaponSkins), WeaponType.Sword, "Dark Past",
                new WeaponSkinPlacement(new Vector3(0, -0.05f, -0.8f), new Vector3(0, 90, 90), Vector3.one));
            WeaponSkinModels model3 = AddWeaponSkin(AssetController.GetAsset("HammerSkinDarkPast", Enumerators.OverhaulAssetsPart.WeaponSkins), WeaponType.Hammer, "Dark Past",
                new WeaponSkinPlacement(new Vector3(-2, -0.05f, 0.12f), new Vector3(0, 0, 270), Vector3.one));
            WeaponSkinModels model4 = AddWeaponSkin(AssetController.GetAsset("P_Steel", Enumerators.OverhaulAssetsPart.WeaponSkins), WeaponType.Sword, "Steel",
                new WeaponSkinPlacement(new Vector3(-1.14f, -1.14f, 0.7f), new Vector3(0, 0, 0), Vector3.one));
        }

        /// <summary>
        /// Register weapon skin
        /// </summary>
        /// <param name="model"></param>
        /// <param name="type"></param>
        /// <param name="skinName"></param>
        /// <param name="placement"></param>
        /// <returns></returns>
        public WeaponSkinModels AddWeaponSkin(in GameObject model, in WeaponType type, in string skinName, WeaponSkinPlacement placement)
        {
            string str = CombineWeaponTypeAndSkinName(type, skinName);
            WeaponSkinModels m = new WeaponSkinModels();
            m.Normal.Item1 = model;
            m.SkinName = skinName;
            m.WeaponType = type;

            if (!_weaponSkins.ContainsKey(str))
            {
                _weaponSkins.Add(str, m);
            }
            if (!_modelPlacements.ContainsKey(str))
            {
                _modelPlacements.Add(str, placement);
            }
            return m;
        }

        /// <summary>
        /// Get spicific skin by entering weapon type and skins name
        /// </summary>
        /// <param name="weaponType"></param>
        /// <param name="skinName"></param>
        /// <returns></returns>
        public WeaponSkinModels GetSkin(in WeaponType weaponType, in string skinName)
        {
            if (!UseThisSystem)
            {
                return null;
            }
            foreach (WeaponSkinModels m in _weaponSkins.Values)
            {
                if (m.WeaponType == weaponType && m.SkinName == skinName)
                {
                    return m;
                }
            }
            return null;
        }

        /// <summary>
        /// Get skin using player data
        /// </summary>
        /// <param name="weaponType"></param>
        /// <returns></returns>
        public WeaponSkinModels GetSkin(in WeaponType weaponType)
        {
            if (!UseThisSystem)
            {
                return null;
            }
            if (!PlayerSelectedSkinsData.Skins.ContainsKey(weaponType) || GameModeManager.IsInLevelEditor())
            {
                return GetSkin(weaponType, DefaultWeaponSkinName);
            }
            return GetSkin(weaponType, PlayerSelectedSkinsData.Skins[weaponType]);
        }

        /// <summary>
        /// Get skin name using player data
        /// </summary>
        /// <param name="weaponType"></param>
        /// <returns></returns>
        public string GetSkinName(in WeaponType weaponType)
        {
            if (!UseThisSystem)
            {
                return null;
            }
            if (!PlayerSelectedSkinsData.Skins.ContainsKey(weaponType))
            {
                return null;
            }
            return PlayerSelectedSkinsData.Skins[weaponType];
        }

        /// <summary>
        /// Check if we have any skins selected for weapon using player data
        /// </summary>
        /// <param name="weaponType"></param>
        /// <returns></returns>
        public bool HasSkinsEquipedOnWeaponType(in WeaponType weaponType)
        {
            return PlayerSelectedSkinsData.Skins.ContainsKey(weaponType) && PlayerSelectedSkinsData.Skins[weaponType] != DefaultWeaponSkinName;
        }

        /// <summary>
        /// Get all available skin for weapon
        /// </summary>
        /// <param name="weaponType"></param>
        /// <returns></returns>
        public List<WeaponSkinModels> GetAllSkins(in WeaponType weaponType)
        {
            if (!UseThisSystem)
            {
                return null;
            }
            List<WeaponSkinModels> result = new List<WeaponSkinModels>();
            foreach (string str in _weaponSkins.Keys)
            {
                string[] strArray = str.Split(':');
                if (strArray[0] == weaponType.ToString())
                {
                    result.Add(_weaponSkins[str]);
                }
            }
            return result;
        }

        /// <summary>
        /// Spawn weapon skin if one exists
        /// </summary>
        public GameObject GetAndSpawnSkin(in WeaponModel model, in string skinName, in FirstPersonMover mover, in bool isMultiplayer, in bool isFire = false)
        {
            if (!UseThisSystem)
            {
                return null;
            }
            GameObject skin = GetAndSpawnSkin(model.WeaponType, null, skinName, mover, isMultiplayer, isFire);

            if (skin == null)
            {
                return null;
            }

            string key = CombineWeaponTypeAndSkinName(model.WeaponType, skinName);
            WeaponSkinPlacement placement = _modelPlacements[key];

            skin.transform.SetParent(model.transform);
            skin.transform.localPosition = placement.Position;
            skin.transform.localEulerAngles = placement.Rotation;
            skin.transform.localScale = placement.Scale;

            return skin;
        }

        /// <summary>
        /// Spawn weapon skin if one exists
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parent"></param>
        /// <param name="skinName"></param>
        /// <param name="mover"></param>
        /// <param name="isMultiplayer"></param>
        /// <param name="isFire"></param>
        /// <param name="dontUpdateColors"></param>
        /// <returns></returns>
        public GameObject GetAndSpawnSkin(in WeaponType model, in Transform parent, in string skinName, in FirstPersonMover mover, in bool isMultiplayer, in bool isFire = false)
        {
            if (!UseThisSystem)
            {
                return null;
            }
            string key = CombineWeaponTypeAndSkinName(model, skinName);
            if (!_weaponSkins.ContainsKey(key) || !_modelPlacements.ContainsKey(key) || skinName == DefaultWeaponSkinName)
            {
                return null;
            }

            FirstPersonMoverData collection = FirstPersonMoverExtention.GetExtention<FirstPersonMoverData>(mover);
            if (!collection.AllowSkinRegistration(key, isFire, isMultiplayer))
            {
                return null;
            }

            WeaponSkinPlacement placement = _modelPlacements[key];
            GameObject toSpawn = _weaponSkins[key].GetModel(isFire, isMultiplayer);
            if (toSpawn == null)
            {
                return null;
            }
            GameObject skin = Instantiate(toSpawn);

            if (!isFire)
            {
                Material material = skin.GetComponent<Renderer>().material;
                HSBColor hsbcolor2 = new HSBColor(mover.GetCharacterModel().GetFavouriteColor())
                {
                    b = 1f,
                    s = 0.7f
                };
                material.SetColor("_EmissionColor", hsbcolor2.ToColor() * 2.5f);
            }
            else
            {
                if (!_weaponSkins[key].HasFireModel(GameModeManager.IsNonCoopMultiplayer()))
                {
                    Material material = skin.GetComponent<Renderer>().material;
                    HSBColor hsbcolor2 = new HSBColor(HumanFactsManager.Instance.FavouriteColors[5].ColorValue)
                    {
                        b = 1f,
                        s = 0.7f
                    };
                    material.SetColor("_EmissionColor", hsbcolor2.ToColor() * 2.5f);
                }
            }


            collection.RegisterSpawnedSkin(skin, key, isFire, isMultiplayer);
            collection.ShowSkin(key, isFire, isMultiplayer);

            if (parent == null)
            {
                return skin;
            }

            skin.transform.SetParent(parent);
            skin.transform.localPosition = placement.Position;
            skin.transform.localEulerAngles = placement.Rotation;
            skin.transform.localScale = placement.Scale;

            return skin;
        }

        /// <summary>
        /// Get skin position
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public WeaponSkinPlacement GetSkinPlacement(WeaponSkinModels models)
        {
            if (!UseThisSystem)
            {
                return null;
            }
            return _modelPlacements[CombineWeaponTypeAndSkinName(models.WeaponType, models.SkinName)];
        }

        /// <summary>
        /// Turn <see cref="WeaponType"/> and <see cref="string"/> into skin ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string CombineWeaponTypeAndSkinName(in WeaponType type, in string name)
        {
            if (!UseThisSystem)
            {
                return null;
            }
            return type.ToString() + ":" + name;
        }

        /// <summary>
        /// Start new gamemode where you can change your weapon skin
        /// </summary>
        public void EnterSkinSelectionRoom()
        {
            if (!UseThisSystem)
            {
                return;
            }
            if (GameModeManager.Is(EditorGamemode))
            {
                return;
            }

            Transform t = SceneTransitionManager.Instance.InstantiateSceneTransitionOverlay();
            MainGameplayController.Instance.SetGamemode(EditorGamemode);
            MainGameplayController.Instance.Levels.ArenaIsHidden = true;
            DelegateScheduler.Instance.Schedule(delegate
            {
                MainGameplayController.Instance.StartGamemode(EditorGamemode, MainGameplayController.Instance.Levels.GetLevelData(OverhaulMod.Core.ModDirectory + "Levels/EditorRooms/WeaponSkinEditorMap.json"), delegate
                {
                    LevelSection[] sections = FindObjectsOfType<LevelSection>();
                    foreach (LevelSection s in sections)
                    {
                        s.EnableFromAnimation();
                    }
                    Destroy(t.gameObject);
                });
            }, 0.1f);
        }

        /// <summary>
        /// Start creating a skin
        /// </summary>
        public void EnterSkinCreationMode()
        {
            if (!UseThisSystem)
            {
                return;
            }
            MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate = GamemodeSubstate.WeaponSkinCreation;
            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if (mover == null || CustomSkinSpawnpointInLevel == null)
            {
                return;
            }

            RefreshSkins(mover, true);

            CharacterModel m = mover.GetCharacterModel();
            WeaponModel model = m.GetWeaponModel(mover.GetEquippedWeaponType());
            m.UpperAnimator.enabled = false;
            _prevWeaponTransform = model.transform.parent;
            model.transform.SetParent(CustomSkinSpawnpointInLevel.transform);
            model.transform.localEulerAngles = Vector3.zero;
            model.transform.localPosition = Vector3.zero;

            FreeCameraMovement camMove = Camera.main.gameObject.GetComponent<FreeCameraMovement>();
            if (camMove == null)
            {
                _ = Camera.main.gameObject.AddComponent<FreeCameraMovement>();
            }

            _currentCameraMover = Camera.main.GetComponent<PlayerCameraMover>();
            if (_currentCameraMover != null)
            {
                _currentCameraMover.enabled = false;
            }

            _ = model.GetPrimaryVolume();
        }

        /// <summary>
        /// Stop creating a skin
        /// </summary>
        public void ExitSkinCreationMode()
        {
            if (!UseThisSystem)
            {
                return;
            }
            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if (mover == null)
            {
                return;
            }

            FreeCameraMovement camMove = Camera.main.gameObject.GetComponent<FreeCameraMovement>();
            if (camMove != null)
            {
                if (_currentCameraMover != null)
                {
                    _currentCameraMover.enabled = true;
                }

                Destroy(camMove);
            }

            Camera.main.transform.localEulerAngles = Vector3.zero;

            RefreshSkins(mover, false);

            CharacterModel m = mover.GetCharacterModel();
            WeaponModel model = m.GetWeaponModel(mover.GetEquippedWeaponType());
            m.UpperAnimator.enabled = true;
            model.transform.SetParent(_prevWeaponTransform);
        }

        /// <summary>
        /// Select the skin and save the data
        /// </summary>
        /// <param name="weaponType"></param>
        /// <param name="skinName"></param>
        public void ConfirmSkinSelect(in WeaponType weaponType, in string skinName)
        {
            if (!UseThisSystem)
            {
                return;
            }
            if (MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate == GamemodeSubstate.WeaponSkinCreation)
            {
                return;
            }

            PlayerSelectedSkinsData.SelectSkin(weaponType, skinName);
            RefreshSkins(CharacterTracker.Instance.GetPlayerRobot());
        }

        private void onPlayerFound(FirstPersonMover mover)
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                RefreshSkins(mover);
            }, 0.1f);
        }

        /// <summary>
        /// Instantiate or remove skins on <see cref="FirstPersonMover"/> only!
        /// </summary>
        /// <param name="mover"></param>
        /// <param name="default">Force use of vanilla skins</param>
        public void RefreshSkins(in FirstPersonMover mover, in bool @default = false)
        {
            if (!UseThisSystem)
            {
                return;
            }
            if (mover == null || !mover.HasCharacterModel() || mover.IsMindSpaceCharacter)
            {
                return;
            }
            if (SkinsOnlyForPlayer && !mover.IsPlayer())
            {
                return;
            }

            FirstPersonMoverData collection = FirstPersonMoverExtention.GetExtention<FirstPersonMoverData>(mover);
            if (collection == null)
            {
                return;
            }
            collection.DestroyAllSkins();

            CharacterModel cModel = mover.GetCharacterModel();
            WeaponModel[] weaponModelArray = cModel.WeaponModels;

            foreach (WeaponModel wModel in weaponModelArray)
            {
                string skinName = GetSkinName(wModel.WeaponType);
                bool usingVanillaSkin = skinName == null || skinName == DefaultWeaponSkinName || !HasSkinsEquipedOnWeaponType(wModel.WeaponType);
                if (@default)
                {
                    usingVanillaSkin = true;
                }

                Transform[] partsToDropArray = wModel.PartsToDrop;
                if (partsToDropArray != null && partsToDropArray.Length != 0)
                {
                    foreach (Transform part in partsToDropArray)
                    {
                        part.gameObject.SetActive(usingVanillaSkin);
                    }
                }


                if (!usingVanillaSkin)
                {
                    _ = GetAndSpawnSkin(wModel, skinName, mover, GameModeManager.UsesMultiplayerSpeedMultiplier(), wModel.HasReplacedWithFireVariant());
                }
            }
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}