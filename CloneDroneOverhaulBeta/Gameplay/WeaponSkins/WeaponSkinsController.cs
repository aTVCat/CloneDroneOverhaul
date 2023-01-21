using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinsController : ModController
    {
        private static Dictionary<string, WeaponSkinModels> _weaponSkins = new Dictionary<string, WeaponSkinModels>();
        private static Dictionary<string, WeaponSkinPlacement> _modelPlacements = new Dictionary<string, WeaponSkinPlacement>();

        public static readonly Vector3 UsualScale = new Vector3(0.55f, 0.55f, 0.55f);
        public static readonly Vector3 UsualRotation = new Vector3(0, 270, 270);
        public static readonly Vector3 UsualPosition = new Vector3(0, 0, -0.7f);

        public static readonly GameMode EditorGamemode = (GameMode)(MainGameplayController.GamemodeStartIndex + 1);

        public const string SaveDataFileName = "SkinSelection";
        public const string DefaultWeaponSkinName = "VanillaGame";

        public const bool SkinsOnlyForPlayer = false;

        public PlayerSkinsData PlayerSelectedSkinsData;

        public override void Initialize()
        {
            PlayerSelectedSkinsData = PlayerSkinsData.GetData<PlayerSkinsData>(SaveDataFileName);

            if (SkinsOnlyForPlayer)
            {
                OverhaulEventManager.AddListenerToEvent<FirstPersonMover>(MainGameplayController.PlayerSetAsFirstPersonMover, onPlayerFound);
            }
            else
            {
                OverhaulEventManager.AddListenerToEvent<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawned_DelayEventString, onPlayerFound);
            }

            AddWeaponSkin(null, WeaponType.Sword, DefaultWeaponSkinName, default(WeaponSkinPlacement));
            AddWeaponSkin(null, WeaponType.Hammer, DefaultWeaponSkinName, default(WeaponSkinPlacement));
            AddWeaponSkin(null, WeaponType.Bow, DefaultWeaponSkinName, default(WeaponSkinPlacement));
            AddWeaponSkin(null, WeaponType.Spear, DefaultWeaponSkinName, default(WeaponSkinPlacement));
            AddWeaponSkin(null, WeaponType.Shield, DefaultWeaponSkinName, default(WeaponSkinPlacement));

            WeaponSkinModels model1 = AddWeaponSkin(AssetController.GetAsset("SwordSkinDetailedOne", Enumerators.EModAssetBundlePart.WeaponSkins), WeaponType.Sword, "DetailedOne", new WeaponSkinPlacement(UsualPosition, UsualRotation, UsualScale));
            model1.SetFireModel(AssetController.GetAsset("SwordSkinDetailedOne_Fire", Enumerators.EModAssetBundlePart.WeaponSkins), false);
            model1.SetFireModel(AssetController.GetAsset("SwordSkinDetailedOne_Fire", Enumerators.EModAssetBundlePart.WeaponSkins), true);
            model1.SetNormalModel(model1.Normal.Item1, true);

            WeaponSkinModels model2 = AddWeaponSkin(AssetController.GetAsset("SwordSkinDarkPast", Enumerators.EModAssetBundlePart.WeaponSkins), WeaponType.Sword, "Dark Past",
                new WeaponSkinPlacement(new Vector3(0, -0.05f, -0.8f), new Vector3(0, 90, 90), Vector3.one));
            WeaponSkinModels model3 = AddWeaponSkin(AssetController.GetAsset("HammerSkinDarkPast", Enumerators.EModAssetBundlePart.WeaponSkins), WeaponType.Hammer, "Dark Past",
                new WeaponSkinPlacement(new Vector3(-2, -0.05f, 0.12f), new Vector3(0, 0, 270), Vector3.one));

            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        public void ConfirmSkinSelect(in WeaponType weaponType, in string skinName)
        {
            PlayerSelectedSkinsData.SelectSkin(weaponType, skinName);
            RefreshSkins(CharacterTracker.Instance.GetPlayerRobot());
        }

        public WeaponSkinModels GetSkin(in WeaponType weaponType, in string skinName)
        {
            foreach (WeaponSkinModels m in _weaponSkins.Values)
            {
                if (m.WeaponType == weaponType && m.SkinName == skinName)
                {
                    return m;
                }
            }
            return null;
        }
        public WeaponSkinModels GetSkin(in WeaponType weaponType)
        {
            return GetSkin(weaponType, PlayerSelectedSkinsData.Skins[weaponType]);
        }

        public string GetSkinName(in WeaponType weaponType)
        {
            if (!PlayerSelectedSkinsData.Skins.ContainsKey(weaponType))
            {
                return null;
            }
            return PlayerSelectedSkinsData.Skins[weaponType];
        }

        /// <summary>
        /// Get all available skin for weapon
        /// </summary>
        /// <param name="weaponType"></param>
        /// <returns></returns>
        public List<WeaponSkinModels> GetAllSkins(in WeaponType weaponType)
        {
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
        /// Spawn weapon skin if one exists
        /// </summary>
        public GameObject GetAndSpawnSkin(in WeaponModel model, in string skinName, in FirstPersonMover mover, in bool isMultiplayer, in bool isFire = false)
        {
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
            string key = CombineWeaponTypeAndSkinName(model, skinName);
            if (!_weaponSkins.ContainsKey(key) || !_modelPlacements.ContainsKey(key) || skinName == DefaultWeaponSkinName)
            {
                return null;
            }

            RobotDataCollection collection = mover.GetComponent<RobotDataCollection>();
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
                    s = 0.8f
                };
                material.SetColor("_EmissionColor", hsbcolor2.ToColor() * 2.5f);
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

        public WeaponSkinPlacement GetSkinPlacement(WeaponSkinModels models)
        {
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
            return type.ToString() + ":" + name;
        }

        /// <summary>
        /// Start new gamemode where you can change your weapon skin
        /// </summary>
        public void EnterSkinSelectionRoom()
        {
            MainGameplayController.Instance.StartGamemode(EditorGamemode, MainGameplayController.Instance.Levels.GetLevelData(OverhaulBase.Core.ModFolder + "Levels/EditorRooms/WeaponSkinEditorMap.json"), delegate
            {
                FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();

                DelegateScheduler.Instance.Schedule(delegate
                {
                    mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.Hammer, 3);
                    mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.BowUnlock, 1);
                    mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.SpearUnlock, 1);
                    mover.RefreshUpgrades();

                }, 0.2f);

                LevelSection[] sections = FindObjectsOfType<LevelSection>();
                foreach (LevelSection s in sections)
                {
                    s.EnableFromAnimation();
                }
            });
        }

        public bool AllowSkinsOnWeapon(in WeaponType weaponType)
        {
            return PlayerSelectedSkinsData.Skins.ContainsKey(weaponType) && PlayerSelectedSkinsData.Skins[weaponType] != DefaultWeaponSkinName;
        }

        private void onPlayerFound(FirstPersonMover mover)
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                RefreshSkins(mover);

            }, 0.1f);
        }

        public void RefreshSkins(in FirstPersonMover mover)
        {
            if (mover == null || !mover.HasCharacterModel())
            {
                return;
            }

            if (SkinsOnlyForPlayer)
            {
                if (!mover.IsPlayer())
                {
                    return;
                }
            }

            RobotDataCollection collection = mover.GetComponent<RobotDataCollection>();
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
                bool usingVanillaSkin = skinName == null || skinName == DefaultWeaponSkinName || !AllowSkinsOnWeapon(wModel.WeaponType);

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
                    GetAndSpawnSkin(wModel, skinName, mover, GameModeManager.UsesMultiplayerSpeedMultiplier(), wModel.HasReplacedWithFireVariant());
                }
            }
        }
    }
}