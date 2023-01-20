using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinsController : ModController
    {
        private static Dictionary<string, WeaponSkinModels> _weaponSkins = new Dictionary<string, WeaponSkinModels>();
        private static Dictionary<string, SModelPlacement> _modelPlacements = new Dictionary<string, SModelPlacement>();

        public static readonly Vector3 UsualScale = new Vector3(0.55f, 0.55f, 0.55f);
        public static readonly Vector3 UsualRotation = new Vector3(0, 270, 270);
        public static readonly Vector3 UsualPosition = new Vector3(0, 0, -0.7f);

        public static readonly GameMode EditorGamemode = (GameMode)(OverhaulBase.GamemodeStartIndex + 1);

        public override void Initialize()
        {
            OverhaulEventManager.AddListenerToEvent<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawned_DelayEventString, onFPMSpawned);

            WeaponSkinModels model1 = AddWeaponSkin(AssetController.GetAsset("SwordSkinDetailedOne", Enumerators.EModAssetBundlePart.WeaponSkins), WeaponType.Sword, "DetailedOne", new SModelPlacement(UsualPosition, UsualRotation, UsualScale));
            model1.SetFireModel(AssetController.GetAsset("SwordSkinDetailedOne_Fire", Enumerators.EModAssetBundlePart.WeaponSkins), false);
            model1.SetFireModel(AssetController.GetAsset("SwordSkinDetailedOne_Fire", Enumerators.EModAssetBundlePart.WeaponSkins), true);
            model1.SetNormalModel(model1.Normal.Item1, true);

            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        /// <summary>
        /// Register weapon skin
        /// </summary>
        /// <param name="model"></param>
        /// <param name="type"></param>
        /// <param name="skinName"></param>
        /// <param name="placement"></param>
        /// <returns></returns>
        public WeaponSkinModels AddWeaponSkin(in GameObject model, in WeaponType type, in string skinName, SModelPlacement placement)
        {
            string str = CombineWeaponTypeAndSkinName(type, skinName);
            WeaponSkinModels m = new WeaponSkinModels();
            m.Normal.Item1 = model;

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
        public GameObject GetAndSpawnSkin(in WeaponModel model, in string skinName, in FirstPersonMover mover, in bool isMultiplayer, in bool isFire = false, in bool dontUpdateColors = false)
        {
            GameObject skin = GetAndSpawnSkin(model.WeaponType, null, skinName, mover, isMultiplayer, isFire, dontUpdateColors);

            if (skin == null)
            {
                return null;
            }

            string key = CombineWeaponTypeAndSkinName(model.WeaponType, skinName);
            SModelPlacement placement = _modelPlacements[key];

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
        public GameObject GetAndSpawnSkin(in WeaponType model, in Transform parent, in string skinName, in FirstPersonMover mover, in bool isMultiplayer, in bool isFire = false, in bool dontUpdateColors = false)
        {
            string key = CombineWeaponTypeAndSkinName(model, skinName);
            if (!_weaponSkins.ContainsKey(key) || !_modelPlacements.ContainsKey(key))
            {
                return null;
            }

            RobotDataCollection collection = mover.GetComponent<RobotDataCollection>();
            if (!collection.AllowSkinRegistration(key, isFire, isMultiplayer))
            {
                return null;
            }

            SModelPlacement placement = _modelPlacements[key];

            GameObject skin = null;
            if (!isMultiplayer)
            {
                if (!isFire)
                {
                    skin = Instantiate(_weaponSkins[key].Normal.Item1);
                }
                else
                {
                    if (_weaponSkins[key].Fire.Item1 == null)
                    {
                        skin = Instantiate(_weaponSkins[key].Normal.Item1);
                        goto IL_0000;
                    }
                    skin = Instantiate(_weaponSkins[key].Fire.Item1);
                }
            }
            else
            {
                if (!isFire)
                {
                    skin = Instantiate(_weaponSkins[key].Normal.Item2);
                }
                else
                {
                    if (_weaponSkins[key].Fire.Item2 == null)
                    {
                        skin = Instantiate(_weaponSkins[key].Normal.Item2);
                        goto IL_0000;
                    }
                    skin = Instantiate(_weaponSkins[key].Fire.Item2);
                }
            }

        IL_0000:

            if (!dontUpdateColors && !isFire)
            {
                Material material = skin.GetComponent<Renderer>().material;
                HSBColor hsbcolor2 = new HSBColor(mover.GetCharacterModel().GetFavouriteColor())
                {
                    b = 1f,
                    s = 0.7f
                };
                Color newCol = hsbcolor2.ToColor() * 2.5f;
                material.SetColor("_EmissionColor", newCol);
            }

            collection.RegisterSpawnedSkin(skin, key, isFire, isMultiplayer);
            collection.HideAllSkins();
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
            MainGameplayController.Instance.StartGamemode(EditorGamemode, MainGameplayController.Instance.Levels.GetLevelData(OverhaulBase.Core.ModFolder + "Levels/EditorRooms/WeaponSkinEditorMap.json"));
        }

        private void onFPMSpawned(FirstPersonMover mover)
        {
            if (!mover.HasCharacterModel())
            {
                return;
            }

            WeaponModel[] weaponModelArray = mover.GetCharacterModel().WeaponModels;
            foreach (WeaponModel model in weaponModelArray)
            {
                tuneWeaponModel(model, mover);
            }
        }

        /// <summary>
        /// Hide original weapon models
        /// </summary>
        /// <param name="weaponModel"></param>
        /// <param name="owner"></param>
        private void tuneWeaponModel(in WeaponModel weaponModel, in FirstPersonMover owner)
        {
            switch (weaponModel.WeaponType)
            {
                case WeaponType.Sword:

                    Transform[] partsToDropArray = weaponModel.PartsToDrop;
                    if (partsToDropArray == null || partsToDropArray.Length == 0)
                    {
                        return;
                    }

                    foreach (Transform part in partsToDropArray)
                    {
                        part.gameObject.SetActive(false);
                    }

                    //GetAndSpawnSkin(weaponModel, "DetailedOne", owner, GameModeManager.UsesMultiplayerSpeedMultiplier(), owner.HasUpgrade(UpgradeType.FireSword), owner.HasUpgrade(UpgradeType.FireSword));

                    break;
            }
        }
    }
}